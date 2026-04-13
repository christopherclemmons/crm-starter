using Crm.Api.Contracts;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Crm.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/cases")]
public sealed class CasesController(CrmDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CaseResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.Cases.AsNoTracking()
            .OrderByDescending(x => x.OpenedAtUtc)
            .Select(x => new CaseResponse(x.Id, x.Subject, x.AccountId, x.OwnerName, x.Description, x.ContactId, x.Priority, x.Status, x.OpenedAtUtc, x.ResolvedAtUtc, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CaseResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Cases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return Ok(new CaseResponse(entity.Id, entity.Subject, entity.AccountId, entity.OwnerName, entity.Description, entity.ContactId, entity.Priority, entity.Status, entity.OpenedAtUtc, entity.ResolvedAtUtc, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPost]
    public async Task<ActionResult<CaseResponse>> Create(CaseRequest request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Accounts.AnyAsync(x => x.Id == request.AccountId, cancellationToken))
        {
            return BadRequest("The referenced account does not exist.");
        }

        if (request.ContactId.HasValue && !await dbContext.Contacts.AnyAsync(x => x.Id == request.ContactId.Value, cancellationToken))
        {
            return BadRequest("The referenced contact does not exist.");
        }

        var entity = new Case
        {
            Subject = request.Subject.Trim(),
            AccountId = request.AccountId,
            OwnerName = request.OwnerName.Trim(),
            Description = request.Description?.Trim(),
            ContactId = request.ContactId,
            Priority = request.Priority,
            Status = request.Status
        };

        CrmValidator.EnsureCaseResolution(entity);
        dbContext.Cases.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new CaseResponse(entity.Id, entity.Subject, entity.AccountId, entity.OwnerName, entity.Description, entity.ContactId, entity.Priority, entity.Status, entity.OpenedAtUtc, entity.ResolvedAtUtc, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CaseResponse>> Update(Guid id, CaseRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Cases.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (!await dbContext.Accounts.AnyAsync(x => x.Id == request.AccountId, cancellationToken))
        {
            return BadRequest("The referenced account does not exist.");
        }

        if (request.ContactId.HasValue && !await dbContext.Contacts.AnyAsync(x => x.Id == request.ContactId.Value, cancellationToken))
        {
            return BadRequest("The referenced contact does not exist.");
        }

        entity.Subject = request.Subject.Trim();
        entity.AccountId = request.AccountId;
        entity.OwnerName = request.OwnerName.Trim();
        entity.Description = request.Description?.Trim();
        entity.ContactId = request.ContactId;
        entity.Priority = request.Priority;
        entity.Status = request.Status;

        CrmValidator.EnsureCaseResolution(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new CaseResponse(entity.Id, entity.Subject, entity.AccountId, entity.OwnerName, entity.Description, entity.ContactId, entity.Priority, entity.Status, entity.OpenedAtUtc, entity.ResolvedAtUtc, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }
}
