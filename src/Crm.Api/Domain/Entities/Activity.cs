namespace Crm.Api.Domain.Entities;

public sealed class Activity : BaseEntity
{
    public Domain.ActivityType Type { get; set; }
    public required string Summary { get; set; }
    public string? Details { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public required string OwnerName { get; set; }
    public Domain.EntityType ParentType { get; set; }
    public Guid ParentId { get; set; }
}
