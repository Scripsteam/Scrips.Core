Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Confidence tags: [CONFIRMED] read in code or git this session, [INFERRED], [ASSUMED], [NEEDS HUMAN REVIEW].
The old file (556 lines, with time estimates and a roadmap) was replaced. Effort estimates and the sprint roadmap were not re-derived and are removed. Severity here is a code-reading judgement, not a risk assessment. Fixes are not made in this docs-only change.

# Technical Debt Inventory

## Executive summary [UPDATED]

49 items, grouped below. Highest value first:

1. Four services that call `AddAuditOutbox` pin a Core commit without two outbox fixes (TD-A2). Routing: the service owner (Andrew).
2. No test or CI in the repo that every service builds from source (TD-T1, TD-A1).
3. Practice proto returns e-claim and e-prescription credentials in plain message fields (TD-S1). [NEEDS HUMAN REVIEW]
4. The legacy audit path publishes before the data commit (TD-C1).
5. `AuditableEntity` does not get the `NewId` id that `BaseEntity` gets (TD-C5).

Resolved since March, verified: audit publish now has retry and a payload log; transactional outbox exists; Dapr 1.17.0; .NET 8 everywhere; non-GUID `sub` no longer aborts a save. [CONFIRMED in code]

## 1. Code debt [UPDATED]

| ID | Item | Evidence | Severity |
|---|---|---|---|
| TD-C1 | Legacy path publishes `SaveAudit` before `base.SaveChanges`; a failed data save leaves an audit event for a change that did not commit | `AuditableBaseDbContext.cs:42-55`; twin `AuditableMultiTenantBaseDbContext.cs:53-66` | Medium |
| TD-C2 | `AuditEntityEntry.ToLogAudit` uses `DateTime.Now` (server local time) | `AuditEntityEntry.cs:35` | Low |
| TD-C3 | A missing `sub` gives an empty-GUID user in audit rows | `AuditLoggingHelper.cs:33-44`, `AuditEntityEntry.cs:36` | Medium |
| TD-C4 | The two base DbContexts are near copies (same retry, same outbox branch, same `SaveAudit`) | `AuditableBaseDbContext.cs`, `AuditableMultiTenantBaseDbContext.cs` | Low |
| TD-C5 | `AuditableEntity<T>` derives from `BaseEntity<T>`, so it does not run the `BaseEntity` constructor that assigns `NewId.Next()`; its `Id` starts as `default` | `AuditableEntity.cs:7`, `BaseEntity.cs:6-12` | Medium. Consumer entities not checked [NOT VERIFIED] |
| TD-C6 | `CurrentUser.GetUserId()` uses `Guid.Parse` on `sub`; a non-GUID `sub` throws `FormatException` (the audit helper was fixed for this, `CurrentUser` was not) | `Scrips.Infrastructure/Auth/CurrentUser.cs:15-18` | Medium |
| TD-C7 | Domain events are never collected or dispatched in this repo; `IEventPublisher`, `IRepositoryWithEvents<T>` and `EventAddingRepositoryDecorator` (commented) have no implementation | `git grep DomainEvents` (contract and a comment only); `Persistence/Startup.cs:75-90` (commented) | Low |
| TD-C8 | Dead or unreferenced items: `Topics.DoctorCreated` (no match in 20 clones), `StatsChangedNotification`, `JobNotification`, `INotificationSender`, `IJobService` (no use in this repo), Azure.Identity package (no `.cs` use) | greps listed in `06a` | Low |
| TD-C9 | Same type names in two folders: 7 scheduling types in `Models/Scheduling` and `Enums/Scheduling`; 5 billing/practice/scheduling DTO names | `git ls-files` | Low |
| TD-C10 | Misspelled public names baked into the wire contract: `PersonManagment`, `GetUniverisytById`, `GetRelationionList`, `UpdateQuestionnireStatus`, `GetUserAccesCodeByEmail`, `SetIsResivedTrueInClaim`, `UpdateRFVInAppoinmentProfile`, `UpdateIdenitification`, `UpdateGuardianIdenitification` | protos and `Models/Patient` | Low (renaming is breaking) |
| TD-C11 | `PaginationFilter.PageSize` defaults to `int.MaxValue`; `PaginateBy` rewrites `PageNumber` and `PageSize` on the caller's filter object | `PaginationFilter.cs:7`; `SpecificationBuilderExtensions.PaginateBy` | Low |
| TD-C12 | `VersionedApiController` imports `Microsoft.AspNetCore.Components` for `[Route]` (the Blazor attribute namespace); no `Microsoft.AspNetCore.Mvc` using; `ImplicitUsings` in a non-Web SDK project does not add it | `Scrips.WebApi/VersionedApiController.cs:1-6`, `Scrips.WebApi.csproj:1,5` | Unknown. Not tested [NEEDS HUMAN REVIEW] |
| TD-C13 | Legacy path ignores the `Audit:PubSubName` option (hard-coded `"pubsub"`) | `AuditableBaseDbContext.cs:97` vs `AuditOutboxOptions.cs:12` | Low |

