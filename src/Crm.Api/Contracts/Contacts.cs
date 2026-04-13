using Crm.Api.Domain;

namespace Crm.Api.Contracts;

public sealed record ContactRequest(
    string FirstName,
    string LastName,
    string OwnerName,
    string? Email,
    string? PhoneNumber,
    string? JobTitle,
    Guid? AccountId,
    ContactStatus Status);

public sealed record ContactResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string OwnerName,
    string? Email,
    string? PhoneNumber,
    string? JobTitle,
    Guid? AccountId,
    ContactStatus Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
