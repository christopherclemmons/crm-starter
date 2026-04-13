using CrmTaskStatus = Crm.Api.Domain.TaskStatus;
using EntityType = Crm.Api.Domain.EntityType;

namespace Crm.Api.Contracts;

public sealed record TaskRequest(
    string Title,
    string OwnerName,
    EntityType ParentType,
    Guid ParentId,
    string? Description,
    DateOnly? DueDate,
    CrmTaskStatus Status);

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string OwnerName,
    EntityType ParentType,
    Guid ParentId,
    string? Description,
    DateOnly? DueDate,
    CrmTaskStatus Status,
    DateTime? CompletedAtUtc,
    bool IsOverdue,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
