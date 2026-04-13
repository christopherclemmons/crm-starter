namespace Crm.Api.Domain.Entities;

public sealed class Lead : BaseEntity
{
    public required string FullName { get; set; }
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Source { get; set; }
    public Domain.LeadStage Stage { get; set; } = Domain.LeadStage.New;
    public string? Notes { get; set; }
    public required string OwnerName { get; set; }
    public Guid? ConvertedAccountId { get; set; }
    public Guid? ConvertedContactId { get; set; }
    public Guid? ConvertedOpportunityId { get; set; }
    public DateTime? ConvertedAtUtc { get; set; }
}
