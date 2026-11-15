Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-04-17 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Previous text was verified against cc112c6 (2026-04-15).

# Scrips.Core — Business Rules and Invariants

**Scope:** The rules that Scrips.Core enforces, or states, for the services that use it.
Each rule has a code location. Tags: `[CONFIRMED]` read in code this session, `[INFERRED]`, `[NEEDS HUMAN REVIEW]`. A rule that lives only in a comment is marked "comment". File paths are relative to the repo root.

---

## 1. Audit rules [UPDATED]

### R1. Every save on an auditable context records a change list [UPDATED]
`[CONFIRMED: Scrips.BaseDbContext/AuditableBaseDbContext.cs:30-56, 63-87; AuditableMultiTenantBaseDbContext.cs:41-67, 74-98]`
`SaveChanges` and `SaveChangesAsync` call `AuditLoggingHelper.DetectChanges`. What happens next depends on the switch `Audit:UseOutbox`:
- **Off (default):** publish the change list to Dapr topic `SaveAudit` on pubsub `pubsub`, then save the data. A publish failure does not stop the data save.
- **On:** add an outbox row to the same context, then save. The row and the data commit in one transaction. A failure to stage the row fails the save.
A service that does not inherit the base class gets no audit. [CONFIRMED by design]

### R2. Legacy publish: 3 attempts, waits of 1 s and 2 s, then log the payload [UPDATED]
`[CONFIRMED: AuditableBaseDbContext.cs:15, 89-116]`
The old text said the waits were 1 s, 2 s and 4 s. There are only two waits (after attempt 1 and after attempt 2). After the third failure the full payload is written to the error log. The legacy path publishes before the data save, so a data save that fails afterwards leaves an audit event for a change that did not commit. `[CONFIRMED by line order :44-55]`

### R3. The audit payload fields [UPDATED]
`[CONFIRMED: AuditLoggingHelper.cs:13-97; Scrips.Core/Models/Audit/LogAudit.cs:3-14; Entities/AuditEntityEntry.cs:32-44]`
Fields: Id, Timestamp, Tenant, User, Ip, Entity, Action (Added, Updated or Deleted), KeyValue, OldValue, NewValue. The user comes from claim `sub`; the tenant from claim `tenant`; the IP from request header `ipaddress`. If `sub` is not a GUID, the raw text is kept (`UserRaw`). If `sub` is missing, the user shows as an all-zero GUID. The timestamp is server local time (`DateTime.Now`), not UTC.

### R4. Audit records are not audited [UNCHANGED, re-verified]
`[CONFIRMED: AuditLoggingHelper.cs:16-17]` The filter excludes entities of type `LogAudit` only.

### R5. A property marked `[MaskValueAudit]` shows `"*"` in old and new values [UPDATED]
`[CONFIRMED: AuditLoggingHelper.cs:27, 53-54, 66, 70, 77-78; MaskValueAuditAttribute.cs]`
Masking is per property. Primary keys are never masked (`:58`). No use of the attribute exists inside this repo. Consumers use it: Patient, Integration.Malaffi and QuestionBank (3 of 20 clones checked). `[CONFIRMED by grep on default branches]` A PHI property without the attribute appears in clear text. `[INFERRED]`

### R6. Only real changes are recorded on update [UPDATED]
`[CONFIRMED: AuditLoggingHelper.cs:72-80]` A Modified property is recorded only if EF marks it modified and the old and new values differ.

### R7. Entries with no recorded change are dropped [UPDATED]
`[CONFIRMED: AuditLoggingHelper.cs:90-91]` An entry whose action type stays 0 is skipped.

### R25. In outbox mode the audit event and the data commit together [NEW]
`[CONFIRMED: AuditableBaseDbContext.cs:32-40; AuditableMultiTenantBaseDbContext.cs:43-51]` The outbox row carries the tenant id from the multi-tenant context.

### R26. Outbox rows are retried, then dead-lettered, then recovered or scrubbed [NEW]
`[CONFIRMED: OutboxDrainerService.cs:89-178, 254-265, 280-295; AuditOutboxOptions.cs]`
- A publish failure adds 1 to `Attempts`. At 5 the row becomes a dead letter and an error is logged.
- A worker that dies leaves a row Claimed; after 5 minutes it returns to Pending without using an attempt.
- Published rows are deleted after 7 days. Dead letters are never deleted.
- Once a day at 02:00 UTC, dead letters are re-queued (up to 5 recovery cycles).
- A row that exhausted all cycles keeps its metadata. Its payload is replaced by `[]` only if `Audit:ScrubTerminalPayload` is true. The default is false, and the option comment calls it a compliance decision. `[NEEDS HUMAN REVIEW]`

