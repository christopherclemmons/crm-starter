namespace Crm.Api.Domain.Entities;

public sealed class TaskItem : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string OwnerName { get; set; }
    public DateOnly? DueDate { get; set; }
    public Domain.TaskStatus Status { get; set; } = Domain.TaskStatus.Open;
    public DateTime? CompletedAtUtc { get; set; }
    public Domain.EntityType ParentType { get; set; }
    public Guid ParentId { get; set; }
}
