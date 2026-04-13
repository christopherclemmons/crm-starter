using Crm.Api.Domain;

namespace Crm.Api.Contracts;

public sealed record LeadRequest(
    string FullName,
    string OwnerName,
    string Source,
    string? CompanyName,
    string? Email,
    string? PhoneNumber,
    string? Notes,
    LeadStage Stage);

public sealed record LeadResponse(
    Guid Id,
    string FullName,
    string OwnerName,
    string Source,
    string? CompanyName,
    string? Email,
    string? PhoneNumber,
    string? Notes,
    LeadStage Stage,
    Guid? ConvertedAccountId,
    Guid? ConvertedContactId,
    Guid? ConvertedOpportunityId,
    DateTime? ConvertedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record ConvertLeadRequest(
    string AccountName,
    string OpportunityName,
    decimal EstimatedValue,
    DateOnly? ExpectedCloseDate);
