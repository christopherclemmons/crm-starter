using Crm.Api.Domain;

namespace Crm.Api.Contracts;

public sealed record AccountRequest(
    string Name,
    string OwnerName,
    string? Industry,
    string? Website,
    string? PhoneNumber,
    string? BillingCity,
    string? BillingCountry,
    AccountStatus Status);

public sealed record AccountResponse(
    Guid Id,
    string Name,
    string OwnerName,
    string? Industry,
    string? Website,
    string? PhoneNumber,
    string? BillingCity,
    string? BillingCountry,
    AccountStatus Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
