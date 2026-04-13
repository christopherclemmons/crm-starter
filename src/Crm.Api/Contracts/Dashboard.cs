namespace Crm.Api.Contracts;

public sealed record AccountOverviewResponse(
    Guid Id,
    string Name,
    string Status,
    IReadOnlyCollection<OpportunitySummary> OpenOpportunities,
    IReadOnlyCollection<ActivitySummary> RecentActivities,
    IReadOnlyCollection<CaseSummary> ActiveCases);

public sealed record OpportunitySummary(Guid Id, string Name, string Stage, decimal EstimatedValue, DateOnly? ExpectedCloseDate);
public sealed record ActivitySummary(Guid Id, string Summary, string Type, DateTime OccurredAtUtc);
public sealed record CaseSummary(Guid Id, string Subject, string Status, string Priority, DateTime OpenedAtUtc);
