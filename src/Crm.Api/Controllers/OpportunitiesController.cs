using Crm.Api.Contracts;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Crm.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/opportunities")]
public sealed class OpportunitiesController(CrmDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OpportunityResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.Opportunities.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new OpportunityResponse(x.Id, x.Name, x.AccountId, x.OwnerName, x.PrimaryContactId, x.EstimatedValue, x.ExpectedCloseDate, x.Stage, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OpportunityResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Opportunities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return Ok(new OpportunityResponse(entity.Id, entity.Name, entity.AccountId, entity.OwnerName, entity.PrimaryContactId, entity.EstimatedValue, entity.ExpectedCloseDate, entity.Stage, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPost]
    public async Task<ActionResult<OpportunityResponse>> Create(OpportunityRequest request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Accounts.AnyAsync(x => x.Id == request.AccountId, cancellationToken))
        {
            return BadRequest("The referenced account does not exist.");
        }

        if (request.PrimaryContactId.HasValue && !await dbContext.Contacts.AnyAsync(x => x.Id == request.PrimaryContactId.Value, cancellationToken))
        {
            return BadRequest("The referenced contact does not exist.");
        }

        var entity = new Opportunity
        {
            Name = request.Name.Trim(),
            AccountId = request.AccountId,
            OwnerName = request.OwnerName.Trim(),
            PrimaryContactId = request.PrimaryContactId,
            EstimatedValue = request.EstimatedValue,
            ExpectedCloseDate = request.ExpectedCloseDate,
            Stage = request.Stage
        };

        dbContext.Opportunities.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new OpportunityResponse(entity.Id, entity.Name, entity.AccountId, entity.OwnerName, entity.PrimaryContactId, entity.EstimatedValue, entity.ExpectedCloseDate, entity.Stage, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OpportunityResponse>> Update(Guid id, OpportunityRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Opportunities.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (!await dbContext.Accounts.AnyAsync(x => x.Id == request.AccountId, cancellationToken))
        {
            return BadRequest("The referenced account does not exist.");
        }

        if (request.PrimaryContactId.HasValue && !await dbContext.Contacts.AnyAsync(x => x.Id == request.PrimaryContactId.Value, cancellationToken))
        {
            return BadRequest("The referenced contact does not exist.");
        }

        CrmValidator.EnsureOpportunityStageTransition(entity.Stage, request.Stage);

        entity.Name = request.Name.Trim();
        entity.AccountId = request.AccountId;
        entity.OwnerName = request.OwnerName.Trim();
        entity.PrimaryContactId = request.PrimaryContactId;
        entity.EstimatedValue = request.EstimatedValue;
        entity.ExpectedCloseDate = request.ExpectedCloseDate;
        entity.Stage = request.Stage;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new OpportunityResponse(entity.Id, entity.Name, entity.AccountId, entity.OwnerName, entity.PrimaryContactId, entity.EstimatedValue, entity.ExpectedCloseDate, entity.Stage, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }
}