### R27. The audit event id is the CloudEvent id, so the consumer can drop duplicates [NEW]
`[CONFIRMED: OutboxDrainerService.cs:214-218]` Whether the consumer drops duplicates in its store is not verified here.

### R28. Non-audit events can use the same outbox [NEW]
`[CONFIRMED: OutboxMessage.cs:94-115]` `ForIntegrationEvent` rejects the topic `SaveAudit` and an empty topic. The stored JSON is published unchanged.

### R29. A bad `sub` claim must not abort a business write [NEW]
`[CONFIRMED: AuditLoggingHelper.cs:35-43]` Fixed on 2026-07-30 (commit 491c662). A service on an older Core pin does not have the fix. `CurrentUser.GetUserId()` still parses `sub` as a GUID and throws if it is not one (`Scrips.Infrastructure/Auth/CurrentUser.cs:15-18`). `[CONFIRMED]`

---

## 2. Entity and domain rules [UPDATED]

### R8. Entity ids [UPDATED]
`[CONFIRMED: Scrips.Core.Domain/Common/Contracts/BaseEntity.cs:6-17; AuditableEntity.cs:3-19]`
The non-generic `BaseEntity` sets `Id` to a sequential GUID (`NewId.Next().ToGuid()`) in its constructor. `AuditableEntity<T>` derives from `BaseEntity<T>`, not from `BaseEntity`, so it does not run that constructor and its `Id` starts as the default value. The old text said every entity gets a NewId; that holds only for classes that derive from the non-generic `BaseEntity`. Per-entity checks in consuming services were not done. `[NOT VERIFIED]`

### R9. `AuditableEntity` stamps `CreatedOn` and `UpdatedOn` at construction [UNCHANGED, re-verified]
`[CONFIRMED: AuditableEntity.cs:14-18]` Both are `DateTime.UtcNow`. `CreatedOn` has a private setter. The type also implements `ISoftDelete`.

### R10. `CreatedBy` and `UpdatedBy` are not set by the library [UNCHANGED, re-verified]
`[CONFIRMED: AuditableEntity.cs:9-12; no assignment in the repo by git grep]` `UpdatedOn` is also not refreshed on update by this repo.

### R11. Soft delete is a flag only [UNCHANGED, re-verified]
`[CONFIRMED: ISoftDelete.cs; no HasQueryFilter in the repo]` The library gives `IsArchived`. A service must add the query filter.

### R12. Entities can carry domain events; nothing here sends them [UPDATED]
`[CONFIRMED: BaseEntity.cs:16; Events/EntityCreatedEvent.cs, EntityUpdatedEvent.cs, EntityDeletedEvent.cs; git grep DomainEvents finds no dispatcher]` `IEventPublisher`, `IRepositoryWithEvents<T>` and the event-adding decorator (commented out in `Scrips.Infrastructure/Persistence/Startup.cs:75-90`) have no implementation in this repo.

---

## 3. Multi-tenancy rules [UPDATED]

### R13. Multi-tenant services use `AuditableMultiTenantBaseDbContext` [UPDATED]
`[CONFIRMED: AuditableMultiTenantBaseDbContext.cs:12-33]` The class extends Finbuckle `MultiTenantDbContext`. Finbuckle filters only entities that the service marks as multi-tenant; none is marked in this repo. `[INFERRED from Finbuckle design]` A service that picks `AuditableBaseDbContext` gets audit and no tenant handling.

### R14. The tenant claim name is `tenant` [UNCHANGED, re-verified]
`[CONFIRMED: Shared/Multitenancy/MultitenancyConstants.cs:5; Shared/Authorization/ScripsClaims.cs:5; AuditLoggingHelper.cs:21]`

### R15. Slot-search requests carry `OrganizationId` [UNCHANGED, re-verified]
`[CONFIRMED: Scrips.Core/Models/Scheduling/SlotsRequest.cs:51]` Nothing in this repo rejects an empty id. `[INFERRED]`

---

## 4. Authorization and claims rules [UPDATED]

