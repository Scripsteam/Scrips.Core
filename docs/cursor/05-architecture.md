Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Confidence tags: [CONFIRMED] read in code this session, [INFERRED], [ASSUMED], [NEEDS HUMAN REVIEW].

# Architecture Documentation

## 1. System overview [UPDATED]

Scrips.Core is a class-library repo, not a deployable service. Backend repos embed it as a git submodule and add project references to the projects they need. [CONFIRMED: `.gitmodules` in 20 consumer clones; `CLAUDE.md` "How It's Consumed"] It gives them four things:

1. gRPC contracts (`.proto`) for cross-service calls, and DTOs and Refit interfaces for HTTP calls.
2. Base DbContexts that record an audit trail on every save, plus an optional transactional outbox.
3. Base entity, specification, exception and controller classes.
4. Claim names, a `CurrentUser`, and a few middleware classes.

Nothing in the repo starts a host: no `Program.cs`, no Dockerfile, no CI workflow, no test project. [CONFIRMED: `git ls-files`]

## 2. Tech stack [UPDATED]

| Concern | Choice | Evidence |
|---|---|---|
| Runtime | .NET 8 (`net8.0`) in all 6 projects. The old text said `net7.0`; that was wrong or already fixed. | each csproj line 4 |
| ORM | EF Core 8.0.11, SQL Server | `Scrips.BaseDbContext.csproj:21`; `Persistence/Startup.cs:23` |
| Multi-tenancy | Finbuckle.MultiTenant.EntityFrameworkCore 6.10.0 | `Scrips.BaseDbContext.csproj:43` |
| Messaging | Dapr.Client 1.17.0 (was 1.14.0 in March) | `Scrips.BaseDbContext.csproj:17` |
| RPC | Protobuf contracts; client stubs only (`GrpcServices="Client"`) | `Scrips.Core.csproj:24-32` |
| HTTP clients | Refit 8.0.0 | `Scrips.Core.csproj:20` |
| CQRS | MediatR 12.0.1 | `Scrips.Core.Application.csproj:17` |
| Validation, mapping, queries | FluentValidation 11.10.0, Mapster 7.4.0, Ardalis.Specification 8.0.0 | `Scrips.Core.Application.csproj:14-16` |
| Logging | Serilog 4.0.2 | `Scrips.BaseDbContext.csproj:42`, `Scrips.Infrastructure.csproj` |
| Pinned versions | `Microsoft.Extensions.*` and `System.Diagnostics.DiagnosticSource` at 10.0.2 to match Dapr 1.17.0; `System.IO.Pipelines`, `System.Text.Json`, `System.Threading.Channels` kept at 8.x because 10.x breaks Kestrel `PipeWriter.Advance()` in gRPC | comments at `Scrips.BaseDbContext.csproj:7, 23, 45` |
| IDs | NewId (MassTransit namespace) | `Scrips.Core.Domain.csproj:14`; `BaseEntity.cs:2, 8` |

## 3. Components [UPDATED]

### Scrips.Core (contracts) [UPDATED]
11 proto files, 11 Refit interfaces (57 routes), 17 enums, 156 DTO files, `Topics.cs`, `PagedResults<T>`, shared claim and notification types, `AmbientWireValues`. The csproj compiles 9 protos (`:24-32`). `QuestionBankManagement.proto` is not listed there and no `.csproj` references the 4 protos in the root `Protos/` folder, which use namespace `Scrips.Generated.Models`. A consumer that wants the QuestionBank client compiles that file itself; the proto header says Scrips.Patient does so. [CONFIRMED: csproj; `git grep` for the root Protos finds no reference] [NEEDS HUMAN REVIEW: root `Protos/` may be dead]

### Scrips.Core.Application [UPDATED]
Interfaces: `ICacheService`, `ICacheKeyService`, `IFileStorageService`, `IJobService`, `INotificationSender`, `ISerializerService`, `IEventPublisher`, `IRepository<T>` and friends, `IDapperRepository`, `ICurrentUser`. Models: `BaseFilter`, `Search`, `Filter`, `PaginationFilter`, `PaginationResponse<T>`. Specifications: `EntitiesByBaseFilterSpec`, `EntitiesByPaginationFilterSpec`, `AuditableEntitiesByCreatedOnBetweenSpec` plus keyword, advanced-search and advanced-filter builders. Exceptions: `CustomException` and Conflict, Forbidden, InternalServer, NotFound, Unauthorized. [CONFIRMED]
Only `ISerializerService` and the cache extension methods are used by code in this repo. [CONFIRMED]

