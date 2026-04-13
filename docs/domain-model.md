# CRM Domain Model

## 1. Domain Summary
This CRM manages sales and service work around customers, prospects, and the interactions teams have with them. The system centers on accounts, contacts, leads, opportunities, activities, tasks, and support cases, with clear ownership and lifecycle rules so sales and service teams can share a consistent view of customer history.

## 2. Assumptions
- The CRM is single-tenant for the initial implementation.
- Authentication is out of scope for this first backend slice, so owner fields are stored as simple identifiers or names.
- A contact belongs to at most one account.
- An opportunity belongs to exactly one account and may optionally reference a primary contact.
- Activities can be attached to one primary parent record at a time: account, contact, lead, opportunity, or case.
- Support cases use a simple lifecycle of `New`, `InProgress`, `WaitingOnCustomer`, `Resolved`, and `Closed`.
- Archiving is implemented as a soft state flag rather than physical deletion for customer-facing records.
- The truncated case requirements imply standard service case behavior with ownership, priority, and resolution tracking.

## 3. Actors and Roles
- Sales Representative: manages leads, contacts, activities, tasks, and opportunities.
- Sales Manager: reviews opportunity pipeline and account progress.
- Support Agent: creates and updates support cases and related activities/tasks.
- Service Manager: monitors active cases and work distribution.
- System Administrator: manages reference data and operational health.

## 4. Core Entities

### Account
- Purpose: company or organization associated with contacts, opportunities, and cases.
- Owner: sales or service team member.
- Important attributes: name, industry, website, phone, status, billing city, billing country, owner name, timestamps.
- Lifecycle notes: `Active` or `Archived`.
- Scope: tenant-scoped in future, single-tenant for now.

### Contact
- Purpose: individual tied to an account or maintained independently.
- Owner: sales or service team member.
- Important attributes: first name, last name, email, phone, job title, account reference, status, timestamps.
- Lifecycle notes: `Active` or `Archived`.
- Scope: tenant-scoped in future, single-tenant for now.

### Lead
- Purpose: potential customer not yet qualified.
- Owner: sales team member.
- Important attributes: full name, company name, email, phone, source, stage, notes, owner name, timestamps.
- Lifecycle notes: `New`, `Qualified`, `Disqualified`, `Converted`.
- Scope: tenant-scoped in future, single-tenant for now.

### Opportunity
- Purpose: qualified sales deal associated with an account.
- Owner: sales team member.
- Important attributes: name, account reference, primary contact reference, stage, estimated value, expected close date, owner name, timestamps.
- Lifecycle notes: `Prospecting`, `Qualification`, `Proposal`, `Negotiation`, `ClosedWon`, `ClosedLost`.
- Scope: tenant-scoped in future, single-tenant for now.

### Activity
- Purpose: immutable log of a call, email, meeting, or note against a CRM record.
- Owner: user who logged it.
- Important attributes: type, summary, details, occurred at, owner name, parent reference, timestamps.
- Lifecycle notes: created and retained as historical record.
- Scope: tenant-scoped in future, single-tenant for now.

### Task
- Purpose: actionable follow-up item for sales or service work.
- Owner: assigned user.
- Important attributes: title, description, due date, status, owner name, parent reference, completed at, timestamps.
- Lifecycle notes: `Open`, `InProgress`, `Completed`, `Cancelled`, `Overdue` as computed condition.
- Scope: tenant-scoped in future, single-tenant for now.

### Case
- Purpose: support issue or service request for an account/contact.
- Owner: support agent.
- Important attributes: subject, description, priority, status, account reference, contact reference, owner name, opened at, resolved at, timestamps.
- Lifecycle notes: `New`, `InProgress`, `WaitingOnCustomer`, `Resolved`, `Closed`.
- Scope: tenant-scoped in future, single-tenant for now.

## 5. Value Objects
- `PersonName`: first name, last name, and display name semantics for contacts and leads.
- `ContactInfo`: email and phone fields where validation rules apply.
- `AddressSummary`: minimal billing city and country on accounts.
- `EntityReference`: polymorphic linkage target for activities and tasks.

## 6. Relationships
- Account `1 -> many` Contact. Contact account is optional; account owns the relationship.
- Account `1 -> many` Opportunity. Opportunity requires an account.
- Contact `0..1 -> many` Opportunity. Opportunity may reference one primary contact.
- Account `1 -> many` Case. Case requires an account.
- Contact `0..1 -> many` Case. Case may reference a contact.
- Lead `1 -> many` Activity and Task through polymorphic linkage.
- Opportunity `1 -> many` Activity and Task through polymorphic linkage.
- Account `1 -> many` Activity and Task through polymorphic linkage.
- Contact `1 -> many` Activity and Task through polymorphic linkage.
- Case `1 -> many` Activity and Task through polymorphic linkage.

## 7. Business Rules and Invariants
- Account names must be unique.
- Contact email must be unique when present.
- A contact can belong to at most one account.
- An opportunity must reference an existing account.
- Closed opportunities cannot move back to active pipeline stages.
- A qualified lead can be converted only once.
- Tasks marked `Completed` must record `CompletedAtUtc`.
- Cases marked `Resolved` or `Closed` must record `ResolvedAtUtc` once resolution occurs.
- Activities are append-only after creation, except for correcting summary/details metadata if needed.
- Activities and tasks must reference exactly one parent record type.

## 8. Lifecycle States
- Account: `Active -> Archived`
- Contact: `Active -> Archived`
- Lead: `New -> Qualified -> Converted`; `New/Qualified -> Disqualified`
- Opportunity: `Prospecting -> Qualification -> Proposal -> Negotiation -> ClosedWon/ClosedLost`
- Task: `Open -> InProgress -> Completed`; `Open/InProgress -> Cancelled`
- Case: `New -> InProgress -> WaitingOnCustomer -> Resolved -> Closed`

## 9. Multi-Tenancy and Boundaries
- Initial implementation is single-tenant.
- All CRM records should keep schema design compatible with a future tenant key.
- No cross-boundary data sharing rules are needed yet, but account/contact/opportunity/case ownership should remain explicit so tenant scoping can be added cleanly.

## 10. Audit / Compliance Considerations
- Activities are part of the customer interaction audit trail and should be retained.
- Lead conversion should capture converted account, contact, and opportunity references.
- Case state changes and resolutions should remain traceable.
- Contact and lead records contain PII, so logging should avoid raw emails/phones in operational logs.

## 11. Implementation Handoff Notes
- Backend should expose CRUD endpoints for accounts, contacts, leads, opportunities, tasks, and cases, plus activity logging and lead conversion.
- Database should enforce uniqueness for account name and nullable unique contact email.
- Activities and tasks should use explicit parent type and parent id fields instead of many nullable FKs for the first iteration.
- Soft-archive behavior should be implemented for accounts and contacts through status enums.
- Automatic migration application on startup is acceptable for local Docker development.
