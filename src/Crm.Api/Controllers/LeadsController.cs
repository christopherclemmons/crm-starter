using Crm.Api.Contracts;
using Crm.Api.Domain;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Crm.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/leads")]
public sealed class LeadsController(CrmDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<LeadResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.Leads.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new LeadResponse(x.Id, x.FullName, x.OwnerName, x.Source, x.CompanyName, x.Email, x.PhoneNumber, x.Notes, x.Stage, x.ConvertedAccountId, x.ConvertedContactId, x.ConvertedOpportunityId, x.ConvertedAtUtc, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeadResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Leads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return Ok(new LeadResponse(entity.Id, entity.FullName, entity.OwnerName, entity.Source, entity.CompanyName, entity.Email, entity.PhoneNumber, entity.Notes, entity.Stage, entity.ConvertedAccountId, entity.ConvertedContactId, entity.ConvertedOpportunityId, entity.ConvertedAtUtc, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPost]
    public async Task<ActionResult<LeadResponse>> Create(LeadRequest request, CancellationToken cancellationToken)
    {
        var entity = new Lead
        {
            FullName = request.FullName.Trim(),
            OwnerName = request.OwnerName.Trim(),
            Source = request.Source.Trim(),
            CompanyName = request.CompanyName?.Trim(),
            Email = request.Email?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            Notes = request.Notes?.Trim(),
            Stage = request.Stage
        };

        dbContext.Leads.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new LeadResponse(entity.Id, entity.FullName, entity.OwnerName, entity.Source, entity.CompanyName, entity.Email, entity.PhoneNumber, entity.Notes, entity.Stage, entity.ConvertedAccountId, entity.ConvertedContactId, entity.ConvertedOpportunityId, entity.ConvertedAtUtc, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LeadResponse>> Update(Guid id, LeadRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Stage == LeadStage.Converted)
        {
            return BadRequest("Converted leads cannot be edited.");
        }

        entity.FullName = request.FullName.Trim();
        entity.OwnerName = request.OwnerName.Trim();
        entity.Source = request.Source.Trim();
        entity.CompanyName = request.CompanyName?.Trim();
        entity.Email = request.Email?.Trim();
        entity.PhoneNumber = request.PhoneNumber?.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.Stage = request.Stage;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new LeadResponse(entity.Id, entity.FullName, entity.OwnerName, entity.Source, entity.CompanyName, entity.Email, entity.PhoneNumber, entity.Notes, entity.Stage, entity.ConvertedAccountId, entity.ConvertedContactId, entity.ConvertedOpportunityId, entity.ConvertedAtUtc, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPost("{id:guid}/convert")]
    public async Task<ActionResult<LeadResponse>> Convert(Guid id, ConvertLeadRequest request, CancellationToken cancellationToken)
    {
        var lead = await dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (lead is null)
        {
            return NotFound();
        }

        CrmValidator.EnsureLeadConvertible(lead);

        var account = new Account
        {
            Name = request.AccountName.Trim(),
            OwnerName = lead.OwnerName,
            PhoneNumber = lead.PhoneNumber
        };

        var names = lead.FullName.Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var contact = new Contact
        {
            FirstName = names.FirstOrDefault() ?? lead.FullName,
            LastName = names.Skip(1).FirstOrDefault() ?? "Unknown",
            OwnerName = lead.OwnerName,
            Email = lead.Email,
            PhoneNumber = lead.PhoneNumber,
            Account = account
        };

        var opportunity = new Opportunity
        {
            Name = request.OpportunityName.Trim(),
            Account = account,
            PrimaryContact = contact,
            OwnerName = lead.OwnerName,
            EstimatedValue = request.EstimatedValue,
            ExpectedCloseDate = request.ExpectedCloseDate,
            Stage = OpportunityStage.Qualification
        };

        lead.Stage = LeadStage.Converted;
        lead.ConvertedAtUtc = DateTime.UtcNow;
        lead.ConvertedAccountId = account.Id;
        lead.ConvertedContactId = contact.Id;
        lead.ConvertedOpportunityId = opportunity.Id;

        dbContext.Accounts.Add(account);
        dbContext.Contacts.Add(contact);
        dbContext.Opportunities.Add(opportunity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new LeadResponse(lead.Id, lead.FullName, lead.OwnerName, lead.Source, lead.CompanyName, lead.Email, lead.PhoneNumber, lead.Notes, lead.Stage, lead.ConvertedAccountId, lead.ConvertedContactId, lead.ConvertedOpportunityId, lead.ConvertedAtUtc, lead.CreatedAtUtc, lead.UpdatedAtUtc));
    }
}
