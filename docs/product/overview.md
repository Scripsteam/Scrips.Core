Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-04-17 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Previous text was verified against cc112c6 (2026-04-15). 143 commits landed since.

# Scrips.Core — Overview

**Audience:** Clinical leads, product managers, compliance owners. No code assumed.

## What Scrips.Core is [UPDATED]

Scrips.Core is the shared foundation library that the Scrips backend services import. It is not an application. Nothing in this repo is deployed. Nothing in it answers a web request on its own. [CONFIRMED: no `Program.cs`, no Dockerfile, no routable controller; `git ls-files`]

It ships five things:

1. **Data contracts.** The exact shape of a patient, an appointment, an invoice, a provider and an organization, so that two services read the same fields the same way. 156 DTO files and 17 enums. [CONFIRMED: `Scrips.Core/Models`, `Scrips.Core/Enums`]
2. **Service contracts.** 11 HTTP client interfaces (57 routes) and 10 gRPC services (254 calls) in 11 proto files. The old text said 10 protos; one more service (QuestionBank) was added in September 2026. [CONFIRMED: `Scrips.Core/HttpApiClients`, `Scrips.Core/Protos`]
3. **Base database classes.** Two `DbContext` base classes record every entity change as an audit event. This part grew: a new audit "outbox" can store the audit event in the same database transaction as the change. [CONFIRMED: `Scrips.BaseDbContext/AuditableBaseDbContext.cs:30-56`, `Scrips.BaseDbContext/Outbox/`]
4. **A multi-tenant base class.** The multi-tenant `DbContext` extends Finbuckle's `MultiTenantDbContext`. Isolation applies only to entities that the consuming service marks as multi-tenant. [CONFIRMED: `AuditableMultiTenantBaseDbContext.cs:12`; no entity in this repo is marked]
5. **A shared claim vocabulary.** JWT claim names (`tenant`, `orgId`, `orgRoles`, `permission`, `sub` read by helper), a permission name format and a small `CurrentUser` helper. [CONFIRMED: `Shared/Authorization/ScripsClaims.cs`, `Scrips.Infrastructure/Auth/CurrentUser.cs`]

## Why the platform has it [UPDATED]

Without a shared library, each service would define its own patient shape, appointment shape, claim names and audit format. One library gives one source for cross-service data and one audit pipeline. [INFERRED from purpose; the code shows the shared contracts]

## Who depends on it [UPDATED]

At least 20 service repos embed Scrips.Core as a git submodule, each pinned to its own Core commit. On 2026-11-15 the pins were between 1 and 147 commits behind the library's master. [CONFIRMED: `.gitmodules` and submodule pointers on each default branch of the 20 repos cloned locally] The old text said "eleven services"; that was the count of HTTP client interfaces, not of users.
The services named by contracts: Appointment Scheduling, Patient, Billing, Practice Management, Provider, Organization, Identity, Persons, Master data, Question Bank, Notifications and the e-mail sender. [CONFIRMED: interface and proto names] Services that embed Core but have no contract of their own here include Auditing, Reporting, Workflows, Video, Inventory, EClaim and the NABIDH and Malaffi integrations. [CONFIRMED: `.gitmodules`]

## What breaks if Scrips.Core changes or disappears [UPDATED]

- **Audit events stop or change.** Services that inherit the base classes record their audit events there. [CONFIRMED]
- **Tenant isolation depends on each service.** The base class does not mark any entity. [CONFIRMED]
- **Cross-service calls lose their contracts.** The proto and client files live here. [CONFIRMED]
- **Claim names fragment.** `ScripsClaims` is the one place the platform lists the claim names. [CONFIRMED]
- **A change is not live until a service updates its pin.** A service on an old pin keeps the old behaviour. Four services that use the outbox are on pins that lack two later fixes (see `docs/cursor/08-microservices-topology.md`). [CONFIRMED]

## New since April 2026 [NEW]

- **Audit outbox.** Services can save audit events in the same transaction as the data, and a background worker sends them on. It is off by default and is turned on per service with a setting (`Audit:UseOutbox`). Ten of the 20 local service repos call the wiring method. Nine of them commit dev settings with the switch on (and the scrub option on); deployed values were not verified. Delivery is at-least-once, so the receiver must drop duplicates. [CONFIRMED: `AuditOutboxOptions.cs:10`; grep for `AddAuditOutbox`]
- **Care plan goals and goal checks.** New calls let the workflow service create care plans, add goals and record whether a goal is met, with the patient service as owner of the goal status. [CONFIRMED: `PatientManagement.proto:87-152`] [NEEDS HUMAN REVIEW: clinical meaning]
- **Ambient scribe contracts.** Calls for recording consent, transcript corrections, drug, diagnosis and order vocabularies, and AI answers that carry an explanation and a "clinician review required" flag. [CONFIRMED: `PatientManagement.proto:100-113`, `MasterManagement.proto:84-106, 139-146`] [NEEDS HUMAN REVIEW]
- **Devices and rooms in scheduling.** Resources (devices), device slots, room history, group sessions and joint bookings. [CONFIRMED: `Models/Practice/DeviceSlots.cs`, `AppointmentManagement.proto`, `PracticeManagement.proto`]
- **Encounter sign and question bank.** A new service contract lets the patient service freeze template answers at sign and read the narrative back. [CONFIRMED: `QuestionBankManagement.proto`]
- **Clinic services switches.** One call returns five per-organization switches, including `documentIntake`. [CONFIRMED: `OrganizationManagement.proto:166-181`]

## User journeys [UNCHANGED, re-verified]

Not applicable. Scrips.Core is a shared library with no end-user flow. End-user journeys live in the services and apps that use it. [CONFIRMED: no UI or routable endpoint in the repo]

## Related product docs [UNCHANGED, re-verified]

- `business-rules.md` — the rules Scrips.Core forces on the services that use it.
- `compliance-notes.md` — what the code can and cannot support for NABIDH, Malaffi and similar rules.