### R16. Claim names live in `ScripsClaims` [UNCHANGED, re-verified]
`[CONFIRMED: ScripsClaims.cs:5-12]` `tenant`, `fullName`, `permission`, `image_url`, `ipAddress`, `exp`, `orgRoles`, `orgId`. The repo has no helper that reads `orgId` or `orgRoles`.

### R17. Permission names use `Permissions.{Resource}.{Action}` [UNCHANGED, re-verified]
`[CONFIRMED: ScripsPermissions.cs:37]`

### R18. The library defines three permissions [UNCHANGED, re-verified]
`[CONFIRMED: ScripsPermissions.cs:21-26]` View, Create and Update on `Tenants`, all root level. Roles: `Admin` and `Basic`.

### R19. `CurrentUser` is set once per scope [UPDATED]
`[CONFIRMED: Scrips.Infrastructure/Auth/CurrentUser.cs:39-60]` A second `SetCurrentUser` or `SetCurrentUserId` throws. `IsSSA()` is true for claim `sa` = `1` (`:34`).

---

## 5. Event topic rules [UPDATED]

### R20. Topic names [UPDATED]
`[CONFIRMED: Scrips.Core/Topics.cs:5-17; Scrips.BaseDbContext/Outbox/OutboxMessage.cs:119-124]`
`Topics.cs` holds 10 names. `SaveAudit` is not one of them; it is `OutboxTopics.SaveAudit` and a string literal in the legacy path. Other repos define their own topic classes (for example AppointmentScheduling and Patient). `Topics.DoctorCreated` has no reference in the 20 clones checked. `[CONFIRMED by grep]`

---

## 6. Contract rules [UPDATED]

### R21. HTTP contracts [UPDATED]
`[CONFIRMED: Scrips.Core/HttpApiClients]` 11 Refit interfaces, 57 routes. 55 of them take the bearer token as a method argument. There is no retry policy in the repo.

### R22. gRPC contracts [UPDATED]
`[CONFIRMED: Scrips.Core/Protos; Scrips.Core.csproj:24-32]` 10 services and 254 calls. The project file compiles 9 protos; `QuestionBankManagement.proto` is not in the list. Servers are in the services.

### R30. Rules written in proto comments (not enforced by this repo) [NEW]
`[CONFIRMED that the comments exist; the services enforce them or not]` `[NEEDS HUMAN REVIEW for every clinical line]`
- Ambient recording consent fails closed: any error means denied, and the default value 0 means denied (`PatientManagement.proto:100-105, 164-166`).
- Goal checks are rule-based, send codes only (never a value), fail closed ("no-data", never "met"), are capped and audited (`PatientManagement.proto:122-131`).
- A goal status set by a physician is not overwritten by a re-enrolment (`PatientManagement.proto:132-152`; commit ce1ac67).
- A phone lookup is exact, organization-scoped and returns ids only (`PatientManagement.proto:115-121`).
- An AI answer with `clinicianReviewRequired` unset must be read as "review required" (`MasterManagement.proto:183-184`). For the ambient SOAP the comment says it is always true (`:365-370`).
- An empty `assertion` means "not stated", never "absent" or "normal" (`Scrips.Core/Shared/Ambient/AmbientWireValues.cs:63-72`).
- `PreferredDisplay` on a coding is for display only and must never reach claims or HIE payloads (`Scrips.Core/Models/Patient/Coding.cs`, comment).
- `UpdateInvoiceProvider` changes only DRAFT invoices (`BillingManagement.proto:29-31`).

---

## 7. Deliberate library-level choices [UPDATED]

### R23. Sync-over-async in the legacy `SaveChanges()` [UPDATED]
`[CONFIRMED: AuditableBaseDbContext.cs:58-62, 79]` The inline comment says it is deliberate and approved in SND-331 and SND-386, and ties it to NABIDH audit-trail compliance. The code path applies only when the outbox is off. In outbox mode the sync save does not publish. `[NEEDS HUMAN REVIEW: compliance owner should state whether the outbox path meets the same requirement]`

### R24. Version pins are deliberate [UPDATED]
`[CONFIRMED: Scrips.BaseDbContext.csproj:7, 17, 23, 45]` Dapr.Client 1.17.0; `Microsoft.Extensions.*` at 10.0.2; `System.IO.Pipelines`, `System.Text.Json` and `System.Threading.Channels` at 8.x because 10.x breaks Kestrel `PipeWriter.Advance()` in gRPC (comment). Do not bump them without a gRPC test. All projects target `net8.0`.
