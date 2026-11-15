Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (879 lines, 11 diagrams) showed a CQRS request pipeline, a JWT multi-tenancy flow and a database schema that this repo does not contain. Those diagrams are removed. Five diagrams that match the code remain.

# Architecture Diagrams - Scrips.Core

## 1. System context [UPDATED]

```
  Service repos (20 checked)                       This repo (a library, never started)
  Patient, Billing, Identity, ...    git submodule  +--------------------------------------+
  each pins ONE Core commit  ----------------------> | Scrips.Core  (protos, DTOs, enums,   |
                                                     |   Refit interfaces, claims, topics)  |
        build their own services from this source    | Scrips.BaseDbContext (audit + outbox)|
                                                     | Scrips.Core.Domain / .Application    |
                                                     | Scrips.Infrastructure / .WebApi      |
                                                     +--------------------------------------+
  at run time, inside a service:
     DbContext (inherits Auditable*) --SaveAudit--> Dapr "pubsub" --> Scrips.Auditing
```
[CONFIRMED: `.gitmodules` of 20 repos; `AuditingServer.cs:41-52` in Scrips.Auditing] The 20 pin 20 different-age commits (1 to 147 behind master); see `08-microservices-topology.md`.

## 2. Project layers [UPDATED]

```
 Scrips.WebApi ---------> Scrips.Infrastructure ---------> Scrips.Core.Application ---> Scrips.Core.Domain ---+
   base controllers          CurrentUser, middleware          interfaces, filters,          BaseEntity,          |
                             (mostly commented wiring)        specifications, exceptions    AuditableEntity,     v
                                                                        |                    DomainEvent       Scrips.Core
 Scrips.BaseDbContext ------------------------------------------------ (references) -----------------------> (contracts)
   Auditable*DbContext, outbox
```
[CONFIRMED: `ProjectReference` lines: Infrastructure to Application (`Scrips.Infrastructure.csproj:10`), WebApi to Infrastructure, Application to Domain and Core, Domain to Core, BaseDbContext to Core (`Scrips.BaseDbContext.csproj:55`)] Domain does not depend on Application or on BaseDbContext. The old "Domain has no dependencies" statement was wrong: it references `Scrips.Core` for `IEvent`. [CONFIRMED: `DomainEvent.cs:1`]

## 3. An audited save [UPDATED]

```
 db.SaveChangesAsync()
   |
   +-- AuditLoggingHelper.DetectChanges(ChangeTracker)      sub, tenant claim, ipaddress header
   |
   +-- Audit:UseOutbox == true ?
   |      yes: add OutboxMessage(SaveAudit) to the same context; base.SaveChangesAsync()
   |           (one transaction; failure to stage fails the save)
   |      no : publish List<LogAudit> inline (3 tries: wait 1 s, 2 s), swallow errors,
   |           THEN base.SaveChangesAsync()
```
[CONFIRMED: `AuditableBaseDbContext.cs:30-56`]

## 4. Outbox row life cycle [NEW]

```
        insert (same transaction)
 [0 Pending] ---claim (ROWLOCK,READPAST)---> [1 Claimed] ---publish OK---> [2 Published] --after 7 days--> purged
     ^                                           |   |
     |   reaper: Claimed older than 5 min        |   +--publish fails: Attempts+1
     +-------------------------------------------+        Attempts >= 5 --> [3 DeadLetter]
     |                                                                         |
     +----- daily recovery at 02:00 UTC (RecoveryAttempts+1, Attempts=0) ------+
                                          RecoveryAttempts >= 5 --> terminal; payload scrubbed to "[]"
                                          only if Audit:ScrubTerminalPayload is true; row is kept
```
[CONFIRMED: `OutboxMessage.cs:126-132`; `OutboxDrainerService.cs:89-101, 108-125, 146-178, 254-265`; defaults in `AuditOutboxOptions.cs`]

## 5. Contract map [UPDATED]

```
 Scrips.Core/Protos (10 services, 254 rpcs)                      Scrips.Core/HttpApiClients (11 Refit, 57 routes)
   AppointmentManagement  26   -> AppointmentScheduling             ISchedulingApi  5      IPracticeApi  18
   BillingManagement      22   -> Billing                           IPatientApi     5      IProviderApi   5
   IdentityManagement     21   -> Identity                          IBillingApi     7      IOrganizationApi 4
   MasterManagement       52   -> Master                            IMasterApi      8      IIdentityApi   2
   OrganizationConfiguration 14 -> Organization                     IPersonApi      1      IEmailSenderApi 1
   PatientManagement      56   -> Patient                           INotificationsApi 1
   PersonManagment         3   -> Persons
   PracticeManagement     51   -> PracticeManagement
   ProviderManagement      5   -> Provider
   QuestionBankManagement  4   -> QuestionBank   (not in Scrips.Core.csproj)
```
[CONFIRMED: counts; service-to-repo arrows are INFERRED from names]

## 6. Quick navigation [UPDATED]

| I want to know | File |
|---|---|
| What tables and fields the audit writes | `Scrips.Core/Models/Audit/LogAudit.cs`, `Scrips.BaseDbContext/AuditLoggingHelper.cs` |
| How to turn the outbox on | `Scrips.BaseDbContext/Outbox/AuditOutboxServiceCollectionExtensions.cs`, `AuditOutboxOptions.cs` |
| The claim names | `Scrips.Core/Shared/Authorization/ScripsClaims.cs` |
| The Dapr topic names | `Scrips.Core/Topics.cs` and `Scrips.BaseDbContext/Outbox/OutboxMessage.cs` (`OutboxTopics`) |
| A gRPC contract | `Scrips.Core/Protos/<Service>Management.proto` |
| Which Core commit a service uses | `git ls-tree <branch> <submodule path>` in that service repo |