### Scrips.BaseDbContext [UPDATED]
`AuditableBaseDbContext`, `AuditableMultiTenantBaseDbContext`, `AuditLoggingHelper`, `AuditEntityEntry`, `MaskValueAuditAttribute`, and the `Outbox/` folder (6 files, new). Detail in `02-section-7-audit-compliance.md`. It references Scrips.Core for `LogAudit`. [CONFIRMED: `Scrips.BaseDbContext.csproj:55`]

### Scrips.Infrastructure [UPDATED]
Mostly stubs. `AddInfrastructure` registers only API versioning (default 1.0, assume default when unspecified, report versions), `CurrentUser`, localization and `AddPersistence`. Every other registration line (caching, CORS, exception middleware, health check, mailing, multitenancy, notifications, OpenAPI, request logging, background jobs) is commented out. `AddPersistence` requires `DatabaseSettings:ConnectionString` or throws `InvalidOperationException`, then registers the two connection-string helpers. [CONFIRMED: `Startup.cs:13-38`, `Persistence/Startup.cs:13-40`] `ExceptionMiddleware`, `RequestLoggingMiddleware` and `ResponseLoggingMiddleware` exist but their `Add...`/`Use...` methods are `internal` and not called by `AddInfrastructure`, so a consumer cannot enable them through the public surface. [CONFIRMED: `Middleware/Startup.cs:7-37` are `internal static`] [NEEDS HUMAN REVIEW: whether consumers copy them instead]

### Scrips.WebApi [UNCHANGED, re-verified]
`BaseApiController` (MediatR `ISender`), `VersionedApiController` (`api/v{version:apiVersion}/[controller]`), `VersionNeutralApiController` (`api/[controller]`), `ScripsApiConventions`. [CONFIRMED: files re-read]

### Scrips.Core.Domain [UPDATED]
Contracts only. No concrete entity. [CONFIRMED]
- `BaseEntity` (non-generic) sets `Id = NewId.Next().ToGuid()` in its constructor; `BaseEntity<TId>` leaves `Id` unset and has `[NotMapped] List<DomainEvent> DomainEvents` (`BaseEntity.cs:6-17`).
- `AuditableEntity` and `AuditableEntity<T>` add `CreatedBy`, `CreatedOn` (private setter, set to `UtcNow` in the constructor), `UpdatedBy`, `UpdatedOn` (set to `UtcNow` in the constructor), `IsArchived`; they implement `IAuditableEntity` and `ISoftDelete` (`AuditableEntity.cs:3-19`).
- `AuditableEntity<T>` derives from `BaseEntity<T>`, not from `BaseEntity`. So `AuditableEntity<Guid>` does NOT get the `NewId` id from the `BaseEntity` constructor (`AuditableEntity.cs:7`; `BaseEntity.cs:8`). The non-generic `AuditableEntity : AuditableEntity<Guid>` has the same property. [CONFIRMED by reading both files] Any statement that "every `AuditableEntity` gets a NewId" is wrong. Whether a given consumer entity gets an id from elsewhere must be checked per entity. [NOT VERIFIED in consumers]
- `EntityCreatedEvent<T>`, `EntityUpdatedEvent<T>`, `EntityDeletedEvent<T>` are `DomainEvent`s with an internal constructor and a `WithEntity` factory. Nothing in this repo adds them to an entity or dispatches them. [CONFIRMED: `git grep DomainEvents` finds only the contract and a comment]

### Shared authorization [UPDATED]
See `02-section-6-identity-tenancy.md`.

### AI Chief Complaint [UPDATED]
See `02-section-4-clinical-ai.md`.

## 4. Critical data flow: an audited save [UPDATED]

