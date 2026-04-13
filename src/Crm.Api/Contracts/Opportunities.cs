using Crm.Api.Domain;

namespace Crm.Api.Contracts;

public sealed record OpportunityRequest(
    string Name,
    Guid AccountId,
    string OwnerName,
    Guid? PrimaryContactId,
    decimal EstimatedValue,
    DateOnly? ExpectedCloseDate,
    OpportunityStage Stage);

public sealed record OpportunityResponse(
    Guid Id,
    string Name,
    Guid AccountId,
    string OwnerName,
    Guid? PrimaryContactId,
    decimal EstimatedValue,
    DateOnly? ExpectedCloseDate,
    OpportunityStage Stage,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
