using Crm.Api.Domain;

namespace Crm.Api.Contracts;

public sealed record ActivityRequest(
    ActivityType Type,
    string Summary,
    string OwnerName,
    EntityType ParentType,
    Guid ParentId,
    string? Details,
    DateTime? OccurredAtUtc);

public sealed record ActivityResponse(
    Guid Id,
    ActivityType Type,
    string Summary,
    string OwnerName,
    EntityType ParentType,
    Guid ParentId,
    string? Details,
    DateTime OccurredAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
