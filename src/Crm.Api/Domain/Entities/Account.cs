namespace Crm.Api.Domain.Entities;

public sealed class Account : BaseEntity
{
    public required string Name { get; set; }
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingCountry { get; set; }
    public required string OwnerName { get; set; }
    public Domain.AccountStatus Status { get; set; } = Domain.AccountStatus.Active;
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
    public ICollection<Case> Cases { get; set; } = new List<Case>();
}
