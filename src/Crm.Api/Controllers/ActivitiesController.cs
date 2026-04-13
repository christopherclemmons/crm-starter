using Crm.Api.Contracts;
using Crm.Api.Domain;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Crm.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/activities")]
public sealed class ActivitiesController(CrmDbContext dbContext, CrmValidator validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ActivityResponse>>> GetAll([FromQuery] Guid? parentId, [FromQuery] EntityType? parentType, CancellationToken cancellationToken)
    {
        var query = dbContext.Activities.AsNoTracking().AsQueryable();

        if (parentId.HasValue)
        {
            query = query.Where(x => x.ParentId == parentId.Value);
        }

        if (parentType.HasValue)
        {
            query = query.Where(x => x.ParentType == parentType.Value);
        }

        var items = await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .Select(x => new ActivityResponse(x.Id, x.Type, x.Summary, x.OwnerName, x.ParentType, x.ParentId, x.Details, x.OccurredAtUtc, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ActivityResponse>> Create(ActivityRequest request, CancellationToken cancellationToken)
    {
        await validator.EnsureParentExistsAsync(request.ParentType, request.ParentId, cancellationToken);

        var entity = new Activity
        {
            Type = request.Type,
            Summary = request.Summary.Trim(),
            OwnerName = request.OwnerName.Trim(),
            ParentType = request.ParentType,
            ParentId = request.ParentId,
            Details = request.Details?.Trim(),
            OccurredAtUtc = request.OccurredAtUtc ?? DateTime.UtcNow
        };

        dbContext.Activities.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { parentId = entity.ParentId, parentType = entity.ParentType }, new ActivityResponse(entity.Id, entity.Type, entity.Summary, entity.OwnerName, entity.ParentType, entity.ParentId, entity.Details, entity.OccurredAtUtc, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }
}
