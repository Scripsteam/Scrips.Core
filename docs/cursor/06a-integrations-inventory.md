Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Confidence tags: [CONFIRMED] read in code this session, [INFERRED], [ASSUMED], [NEEDS HUMAN REVIEW].

# Integration Inventory

## Summary [UPDATED]

Scrips.Core does not call anything at run time on its own. It defines the contracts other services use, and it contains two active pieces of code that touch infrastructure: the audit publish (Dapr) and the outbox worker (SQL Server and Dapr). [CONFIRMED]

| Kind | Count | Evidence |
|---|---|---|
| Refit HTTP interfaces | 11 interfaces, 57 routes | `Scrips.Core/HttpApiClients/*.cs` |
| gRPC services | 10 services, 254 rpcs, 11 proto files (9 compiled by the csproj) | `Scrips.Core/Protos/*.proto`, `Scrips.Core.csproj:24-32` |
| Dapr pub/sub topics published by code in this repo | 1 (`SaveAudit`) plus any topic a consumer passes to `OutboxMessage.ForIntegrationEvent` | `OutboxMessage.cs:94-111` |
| Dapr topic names declared | 10 in `Topics.cs` | `Topics.cs:5-17` |
| External SaaS called | 0 | no HTTP client code in this repo |
| Database | SQL Server, only for the outbox table | `OutboxDrainerService.cs:54` |

## Internal service integrations [UPDATED]

| Service | Refit interface (routes) | gRPC service (rpcs) | Evidence |
|---|---|---|---|
| Scrips.AppointmentScheduling | `ISchedulingApi` (5) | `AppointmentManagement` (26) | files above |
| Scrips.Patient | `IPatientApi` (5) | `PatientManagement` (56) | |
| Scrips.Billing | `IBillingApi` (7) | `BillingManagement` (22) | |
| Scrips.PracticeManagement | `IPracticeApi` (18) | `PracticeManagement` (51) | |
| Scrips.Provider | `IProviderApi` (5) | `ProviderManagement` (5) | |
| Scrips.Organization | `IOrganizationApi` (4) | `OrganizationConfiguration` (14) | |
| Scrips.Identity | `IIdentityApi` (2) | `IdentityManagement` (21) | |
| Scrips.Persons | `IPersonApi` (1) | `PersonManagment` (3) | |
| Scrips.Master | `IMasterApi` (8) | `MasterManagement` (52) | |
| Scrips.QuestionBank | none | `QuestionBankManagement` (4), not in the csproj | `QuestionBankManagement.proto:6-14` |
| Scrips.Notifications | `INotificationsApi` (1) | none | |
| Email sender service | `IEmailSenderApi` (1) | none | |

The service names above come from the consumer repo names that carry the matching service. The interfaces do not name a host, base URL or Dapr app id. [CONFIRMED: no base address anywhere in this repo] The mapping of `I*Api` to a repo is [INFERRED] from names and `.gitmodules` presence.

### Callers named in proto comments [NEW]
- Scrips.Workflows calls `AddCarePlanGoal`, `EvaluateObservationAgainstGoal`, `RecordGoalAchievement`, `FindPatientsByPhone` and `CreateCarePlan` (comments at `PatientManagement.proto:82-86, 89-91, 115-121, 122-131`). [CONFIRMED comments]
- Scrips.VideoApplication calls `RecordAmbientTranscriptCorrections` and is the ambient consumer of `SearchAndEnhance` (`PatientManagement.proto:110-113`; `MasterManagement.proto:166-169`). [CONFIRMED comments]
- Scrips.AppointmentScheduling calls `BillingManagement.UpdateInvoiceProvider` (`BillingManagement.proto:29`). [CONFIRMED comment]
- Scrips.Patient calls `QuestionBankManagement` (`QuestionBankManagement.proto:6-8`). [CONFIRMED comment]
- Whether the callers do so in their current code was not checked. [NOT VERIFIED]