```
service code: db.SaveChangesAsync()
  -> AuditableBaseDbContext.SaveChangesAsync            (AuditableBaseDbContext.cs:30)
       -> AuditLoggingHelper.DetectChanges              (AuditLoggingHelper.cs:13)
       -> if Audit:UseOutbox:                           (:32)
            add OutboxMessage row to the same context   (:38)
            base.SaveChangesAsync  -> ONE transaction: data + outbox row
            [later] OutboxDrainerService claims rows, publishes "SaveAudit" to Dapr "pubsub",
                    marks Published                     (OutboxDrainerService.cs:180-267)
         else:
            publish "SaveAudit" inline, 3 attempts      (:42-55, 89-116)
            base.SaveChangesAsync
  -> Scrips.Auditing subscribes to "SaveAudit" and stores LogAudit rows
```
[CONFIRMED for every arrow inside this repo; the Scrips.Auditing step is CONFIRMED at `API/Callback/AuditingServer.cs:46-49, 124-129` in that repo]

## 5. Healthcare compliance (code facts) [UPDATED]

- PHI: DTOs, protos and audit payloads carry it. No field-level marker exists beyond `[MaskValueAudit]` and comments. [CONFIRMED] Compliance meaning: [NEEDS HUMAN REVIEW]
- Audit: see section 7. In outbox mode the audit row is atomic with the data; in legacy mode it is best effort. [CONFIRMED]
- Encryption: no encryption code in this repo. [CONFIRMED: `git grep -n 'Encrypt\|Cipher\|KeyVault' -- '*.cs'` finds nothing; the only related items are `Azure.Identity` package references in `Scrips.Core.csproj:10` and `Scrips.BaseDbContext.csproj:16`, with no use in code] [NEEDS HUMAN REVIEW]
- Retention: outbox published rows are purged after 7 days by default; dead letters are kept; scrub is off by default. [CONFIRMED]
- HIE: no HL7, no NABIDH or Malaffi call. One data field (`NabidhAssigningAuthority`) and one comment. [CONFIRMED]

## 6. Services this library describes [UPDATED]

Refit and proto contracts name these services: Billing, Email sender, Identity, Master, Notifications, Organization, Patient, Person, Practice, Provider, Scheduling, QuestionBank. Detail and consumer evidence: `08-microservices-topology.md`.

## 7. Risks and technical debt [UPDATED]

The full list with severity is in `09-technical-debt-inventory.md`. Items that the old file listed and the code no longer supports:

### 7.1 Audit log loss on Dapr failure [UPDATED]
Legacy path unchanged: after 3 failed attempts the payload is logged and the save continues. The outbox path removes this loss for services that turn it on (10 of 20 clones call `AddAuditOutbox`; flag values per environment unknown). `AuditableBaseDbContext.cs:42-55, 112-115`.

### 7.2 Cache not implemented [UPDATED]
Still true for this repo (interface only). 7 consumer clones implement `ICacheService` themselves.

### 7.3 Connection string security [UPDATED]
`DatabaseSettings.ConnectionString` is a plain string (`DatabaseSettings.cs:6`). `IConnectionStringSecurer` exists. [CONFIRMED] No secret is stored in this repo. [CONFIRMED: `git grep -in 'server=\|password=\|pwd=' -- '*.cs'` finds nothing]

### 7.4 Multi-database support commented out [UNCHANGED, re-verified]
`Persistence/Startup.cs:23` hard-codes `mssql`; lines 42-94 are commented out.

### 7.5 No retry policy on Refit calls [UNCHANGED, re-verified]
No Polly package in any csproj. [CONFIRMED: `grep Polly */*.csproj` returns nothing]

### 7.6 MaskValueAudit not used in this repo [UPDATED]
Consumers use it in 3 of 20 clones sampled (Patient, Malaffi, QuestionBank).

### 7.7 .NET 7.0 end of support [REMOVED]
All six projects target `net8.0` (`*.csproj:4`). Stub: the old claim is gone.

### 7.8 Dual proto locations [UPDATED]
Root `Protos/` (4 files, namespace `Scrips.Generated.Models`) and `Scrips.Core/Protos/` (11 files, namespace `Scrips.Core.GrpcApiClients`). No project references the root folder. [CONFIRMED]

### 7.9 ISoftDelete without enforcement [UNCHANGED, re-verified]
No global query filter on `IsArchived` in this repo. [CONFIRMED: `git grep -n HasQueryFilter -- '*.cs'` returns nothing]

### 7.10 No domain entities [UNCHANGED, re-verified]
Confirmed above.
