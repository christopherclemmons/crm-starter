# CRM Domain Requirements

## Purpose
The CRM system must help teams manage customer relationships, sales activity, and service interactions in one shared system.

## Core Entities
- Contact: An individual person associated with a customer or prospect
- Account: A company or organization associated with one or more contacts
- Lead: A potential customer not yet qualified
- Opportunity: A qualified sales deal with an estimated value and expected close date
- Activity: A logged interaction such as a call, email, meeting, or note
- Task: A follow-up action assigned to a user with a due date
- Case: A customer support issue or service request

## Contact Management
- Users must be able to create, view, update, and archive contacts
- Each contact must store basic details such as name, email, phone number, and job title
- A contact may belong to one account
- The system must keep a history of interactions for each contact

## Account Management
- Users must be able to create and manage accounts
- An account may have multiple related contacts
- An account must show open opportunities, recent activities, and active cases
- Users must be able to search accounts by name and status

## Lead Management
- Users must be able to create leads from manual entry or imported data
- Leads must move through defined stages such as New, Qualified, and Disqualified
- A qualified lead must be convertible into an account, contact, and opportunity
- The system must record the source of each lead

## Opportunity Management
- Users must be able to create opportunities tied to accounts and contacts
- Each opportunity must include stage, value, owner, and expected close date
- Users must be able to update opportunity stage over time
- The system must support a basic sales pipeline view

## Activity Tracking
- Users must be able to log calls, emails, meetings, and notes
- Activities must be linked to a contact, account, lead, or opportunity
- Activities must include date, owner, and summary
- Users must be able to view activity history in chronological order

## Task Management
- Users must be able to create tasks for follow-up work
- Tasks must have an owner, due date, and status
- Users must be able to mark tasks as complete
- The system should support reminders for overdue tasks

## Case Management
- Users must be able to create support cases for customers
- Each case 