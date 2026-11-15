Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15.

# Business Logic Outline for Scrips.Core

Scrips.Core holds almost no business logic. It holds contracts (DTOs, enums, protos, Refit interfaces), a few base classes, and the audit outbox. The rules below are the ones the code in THIS repo enforces or defines. Rules that live in a consumer service are out of scope. Each domain has a detail file (`02-section-*.md`).

## 1. What the library enforces itself [UPDATED]

| Rule | Where it is enforced | Strength |
|---|---|---|
| Every `SaveChanges` on an auditable context records a change list | `Scrips.BaseDbContext/AuditableBaseDbContext.cs:30-56, 63-87`; multi-tenant twin `AuditableMultiTenantBaseDbContext.cs:41-67, 74-98` | Code only. A consumer that does not inherit the base class gets no audit. [CONFIRMED] |
| In outbox mode the audit row and the data commit in one transaction | `AuditableBaseDbContext.cs:32-40` (the row is added to the same `DbContext` before `base.SaveChangesAsync`) | Atomic. A failure in staging fails the save. [CONFIRMED] |
| In legacy mode the audit publish is best effort | `AuditableBaseDbContext.cs:42-55` (try/catch, `Log.Warning`, save continues) | Not atomic. The publish runs before the data save, so a later save failure leaves an audit event for a change that did not commit. [CONFIRMED by reading order] |
| Audit rows are never audited | `AuditLoggingHelper.cs:17` (`x.Entity is not LogAudit`) | Excludes the `LogAudit` type only. The outbox row is added after `DetectChanges` has run (`AuditableBaseDbContext.cs:36-38`), so one save never audits its own outbox row. [CONFIRMED] |
| `[MaskValueAudit]` properties show `"*"` in audit values | `AuditLoggingHelper.cs:27, 53-54, 66, 70, 77-78` | Opt-in per property. [CONFIRMED] |
| A non-GUID `sub` claim never aborts the business write | `AuditLoggingHelper.cs:35-43` (`Guid.TryParse`, falls back to `UserRaw`) | [CONFIRMED] |
| `CurrentUser` is set once per scope | `Scrips.Infrastructure/Auth/CurrentUser.cs:39-60` (throws on a second call) | [CONFIRMED] |
| Entity ids are sequential GUIDs | `Scrips.Core.Domain/Common/Contracts/BaseEntity.cs:8` (`NewId.Next().ToGuid()`) | Applies to the non-generic `BaseEntity` only. `BaseEntity<TId>` leaves `Id` as `default`. [CONFIRMED] |
| `AuditableEntity` stamps `CreatedOn` and `UpdatedOn` at construction | `AuditableEntity.cs:14-18` | `CreatedBy` and `UpdatedBy` are public setters and nothing in this repo sets them. [CONFIRMED by `git grep`] |
| Paging defaults | `SpecificationBuilderExtensions.cs` `PaginateBy`: page less than 1 becomes 1, size less than 1 becomes 10 | `PaginationFilter.PageSize` defaults to `int.MaxValue` (`PaginationFilter.cs:7`). [CONFIRMED] |
| Exceptions map to HTTP status | `ExceptionMiddleware.cs:62-80`: `CustomException` uses its own code, `KeyNotFoundException` gives 404, anything else gives 500 | The middleware is defined but its registration (`AddExceptionMiddleware`) is commented out in `Startup.cs:22, 47`. [CONFIRMED] |

## 2. Domain areas [UPDATED]

Each area below is a contract surface. The state machines named here are defined by enums in this repo; the transition rules live in the owning service.

### 2.1 Appointment and scheduling [UPDATED]
Enums: `AppointmentStatus` (12 values), `AppointmentType` (FOLLOWUP, ROUTINE, WALKIN), `SlotStatus`, `ParticipationType`, `ParticipationStatus`, `ParticipantRequired`, `QuestionnaireStatus`, `AppointmentCancellationReason`, `EnumInvoiceStatus`. Contract growth since March: group sessions, joint (multi-practitioner) bookings, room change history, device slots, schedules per actor, resource availability. Detail: `02-section-1-appointments-scheduling.md`.

### 2.2 Patient management [UPDATED]
56 rpcs. Since March: care plans and goals, ambient recording consent and parts, encounter sign state, phone lookup, transcript corrections. Detail: `02-section-2-patient-management.md`. Clinical meaning of these fields is owned by the encounter owner session. [NEEDS HUMAN REVIEW]

### 2.3 Billing [UPDATED]
22 rpcs. One new rpc (`UpdateInvoiceProvider`) and resource fields on invoice requests. Detail: `02-section-3-billing-financial.md`.

### 2.4 Clinical documentation and AI [UPDATED]
`MasterManagement` now carries the AI rpcs (embedding, RAG search, suggestions, documentation) plus drug, diagnosis and order vocabularies, and an explainability message with a `clinicianReviewRequired` flag. Detail: `02-section-4-clinical-ai.md`. [NEEDS HUMAN REVIEW]

### 2.5 Provider, practice and organization [UPDATED]
Resources (devices) and actor presence were added to the practice contract. Detail: `02-section-5-provider-practice-org.md`.

### 2.6 Identity and tenancy [UPDATED]
Claim names, role constants and one permission family (Tenants). Detail: `02-section-6-identity-tenancy.md`.

### 2.7 Audit and compliance [UPDATED]
The audit outbox is new. Detail: `02-section-7-audit-compliance.md`.

### 2.8 Master data [UPDATED]
Detail: `02-section-8-master-data.md`.

### 2.9 Notifications and communication [UPDATED]
Detail: `02-section-9-notifications-communication.md`.

## 3. Documentation mapping [UPDATED]

| Outline area | Detail file |
|---|---|
| 2.1 | `02-section-1-appointments-scheduling.md` |
| 2.2 | `02-section-2-patient-management.md` |
| 2.3 | `02-section-3-billing-financial.md` |
| 2.4 | `02-section-4-clinical-ai.md` |
| 2.5 | `02-section-5-provider-practice-org.md` |
| 2.6 | `02-section-6-identity-tenancy.md` |
| 2.7 | `02-section-7-audit-compliance.md` |
| 2.8 | `02-section-8-master-data.md` |
| 2.9 | `02-section-9-notifications-communication.md` |

## 4. Verification checklist [UPDATED]

- [x] Project count, TFM, package versions re-read from csproj files.
- [x] Every rpc and Refit route counted from source.
- [x] Audit and outbox code read in full.
- [ ] No build or test was run (machine load; brief forbids it). [UNVERIFIED]
- [ ] Consumer behaviour was sampled by grep on default branches, not by running services.
