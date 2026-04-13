namespace Crm.Api.Domain.Entities;

public sealed class Case : BaseEntity
{
    public required string Subject { get; set; }
    public string? Description { get; set; }
    public Domain.CasePriority Priority { get; set; } = Domain.CasePriority.Medium;
    public Domain.CaseStatus Status { get; set; } = Domain.CaseStatus.New;
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }
    public required string OwnerName { get; set; }
    public DateTime OpenedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
}
