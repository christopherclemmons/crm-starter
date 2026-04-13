using Crm.Api.Contracts;
using Crm.Api.Domain;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Crm.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class AccountsController(CrmDbContext dbContext, CrmValidator validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AccountResponse>>> GetAll([FromQuery] string? search, [FromQuery] AccountStatus? status, CancellationToken cancellationToken)
    {
        var query = dbContext.Accounts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(normalized));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var items = await query
            .OrderBy(x => x.Name)
            .Select(x => new AccountResponse(x.Id, x.Name, x.OwnerName, x.Industry, x.Website, x.PhoneNumber, x.BillingCity, x.BillingCountry, x.Status, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return Ok(new AccountResponse(entity.Id, entity.Name, entity.OwnerName, entity.Industry, entity.Website, entity.PhoneNumber, entity.BillingCity, entity.BillingCountry, entity.Status, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpGet("{id:guid}/overview")]
    public async Task<ActionResult<AccountOverviewResponse>> GetOverview(Guid id, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }

        var opportunities = await dbContext.Opportunities.AsNoTracking()
            .Where(x => x.AccountId == id && x.Stage != OpportunityStage.ClosedWon && x.Stage != OpportunityStage.ClosedLost)
            .OrderBy(x => x.ExpectedCloseDate)
            .Select(x => new OpportunitySummary(x.Id, x.Name, x.Stage.ToString(), x.EstimatedValue, x.ExpectedCloseDate))
            .ToListAsync(cancellationToken);

        var activities = await dbContext.Activities.AsNoTracking()
            .Where(x => x.ParentType == EntityType.Account && x.ParentId == id)
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(10)
            .Select(x => new ActivitySummary(x.Id, x.Summary, x.Type.ToString(), x.OccurredAtUtc))
            .ToListAsync(cancellationToken);

        var activeCases = await dbContext.Cases.AsNoTracking()
            .Where(x => x.AccountId == id && x.Status != CaseStatus.Closed)
            .OrderByDescending(x => x.OpenedAtUtc)
            .Select(x => new CaseSummary(x.Id, x.Subject, x.Status.ToString(), x.Priority.ToString(), x.OpenedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(new AccountOverviewResponse(account.Id, account.Name, account.Status.ToString(), opportunities, activities, activeCases));
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create(AccountRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAccountNameUniqueAsync(request.Name, null, cancellationToken);

        var entity = new Account
        {
            Name = request.Name.Trim(),
            OwnerName = request.OwnerName.Trim(),
            Industry = request.Industry?.Trim(),
            Website = request.Website?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            BillingCity = request.BillingCity?.Trim(),
            BillingCountry = request.BillingCountry?.Trim(),
            Status = request.Status
        };

        dbContext.Accounts.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new AccountResponse(entity.Id, entity.Name, entity.OwnerName, entity.Industry, entity.Website, entity.PhoneNumber, entity.BillingCity, entity.BillingCountry, entity.Status, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccountResponse>> Update(Guid id, AccountRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await validator.ValidateAccountNameUniqueAsync(request.Name, id, cancellationToken);

        entity.Name = request.Name.Trim();
        entity.OwnerName = request.OwnerName.Trim();
        entity.Industry = request.Industry?.Trim();
        entity.Website = request.Website?.Trim();
        entity.PhoneNumber = request.PhoneNumber?.Trim();
        entity.BillingCity = request.BillingCity?.Trim();
        entity.BillingCountry = request.BillingCountry?.Trim();
        entity.Status = request.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new AccountResponse(entity.Id, entity.Name, entity.OwnerName, entity.Industry, entity.Website, entity.PhoneNumber, entity.BillingCity, entity.BillingCountry, entity.Status, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }
}
