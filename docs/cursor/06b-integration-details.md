Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Confidence tags: [CONFIRMED] read in code this session, [INFERRED], [ASSUMED], [NEEDS HUMAN REVIEW].
The old file (572 lines) was replaced. Per-service route lists moved to the `02-section-*` files.

# Integration Details

## Classification [UPDATED]

| Tier | Integrations |
|---|---|
| Critical (code here runs it) | 1 Dapr pub/sub audit lane, 2 SQL Server outbox, 3 Dapr integration-event lane |
| Contract (this repo only defines) | 4 gRPC services, 5 Refit clients, 6 Azure AI Search index class |
| Data field only | 7 NABIDH assigning authority |

## 1. Dapr pub/sub, audit lane [UPDATED]

- Producer: `AuditableBaseDbContext` and `AuditableMultiTenantBaseDbContext`, legacy path: `_daprClient.PublishEventAsync("pubsub", "SaveAudit", changes)` with up to 3 attempts, waiting 1 s then 2 s (`AuditableBaseDbContext.cs:93-109`; multi-tenant twin `:104-121`). After the last failure it logs the entire payload as JSON at Error level (`:112-115`). [CONFIRMED]
- Payload: `List<LogAudit>`. Fields: Id, Timestamp, Tenant, User, Ip, Entity, Action, KeyValue, OldValue, NewValue (`Models/Audit/LogAudit.cs:3-14`). The payload carries PHI when entities do. [CONFIRMED]
- Consumer: Scrips.Auditing, `TopicSubscription { PubsubName = "pubsub", Topic = "SaveAudit" }` and `CreateAuditLogCommand { LogAudits, EventId }` (`API/Callback/AuditingServer.cs:46-49, 124-129` in that repo). [CONFIRMED]
- Failure behaviour: legacy path never blocks the business save; outbox path blocks it only if staging the row fails. [CONFIRMED]

## 2. SQL Server outbox [NEW]

- Table `OutboxMessages` mapped by `OutboxModelConfig.Apply` from `OnModelCreating` in both bases (`AuditableBaseDbContext.cs:24-28`; `AuditableMultiTenantBaseDbContext.cs:35-39`).
- All reads and writes by the drainer are raw ADO.NET with `ROWLOCK`, `READPAST`, `OUTPUT` and `DELETE TOP`; they run on SQL Server only. `SET TRANSACTION ISOLATION LEVEL READ COMMITTED` is issued first on every iteration because a pooled session can carry another level and `READPAST` then fails with error 650 (commit 1ba4c4f; `OutboxDrainerService.cs:60, 79-86`). [CONFIRMED]
- Multi-pod behaviour: no leader election; pods claim disjoint row sets with `READPAST`, so two pods do not claim the same row at once. Delivery is still at-least-once: a pod that stops after a successful publish and before `MarkPublishedAsync` leaves the row Claimed, the reaper returns it to Pending, and it is published again (`OutboxDrainerService.cs:238-248, 269-275`). The consumer must deduplicate on the CloudEvent id. A claimed row held by a crashed pod returns to Pending after `VisibilityTimeoutMinutes`. [CONFIRMED: `:89-101, 187-191`]
- Ordering: rows are claimed by status, not by `Id`. The claim has no `ORDER BY`, so SQL Server does not guarantee drain order. `Id` is described as "drain order" in a comment (`OutboxMessage.cs:15`). [CONFIRMED by reading; INFERRED that order is not guaranteed] No consumer in this repo depends on order.
- Shutdown: a cancelled publish leaves the row Claimed for the reaper and does not count as a failure (`:250-253`). [CONFIRMED]
- Schema script: not in this repo (see `02-section-7-audit-compliance.md`).
- Per-service isolation: the recovery pass re-publishes only; it never writes the audit database directly (comment `:142-145`). [CONFIRMED]

## 3. Dapr integration-event lane [NEW]

- `OutboxMessage.ForIntegrationEvent(topic, payload, tenantId)`: for non-audit events, so the event commits with the business change. It rejects an empty topic and `SaveAudit` (`OutboxMessage.cs:94-111`).
- The drainer publishes these rows as the stored JSON, unchanged, with the same CloudEvent id header (`OutboxDrainerService.cs:243-247`).
- A producer must store the payload already in its final wire format; the helper uses `JsonSerializerDefaults.Web` (camelCase) to match `DaprClient`'s default. [CONFIRMED: `OutboxMessage.cs:115`]
- Measured use: Scrips.Patient (11 files mention `OutboxMessage.ForIntegrationEvent` on origin/master). [CONFIRMED by grep]

## 4. gRPC services [UPDATED]

- 10 services, 254 rpcs. `Scrips.Core.csproj` declares `GrpcServices="Client"` for 9 protos. The tenth (`QuestionBankManagement.proto`) is not listed, so `dotnet build` of this repo does not generate its client. [CONFIRMED: `Scrips.Core.csproj:24-32`]
- `Common.proto` (package `Common`) defines `CountUserByIdRequest/Response`, `Id`, `Ids`. It is imported by Patient, Person, Practice and Provider protos. It is not listed as its own `Protobuf` item; it is found through `import "Common.proto"`. [CONFIRMED: `git grep import`] How the import path resolves for consumers is not verified.
- `OrganizationManagement.proto` has no `package` and its service is `OrganizationConfiguration`. Person's service is `PersonManagment`. [CONFIRMED]
- Two services define the same rpc name `CountTotalNumberOfUsersById` (Patient and Practice). They use the shared `Common` messages. [CONFIRMED: `PatientManagement.proto`, `PracticeManagement.proto`]
- Contract rules stated in comments (fail closed, codes only, organization from caller, idempotent): see sections 2, 4 and 5 of the domain files. [CONFIRMED comments]

## 5. Refit clients [UPDATED]

- 11 interfaces, 57 routes. 55 routes take the bearer token as a method parameter. No base address, no retry. [CONFIRMED]
- `[Header("Authorization")] string token` means each caller must supply the whole header value ("Bearer ..."); the interface does not add it. [INFERRED from the signature]
- `PagedResults<T>` (PageNumber, PageSize, TotalNumberOfPages, TotalNumberOfRecords, Results) is the paged response type in `Scrips.Core` (`PagedResults.cs`). Application layer has a second one, `PaginationResponse<T>`. [CONFIRMED]

## 6. Azure AI Search index class [UPDATED]

`ChiefComplaintDocument` with attributes for key, searchable fields and a 1536-dimension vector field (`ChiefComplaintDocument.cs:8-40`). No code here creates, fills or queries an index. [CONFIRMED]

## 7. NABIDH data field [UPDATED]

`PracticeSetupDetails.NabidhAssigningAuthority`, `PracticeResponse.NabidhAssigningAuthority`. [CONFIRMED] No HL7 or HIE code. HIE rules are out of scope for this repo. [NEEDS HUMAN REVIEW if a consumer relies on these fields]

## Removed items [REMOVED]

The old file had sections for SNOMED CT as an integration and for a "Master data (lookup endpoints)" integration. SNOMED CT: no SNOMED package or lookup code exists in this repo; the word appears only in DTO types `AssessmentPlanSnomed` and `SnomedCode` in `Models/Billing/InvoiceDto.cs:84, 226-232` (`git grep -in snomed -- '*.cs'`). The stub stays here as evidence that no integration exists. Lookup endpoints: moved to `02-section-8-master-data.md`.