## 2. Architecture debt [UPDATED]

| ID | Item | Evidence | Severity |
|---|---|---|---|
| TD-A1 | No CI workflow, no build gate, no breaking-change check for protos | `git ls-files` has no `.github` | High |
| TD-A2 | Consumers pin old Core commits. Identity (9870a26), Persons and Inventory (65984e4), Provider (108a11f) call `AddAuditOutbox` but lack fix 491c662 (non-GUID `sub` aborts the business write) and fix 1ba4c4f (drainer `READPAST` error 650). Flag state per environment is not in any repo. | `08-microservices-topology.md` section 1; `git merge-base --is-ancestor` | High if the flag is on [UNKNOWN] |
| TD-A3 | Three repos (Auditing, Integration.Nabidh, Reporting) pin 2026-03-09 (c22643c) and have no outbox. Auditing is the `SaveAudit` consumer. | same | Medium |
| TD-A4 | No `OutboxMessages` schema script in this repo; the startup validator names `OutboxMessages.sql`, which is not here | `OutboxStartupValidator.cs:60`; `git ls-files` | Medium |
| TD-A5 | `QuestionBankManagement.proto` is not in `Scrips.Core.csproj`; the root `Protos/` folder (4 files, namespace `Scrips.Generated.Models`) is referenced by no project | `Scrips.Core.csproj:24-32` | Low |
| TD-A6 | `Scrips.Core.csproj` lists `<Protobuf>` items but no `Grpc.Tools`, `Google.Protobuf` or `Grpc.Net.Client` package; a standalone build was not tried | `Scrips.Core.csproj:9-33` | Unknown [UNVERIFIED] |
| TD-A7 | `Scrips.Infrastructure` is mostly commented-out wiring; its middleware classes are `internal` with `internal` registration methods | `Startup.cs:13-58`, `Middleware/Startup.cs:7-37` | Medium |
| TD-A8 | Multi-database support commented out; SQL Server hard-coded | `Persistence/Startup.cs:23, 42-94` | Low |
| TD-A9 | Two parallel contracts for the same service (Refit and gRPC); no statement of which is canonical | `IMasterApi` AI routes vs `MasterManagement` AI rpcs | Low |
| TD-A10 | .NET 8 support end date is November 2026 [UNVERIFIED date]; every consumer must move together | csproj line 4 | Medium [NEEDS HUMAN REVIEW] |
| TD-A11 | Legacy `Scrips.Core.sln` holds only one project; `Scrips.Core.All.sln` holds all six | `CLAUDE.md` [INFERRED; sln files not re-read] | Low |

## 3. Dependency debt [UPDATED]

| ID | Item | Severity |
|---|---|---|
| TD-D1 | Deliberate version split: `Microsoft.Extensions.*` 10.0.2 with EF Core 8.0.11 and System.* 8.x. Comments say a bump breaks Kestrel gRPC. `Scrips.BaseDbContext.csproj:7, 23, 45` | Medium (fragile) |
| TD-D2 | Old fixed versions: `Microsoft.CSharp` 4.7.0, `System.Runtime` 4.3.1, `System.Memory` 4.5.5, `Microsoft.IdentityModel.*` 7.7.1 | Low [advisory status UNVERIFIED] |
| TD-D3 | No advisory scan was run | Unknown [UNVERIFIED] |

## 4. Test debt [UPDATED]

| ID | Item | Severity |
|---|---|---|
| TD-T1 | No test project in the repo; audit, outbox, specification filter builder and exception middleware have no test here | High |
| TD-T2 | The advanced filter builds expression trees from caller strings (`Field`, `Operator`, `Value`) via reflection; no test and no allow-list of fields | Medium. Caller is a consumer's API. [INFERRED exposure] `SpecificationBuilderExtensions.cs` |

## 5. Documentation debt [UPDATED]

| ID | Item | Severity |
|---|---|---|
| TD-X1 | Root `README.md`, `ONBOARDING.md`, `CONTRIBUTING.md`, `CLAUDE.md` and `audit-report-Scrips.Core.md` (2026-02-18) were not refreshed in this change. `README.md` says "9 gRPC" and "docs 28k lines", which are out of date. | Low |
| TD-X2 | The old `docs/cursor` files carried an audit score table and "HIPAA violation" statements that did not match code (for example `Dapr.Client v1.10.0`, `net7.0`). They are replaced in this change. | Done |

