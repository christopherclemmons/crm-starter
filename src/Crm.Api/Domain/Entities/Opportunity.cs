namespace Crm.Api.Domain.Entities;

public sealed class Opportunity : BaseEntity
{
    public required string Name { get; set; }
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
    public Guid? PrimaryContactId { get; set; }
    public Contact? PrimaryContact { get; set; }
    public Domain.OpportunityStage Stage { get; set; } = Domain.OpportunityStage.Prospecting;
    public decimal EstimatedValue { get; set; }
    public DateOnly? ExpectedCloseDate { get; set; }
    public required string OwnerName { get; set; }
}
