namespace Crm.Api.Domain;

public enum AccountStatus
{
    Active = 1,
    Archived = 2
}

public enum ContactStatus
{
    Active = 1,
    Archived = 2
}

public enum LeadStage
{
    New = 1,
    Qualified = 2,
    Disqualified = 3,
    Converted = 4
}

public enum OpportunityStage
{
    Prospecting = 1,
    Qualification = 2,
    Proposal = 3,
    Negotiation = 4,
    ClosedWon = 5,
    ClosedLost = 6
}

public enum ActivityType
{
    Call = 1,
    Email = 2,
    Meeting = 3,
    Note = 4
}

public enum TaskStatus
{
    Open = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}

public enum CasePriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum CaseStatus
{
    New = 1,
    InProgress = 2,
    WaitingOnCustomer = 3,
    Resolved = 4,
    Closed = 5
}

public enum EntityType
{
    Account = 1,
    Contact = 2,
    Lead = 3,
    Opportunity = 4,
    Case = 5
}
