using Crm.Api.Domain;

namespace Crm.Api.Contracts;

public sealed record CaseRequest(
    string Subject,
    Guid AccountId,
    string OwnerName,
    string? Description,
    Guid? ContactId,
    CasePriority Priority,
    CaseStatus Status);

public sealed record CaseResponse(
    Guid Id,
    string Subject,
    Guid AccountId,
    string OwnerName,
    string? Description,
    Guid? ContactId,
    CasePriority Priority,
    CaseStatus Status,
    DateTime OpenedAtUtc,
    DateTime? ResolvedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
