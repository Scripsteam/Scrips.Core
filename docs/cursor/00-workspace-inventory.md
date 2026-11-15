Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Confidence tags: [CONFIRMED] read in code this session, [INFERRED], [ASSUMED], [NEEDS HUMAN REVIEW].

# Workspace Inventory for Scrips.Core

## 1. Project summary [UPDATED]

- Scrips.Core is a shared library, not a service. It has no `Program.cs`, no port, no database and no controller routes of its own. [CONFIRMED: `git ls-files` shows only class-library projects; only base controller classes in `Scrips.WebApi/`]
- 6 projects, all `net8.0`. [CONFIRMED: each csproj line 4 (`<TargetFramework>net8.0</TargetFramework>`)]
- 282 `.cs` files (old text said about 274). By project: Scrips.Core 199, Scrips.Core.Application 42, Scrips.BaseDbContext 11, Scrips.Infrastructure 15, Scrips.Core.Domain 11, Scrips.WebApi 4. [CONFIRMED: `git ls-files '*.cs'`]
- 11 `.proto` files in `Scrips.Core/Protos/` (10 services plus `Common.proto`) and 4 more in root `Protos/`. [CONFIRMED]
- No test project, no CI workflow, no Dockerfile in the repo. [CONFIRMED: `git ls-files` has no path with `test`, `.github`, `Dockerfile`]
- Two solution files: `Scrips.Core.sln` (legacy, Scrips.Core only) and `Scrips.Core.All.sln` (all projects). [INFERRED from `CLAUDE.md`; both files exist, contents not re-read]
- Consumed as a git submodule by at least 20 service repos (see 08-microservices-topology.md). [CONFIRMED: `.gitmodules` on each repo's default branch]

### Projects [UPDATED]

| Project | What it holds | Key evidence |
|---|---|---|
| `Scrips.Core` | DTOs (156 files in `Models/`), 17 enums, 11 Refit interfaces, 11 proto files, `Topics.cs`, `Shared/` helpers | `Scrips.Core/Models`, `Scrips.Core/Enums`, `Scrips.Core/HttpApiClients`, `Scrips.Core/Protos` |
| `Scrips.Core.Domain` | `BaseEntity`, `AuditableEntity`, `DomainEvent`, entity events, `ISoftDelete`, `IAggregateRoot`, `FileType` | `Scrips.Core.Domain/Common/Contracts/*.cs` |
| `Scrips.Core.Application` | Interfaces (cache, file storage, jobs, notifications, repository), filters, specifications, exceptions, validators | `Scrips.Core.Application/Common/*` |
| `Scrips.BaseDbContext` | Two auditable DbContext bases, audit helper, `[MaskValueAudit]`, the audit outbox (new) | `Scrips.BaseDbContext/*.cs`, `Scrips.BaseDbContext/Outbox/*.cs` |
| `Scrips.Infrastructure` | `CurrentUser` and its middleware, exception and request/response logging middleware, API versioning, connection-string helpers | `Scrips.Infrastructure/Startup.cs:13-38` |
| `Scrips.WebApi` | `BaseApiController`, versioned and version-neutral controller bases, API conventions | `Scrips.WebApi/*.cs` |

## 2. Entry points [UPDATED]

### Controllers [UPDATED]
None are routable from this repo. Three abstract bases exist. [CONFIRMED]

| File | Class | Purpose |
|---|---|---|
| `Scrips.WebApi/BaseApiController.cs:7-13` | `BaseApiController` | `[ApiController]` base; resolves `ISender` (MediatR) lazily |
| `Scrips.WebApi/VersionedApiController.cs:5-8` | `VersionedApiController` | Route `api/v{version:apiVersion}/[controller]` |
| `Scrips.WebApi/VersionNeutralApiController.cs:6-9` | `VersionNeutralApiController` | Route `api/[controller]`, `[ApiVersionNeutral]` |

### Public wiring methods [UPDATED]
- `AddInfrastructure(IConfiguration)` and `UseInfrastructure()` register API versioning, `CurrentUser`, localization, and the two connection-string helpers. Most other registrations are commented out. [CONFIRMED: `Scrips.Infrastructure/Startup.cs:13-58`]
- `AddAuditOutbox<TContext>(IConfiguration)` wires the audit outbox. [CONFIRMED: `Scrips.BaseDbContext/Outbox/AuditOutboxServiceCollectionExtensions.cs:16-33`]

### Background workers [NEW]
- `OutboxDrainerService<TContext>` (a `BackgroundService`) and `OutboxStartupValidator<TContext>` (an `IHostedService`). Both start only when `Audit:UseOutbox` is true. [CONFIRMED: `AuditOutboxServiceCollectionExtensions.cs:24-30`; `OutboxDrainerService.cs:23`; `OutboxStartupValidator.cs:19`]

### Interfaces without an implementation in this repo [UPDATED]
`IEventPublisher`, `IJobService`, `ICacheService`, `ICacheKeyService`, `IFileStorageService`, `INotificationSender`, `ISerializerService`, `IAuditService`, `IDapperRepository`, `IRepository<T>`, `IReadRepository<T>`, `IRepositoryWithEvents<T>`. A `git grep` for usages inside this repo finds only `ISerializerService` (used by `ExceptionMiddleware`) and the cache extension methods. [CONFIRMED]
Implementations live in consumer repos. Example: 7 consumer repos have a class that implements `ICacheService` (Billing, Organization, Patient, Persons, PracticeManagement, VideoApplication, Workflows). [CONFIRMED: grep on default branches, 2026-11-15]

### Dapr topics and subscriptions [UPDATED]
- This repo publishes one topic itself: `SaveAudit` on pubsub component `pubsub`. [CONFIRMED: `AuditableBaseDbContext.cs:97`; `OutboxMessage.cs:123`; `AuditOutboxOptions.cs:12`]
- `Topics.cs` holds 10 name constants and no publisher or subscriber. [CONFIRMED: `Scrips.Core/Topics.cs:5-17`]
- Scrips.Auditing subscribes to `SaveAudit` on `pubsub`. [CONFIRMED: Scrips.Auditing `API/Callback/AuditingServer.cs:46-49` on origin/master]

## 3. Domain areas [UPDATED]

| Area | DTO files | Enums | Refit interface | gRPC service (rpcs) |
|---|---|---|---|---|
| Scheduling | 40 in `Models/Scheduling` | 9 in `Enums/Scheduling` | `ISchedulingApi` (5) | `AppointmentManagement` (26) |
| Patient | 20 in `Models/Patient` | 2 in `Enums/Patient` | `IPatientApi` (5) | `PatientManagement` (56) |
| Billing | 22 in `Models/Billing` | 6 in `Enums/Billing` | `IBillingApi` (7) | `BillingManagement` (22) |
| Practice | 34 in `Models/Practice` | none | `IPracticeApi` (18) | `PracticeManagement` (51) |
| Provider | 12 in `Models/Provider` | none | `IProviderApi` (5) | `ProviderManagement` (5) |
| Organization | 4 in `Models/Organization` | none | `IOrganizationApi` (4) | `OrganizationConfiguration` (14) |
| Identity | 4 in `Models/Identity` | none | `IIdentityApi` (2) | `IdentityManagement` (21) |
| Person | 1 in `Models/Person` | none | `IPersonApi` (1) | `PersonManagment` (3; the service name is misspelled in the proto) |
| Master data and clinical AI | 16 in `Models/AIChiefComplaint` | none | `IMasterApi` (8) | `MasterManagement` (52) |
| Question bank | none | none | none | `QuestionBankManagement` (4) [NEW] |
| Audit | 1 (`LogAudit`) | none | none | none |
| Email and notifications | 1 (`EmailData`) | none | `IEmailSenderApi` (1), `INotificationsApi` (1) | none |

Totals: 57 Refit routes, 254 rpc definitions. [CONFIRMED: counted with `grep -cE '^\s*(\[(Get|Post|Put|Delete|Patch)\(|rpc )'`]. Old text said 9 gRPC protos; there are now 10 services (9 in the csproj, plus QuestionBank, which the csproj does not compile; see 05-architecture.md).

## 4. External dependencies [UPDATED]

### Healthcare integrations [UPDATED]
- NABIDH: only the data field `NabidhAssigningAuthority` (`Scrips.Core/Models/Practice/PracticeSetupDetails.cs:18`, `Scrips.Core/Models/Scheduling/PracticeResponse.cs:60`) and a doc comment that says a display field must never reach NABIDH/Malaffi payloads (`Scrips.Core/Models/Patient/Coding.cs:15`). No HL7 code. [CONFIRMED]
- Malaffi: named only in that same comment. [CONFIRMED: `git grep -in malaffi -- '*.cs'` returns only `Coding.cs`]

### Database [UPDATED]
- SQL Server only. `AddPersistence` requires `DatabaseSettings:ConnectionString` and hard-codes `dbProvider = "mssql"`. The multi-provider `UseDatabase` code is commented out. [CONFIRMED: `Scrips.Infrastructure/Persistence/Startup.cs:13-25, 42-94`] The old text said "SQL Server, PostgreSQL or MySQL"; that was wrong.
- The audit outbox uses raw ADO.NET with `Microsoft.Data.SqlClient` and SQL Server hints (`ROWLOCK`, `READPAST`). [CONFIRMED: `OutboxDrainerService.cs:93, 150, 188`]
- Multi-tenancy: `Finbuckle.MultiTenant.EntityFrameworkCore` 6.10.0. [CONFIRMED: `Scrips.BaseDbContext.csproj:43`]

### Messaging [UPDATED]
Dapr pub/sub via `Dapr.Client` 1.17.0 (old text said 1.14.0). [CONFIRMED: `Scrips.BaseDbContext.csproj:17`]

### Cache, jobs, file storage [UPDATED]
Interfaces only. `IJobService` is shaped like Hangfire (`Enqueue`, `Schedule`, `Delete`, `Requeue`) but no Hangfire package is referenced in any csproj in this repo. [CONFIRMED]

### Search and AI [UPDATED]
- `Azure.Search.Documents` 11.6.0 is referenced by `Scrips.Core.csproj:11`, used only for index attributes on `ChiefComplaintDocument` (1536-dimension vector field, profile `cc-vector-profile`). [CONFIRMED: `Scrips.Core/Models/AIChiefComplaint/ChiefComplaintDocument.cs:6-40`]
- The AI calls themselves are rpcs on `MasterManagement` (`GenerateEmbedding`, `SearchAndEnhance`, `GetClinicalSuggestions`, `GenerateDocumentation`) and four Refit routes on `IMasterApi`. [CONFIRMED]

### Key packages [UPDATED]

| Package | Version | Where |
|---|---|---|
| MediatR | 12.0.1 | `Scrips.Core.Application.csproj`, `Scrips.WebApi.csproj` |
| EF Core (+Relational) | 8.0.11 | `Scrips.BaseDbContext.csproj` |
| FluentValidation | 11.10.0 | `Scrips.Core.Application.csproj` |
| Mapster | 7.4.0 | `Scrips.Core.Application.csproj` |
| Ardalis.Specification | 8.0.0 | `Scrips.Core.Application.csproj` |
| Dapr.Client | 1.17.0 | `Scrips.BaseDbContext.csproj` |
| Finbuckle.MultiTenant.EntityFrameworkCore | 6.10.0 | `Scrips.BaseDbContext.csproj` |
| Google.Protobuf | 3.33.5 | `Scrips.BaseDbContext.csproj` |
| Refit | 8.0.0 | `Scrips.Core.csproj` |
| NewId | 4.0.1 | `Scrips.Core.Domain.csproj` |
| Serilog | 4.0.2 | `Scrips.BaseDbContext.csproj`, `Scrips.Infrastructure.csproj` |
| Asp.Versioning.Mvc | 8.1.0 | `Scrips.Infrastructure.csproj`, `Scrips.WebApi.csproj` |
| Newtonsoft.Json | 13.0.4 | all projects |

`Scrips.Core.csproj` has no `Grpc.Tools` or `Grpc.Net.Client` reference; the `<Protobuf ... GrpcServices="Client">` items rely on packages that the consuming service supplies. [CONFIRMED: `Scrips.Core.csproj:23-33`; INFERRED that the consumer supplies them]