### Authentication pattern [UPDATED]
55 of 57 Refit routes take the bearer token as an explicit `[Header("Authorization")] string token` parameter (`IEmailSenderApi` and `INotificationsApi` do not). [CONFIRMED: `git grep -c`] The internal-only gRPC rpcs added in Oct and Nov 2026 say in comments "Internal gRPC, no token needed" and take the organization from the token's `orgId` claim when present, else from the `organizationid` metadata header (`PatientManagement.proto:91-92, 1266-1267`). A caller whose token and header disagree, or that sends neither, is refused. [CONFIRMED comment] Whether the services enforce this is not verified. [NEEDS HUMAN REVIEW]

## External integrations [UPDATED]

### Healthcare [UPDATED]
- NABIDH: data field only (`NabidhAssigningAuthority`). Malaffi: none. [CONFIRMED]
- DHA / DOH / eClaim / e-prescription: the Practice proto returns credentials (`GetPracticeEClaimCredential`, `GetAllEnabledEClaimCredentials`, `GetProviderPracticeEPrescriptionCredentials`, `...List`). No call to those external systems is made from this repo. [CONFIRMED]

### Cloud and SaaS [UPDATED]
- Azure AI Search: package and index attributes only (`ChiefComplaintDocument.cs`). No `SearchClient` is created here. [CONFIRMED: `git grep -n "SearchClient"` returns nothing]
- Azure.Identity: referenced in two csproj files, not used in any `.cs` file. [CONFIRMED: `git grep -n "Azure.Identity\|DefaultAzureCredential"` returns nothing in `.cs`]
- Dapr sidecar: `DaprClient` is used to publish. [CONFIRMED]

## Database connections [UPDATED]

- Outbox worker and validator open a `SqlConnection` with the connection string read from the registered `DbContextOptions<TContext>` (relational extension). They throw if none is found (`OutboxDrainerService.cs:312-326`). [CONFIRMED]
- `AddPersistence` reads `DatabaseSettings:ConnectionString` (required) and registers `IConnectionStringSecurer` and `IConnectionStringValidator`. It does not register a DbContext (commented out). [CONFIRMED: `Persistence/Startup.cs:13-40`]
- Provider: SQL Server only. [CONFIRMED]

## Message systems [UPDATED]

- Pub/sub component name: `pubsub` (`AuditOutboxOptions.cs:12`; literal `"pubsub"` at `AuditableBaseDbContext.cs:97`). In legacy mode the name is hard-coded and ignores the `Audit:PubSubName` option. [CONFIRMED]
- Topic `SaveAudit`: published by the DbContext bases; consumed by Scrips.Auditing. [CONFIRMED]
- CloudEvent id: set from the outbox `EventId` on every drained row so the consumer can dedupe (`OutboxDrainerService.cs:218`). Scrips.Auditing passes `EventId` into its command (`AuditingServer.cs:124-129`). Whether it dedupes in the store is not verified here. [CONFIRMED the plumbing; NOT VERIFIED the dedupe]
- Topic constants in `Topics.cs`: see `02-section-9-notifications-communication.md`.

## Cache systems [UPDATED]
Interface only (`ICacheService`, `ICacheKeyService`). 7 consumer clones implement `ICacheService`. [CONFIRMED]

## Integration patterns [UPDATED]
- Contracts by submodule, not by NuGet. Consumers pin different Core commits; on 2026-11-15 they were between 1 and 147 commits behind origin/master (see `08-microservices-topology.md`). [CONFIRMED]
- Protos are additive by habit: most commit titles say "additive". No `buf` or breaking-change check exists in this repo (no CI). [CONFIRMED absence]
- Refit contracts have no retry or timeout policy of their own. [CONFIRMED: no Polly]

## Missing integrations [UPDATED]
Not in this repo: any Redis or distributed cache, Hangfire, SignalR, SMTP, Malaffi, HL7, secrets store use, Polly. [CONFIRMED by grep and csproj read]
