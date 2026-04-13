using Crm.Api.Contracts;
using Crm.Api.Domain;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Crm.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(CrmDbContext dbContext, CrmValidator validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TaskResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var items = await dbContext.Tasks.AsNoTracking()
            .OrderBy(x => x.DueDate)
            .Select(x => new TaskResponse(x.Id, x.Title, x.OwnerName, x.ParentType, x.ParentId, x.Description, x.DueDate, x.Status, x.CompletedAtUtc, x.Status != Domain.TaskStatus.Completed && x.DueDate.HasValue && x.DueDate.Value < today, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(TaskRequest request, CancellationToken cancellationToken)
    {
        await validator.EnsureParentExistsAsync(request.ParentType, request.ParentId, cancellationToken);

        var entity = new TaskItem
        {
            Title = request.Title.Trim(),
            OwnerName = request.OwnerName.Trim(),
            ParentType = request.ParentType,
            ParentId = request.ParentId,
            Description = request.Description?.Trim(),
            DueDate = request.DueDate,
            Status = request.Status
        };

        CrmValidator.EnsureTaskCompletion(entity);
        dbContext.Tasks.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetAll), null, ToResponse(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(Guid id, TaskRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await validator.EnsureParentExistsAsync(request.ParentType, request.ParentId, cancellationToken);

        entity.Title = request.Title.Trim();
        entity.OwnerName = request.OwnerName.Trim();
        entity.ParentType = request.ParentType;
        entity.ParentId = request.ParentId;
        entity.Description = request.Description?.Trim();
        entity.DueDate = request.DueDate;
        entity.Status = request.Status;

        CrmValidator.EnsureTaskCompletion(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(entity));
    }

    private static TaskResponse ToResponse(TaskItem entity)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var isOverdue = entity.Status != Domain.TaskStatus.Completed && entity.DueDate.HasValue && entity.DueDate.Value < today;
        return new TaskResponse(entity.Id, entity.Title, entity.OwnerName, entity.ParentType, entity.ParentId, entity.Description, entity.DueDate, entity.Status, entity.CompletedAtUtc, isOverdue, entity.CreatedAtUtc, entity.UpdatedAtUtc);
    }
}
