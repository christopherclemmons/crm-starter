using Crm.Api.Domain;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Infrastructure.Validation;

public sealed class CrmValidator(CrmDbContext dbContext)
{
    public async Task ValidateAccountNameUniqueAsync(string name, Guid? currentId, CancellationToken cancellationToken)
    {
        var normalized = name.Trim().ToLower();
        var exists = await dbContext.Accounts.AnyAsync(x => x.Name.ToLower() == normalized && x.Id != currentId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Account name must be unique.");
        }
    }

    public async Task ValidateContactEmailUniqueAsync(string? email, Guid? currentId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var normalized = email.Trim().ToLower();
        var exists = await dbContext.Contacts.AnyAsync(x => x.Email != null && x.Email.ToLower() == normalized && x.Id != currentId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Contact email must be unique.");
        }
    }

    public async Task EnsureParentExistsAsync(EntityType parentType, Guid parentId, CancellationToken cancellationToken)
    {
        var exists = parentType switch
        {
            EntityType.Account => await dbContext.Accounts.AnyAsync(x => x.Id == parentId, cancellationToken),
            EntityType.Contact => await dbContext.Contacts.AnyAsync(x => x.Id == parentId, cancellationToken),
            EntityType.Lead => await dbContext.Leads.AnyAsync(x => x.Id == parentId, cancellationToken),
            EntityType.Opportunity => await dbContext.Opportunities.AnyAsync(x => x.Id == parentId, cancellationToken),
            EntityType.Case => await dbContext.Cases.AnyAsync(x => x.Id == parentId, cancellationToken),
            _ => false
        };

        if (!exists)
        {
            throw new InvalidOperationException("The referenced parent record does not exist.");
        }
    }

    public static void EnsureOpportunityStageTransition(OpportunityStage current, OpportunityStage next)
    {
        if (current is OpportunityStage.ClosedWon or OpportunityStage.ClosedLost && next != current)
        {
            throw new InvalidOperationException("Closed opportunities cannot move back into the active pipeline.");
        }
    }

    public static void EnsureLeadConvertible(Lead lead)
    {
        if (lead.Stage != LeadStage.Qualified)
        {
            throw new InvalidOperationException("Only qualified leads can be converted.");
        }

        if (lead.ConvertedAtUtc.HasValue)
        {
            throw new InvalidOperationException("This lead has already been converted.");
        }
    }

    public static void EnsureTaskCompletion(TaskItem entity)
    {
        if (entity.Status == Domain.TaskStatus.Completed && !entity.CompletedAtUtc.HasValue)
        {
            entity.CompletedAtUtc = DateTime.UtcNow;
        }
    }

    public static void EnsureCaseResolution(Case entity)
    {
        if (entity.Status is CaseStatus.Resolved or CaseStatus.Closed && !entity.ResolvedAtUtc.HasValue)
        {
            entity.ResolvedAtUtc = DateTime.UtcNow;
        }
    }
}