## 6. Performance debt [UPDATED]

| ID | Item | Evidence | Severity |
|---|---|---|---|
| TD-P1 | Legacy sync `SaveChanges()` blocks a thread-pool thread on the publish (`Task.Run(...).GetAwaiter().GetResult()`) | `AuditableBaseDbContext.cs:79` | Medium. The outbox path avoids it. |
| TD-P2 | Audit values are serialized per save on the request thread in both modes | `AuditEntityEntry.cs:41-43` | Low |
| TD-P3 | Drainer publishes rows one by one, in a loop, per batch of 200 | `OutboxDrainerService.cs:205-266` | Low |
| TD-P4 | Several proto messages return unbounded repeated fields without paging (old audit text said so; not re-counted) | protos | Unknown [NOT VERIFIED] |

## 7. Security debt [UPDATED]

| ID | Item | Evidence | Severity |
|---|---|---|---|
| TD-S1 | `GetPracticeEClaimCredentialResponse` returns `password`, `userName`, `licenseNumber`, `organizationId`; `GetProviderPracticeEPrescriptionCredentialsResponse` returns `facilityLogin`, `facilityPwd`, `clinicianLogin`, `clinicianPwd` and licence fields; `GetAllEnabledEClaimCredentials` returns the e-claim set for every enabled practice in one call. Field names only; no value is copied here. | `PracticeManagement.proto:72-76, 220-229, 263-271` | High [NEEDS HUMAN REVIEW] |
| TD-S2 | `passwordResetToken` field in an Identity user-details response | `IdentityManagement.proto:199` | Medium [NEEDS HUMAN REVIEW] |
| TD-S3 | `EmiratesIdCardData` DTO holds ID number, names, DOB, passport number, photo and signature bytes | `Models/Identity/EmiratesIdCardData.cs` | Medium [NEEDS HUMAN REVIEW] |
| TD-S4 | 55 of 57 Refit routes pass the bearer token as a method argument | `git grep -c` | Low |
| TD-S5 | `SearchAndEnhance` and `GetClinicalSuggestions` carry no organization, patient or encounter id; comment says the consumer is `[Authorize]`-bypassed (CRIT-002) | `MasterManagement.proto:166-174, 206-211` | Medium [NEEDS HUMAN REVIEW] |
| TD-S6 | Masking is opt-in per property; primary keys are never masked; consumer use is in 3 of 20 clones | `AuditLoggingHelper.cs:53-58` | Medium [NEEDS HUMAN REVIEW] |
| TD-S7 | Outbox rows hold the audit payload (old and new values) at rest in each service database | `OutboxMessage.cs:34-35` | Medium [NEEDS HUMAN REVIEW] |
| TD-S8 | Internal rpcs say "no token needed" and take the organization from the caller; no in-repo check that ingress blocks them | `PatientManagement.proto:91-92, 127, 1266-1267` | Medium [NEEDS HUMAN REVIEW] |

## 8. Healthcare compliance debt [UPDATED]

Code facts only. Mapping to a regulation is not decided here. [NEEDS HUMAN REVIEW for each row]

| ID | Item | Evidence |
|---|---|---|
| TD-H1 | The inline comment ties legacy audit publish to NABIDH audit-trail compliance. The outbox path changes when and how the publish happens. Nobody has recorded that the outbox path still meets it. | `AuditableBaseDbContext.cs:58-62` |
| TD-H2 | Scrub of terminal dead-letter payloads is off by default and gated on a compliance sign-off named PROD-1481 | `AuditOutboxOptions.cs:56-61` |
| TD-H3 | No tamper evidence on `LogAudit` (no hash or signature field) | `LogAudit.cs:3-14` |
| TD-H4 | `Coding.PreferredDisplay` must not reach claims or HIE payloads; nothing in this repo enforces it | `Models/Patient/Coding.cs` |
| TD-H5 | Ambient consent rules (fail closed, `consentNotRequired`) live in proto comments; the service enforces them or not | `PatientManagement.proto:100-105, 183-185` |
| TD-H6 | `clinicianReviewRequired` is an optional field; an unset value must be read as "review required" by every client | `MasterManagement.proto:183-184` |

## Remediation routing [NEW]

Not decided in this docs change. Suggested owners by lane: pins and deploys (TD-A2, TD-A3) to the engineering owner for service deploys; credential and token fields (TD-S1, TD-S2) to a security and compliance review; everything else is code work in this repo.
