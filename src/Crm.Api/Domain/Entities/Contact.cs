namespace Crm.Api.Domain.Entities;

public sealed class Contact : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }
    public required string OwnerName { get; set; }
    public Domain.ContactStatus Status { get; set; } = Domain.ContactStatus.Active;
    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
    public ICollection<Case> Cases { get; set; } = new List<Case>();
}
