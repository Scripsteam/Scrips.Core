Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Confidence tags: [CONFIRMED] read in code or git this session, [INFERRED], [ASSUMED], [NEEDS HUMAN REVIEW].
The old file (584 lines) described workflows, PHI flows and failure cascades of the whole platform. Those cannot be verified from this repo, so they are removed. What remains is what this repo and the submodule pointers of other repos prove.

# Microservices Topology

## Context note [UPDATED]

Scrips.Core is a shared library. It is not a node in the runtime graph. It is a source dependency of the nodes. The facts below come from `.gitmodules` and the submodule commit pointer on each repo's default branch, read with `git show` / `git ls-tree` on 2026-11-15, plus `git grep` on those branches. Only the 20 repos cloned under `~/scrips-repos` were checked; other repos may embed Core too. [CONFIRMED scope]

## 1. Repos that embed Scrips.Core [UPDATED]

"Behind" = commits on Core origin/master (6fc2554) that the pinned commit lacks, from `git rev-list --count <pin>..origin/master` in the Core clone.

| Repo | Default branch | Submodule path | Pinned Core commit | Behind |
|---|---|---|---|---|
| Scrips.Patient | master | `Scrips.Core` | 4268a91 | 1 |
| Scrips.Workflows | master | `Scrips.Core` | 4268a91 | 1 |
| Scrips.VideoApplication | master | `Scrips.Core` | f63d897 | 3 |
| Scrips.PracticeManagement | master | `src/Scrips.Core` | 02f2e28 | 4 |
| Scrips.AppointmentScheduling | master | `Scrips.Core` | c758646 | 5 |
| Scrips.Billing | master | `src/Scrips.Core` | c758646 | 5 |
| Scrips.Notifications | master | `Scrips.Core` | def46c5 | 7 |
| Scrips.QuestionBank | master | `Scrips.Core` | 5d28a54 | 11 |
| Scrips.Organization | master | `Scrips.Core` | 6d07439 | 48 |
| Scrips.Master | master | `Scrips.Core` | d7bd181 | 54 |
| Scrips.Practice.Aggregator | master | `src/Scrips.Core` | 363a8ad | 82 |
| Scrips.EClaim | master | `Scrips.Core` | 108a11f | 116 |
| Scrips.Provider | master | `src/Scrips.Core` | 108a11f | 116 |
| Scrips.Identity | master | `Scrips.Core` | 9870a26 | 118 |
| Scrips.Inventory | master | `src/Scrips.Core` | 65984e4 | 120 |
| Scrips.Persons | master | `Scrips.Core` | 65984e4 | 120 |
| Scrips.Auditing | master | `Scrips.Core` | c22643c | 147 |
| Scrips.Integration.Nabidh | master | `src/Scrips.Core` | c22643c | 147 |
| Scrips.Reporting | master | `Scrips.Core` | c22643c | 147 |
| Scrips.Integration.Malaffi | packages_vulnerabilities (origin HEAD of the local clone) | `src/Scrips.Core` | 2859dbb | not in the local Core clone, unknown |

[CONFIRMED: `.gitmodules` and `git ls-tree` on each default branch] The Malaffi row uses the branch that is origin HEAD in that clone; its master branch was not checked. [NEEDS HUMAN REVIEW]

Consequences:
- Any Core change takes effect in a service only after that service bumps its pointer. A proto change that a caller needs before the callee has it is a coordination problem across repos. [INFERRED from the mechanism]
- Outbox availability by pin (checked with `git merge-base --is-ancestor 12ee6ee <pin>`): the three repos pinned at c22643c (2026-03-09: Auditing, Integration.Nabidh, Reporting) lack the outbox. All other pins (except Malaffi, not resolvable) contain it. [CONFIRMED]
- Fix coverage by pin: four repos that call `AddAuditOutbox` pin a Core commit that lacks the non-GUID `sub` fix (491c662, PROD-1815) and the `READ COMMITTED` drainer fix (1ba4c4f): Identity (9870a26), Persons (65984e4), Inventory (65984e4), Provider (108a11f). Their `AddAuditOutbox` call compiles against an older drainer. If their `Audit:UseOutbox` flag is on, they carry both defects the later commits fixed. The flag values are not in any repo. [CONFIRMED pins and commit ancestry; flag state UNKNOWN] [NEEDS HUMAN REVIEW: Andrew owns the service deploys]
- Organization (6d07439) and the repos pinned later have both fixes; AppointmentScheduling/Billing/Patient and later pins have all three (`200ba1d` integration-event lane too). Master (d7bd181) and Practice.Aggregator (363a8ad) lack only 1ba4c4f. [CONFIRMED]
- Several repos embed Core under a `src/` prefix and `Scrips.Integration.Malaffi` has a copy of Core source in its tree on the checked branch (`src/Scrips.Core/Scrips.Core.Application/GlobalUsings.cs` exists there). [CONFIRMED]

## 2. Which parts each repo uses (sampled) [NEW]

Measured with `git grep` on each default branch, 2026-11-15, counting `.cs` files that match. Submodule content is not searched by `git grep`.

| Feature | Repos with a match |
|---|---|
| `AuditableBaseDbContext` or `AuditableMultiTenantBaseDbContext` | AppointmentScheduling (4), Billing (2), Identity (2), Integration.Malaffi (2), Inventory (1), Organization (5), Patient (10), Persons (1), PracticeManagement (1), Provider (1), QuestionBank (4) |
| `AddAuditOutbox` | AppointmentScheduling, Billing, Identity, Inventory, Organization, Patient, Persons, PracticeManagement, Provider, QuestionBank (10 repos) |
| `OutboxMessage.ForIntegrationEvent` | Patient (11 files) |
| `[MaskValueAudit]` | Integration.Malaffi (2), Patient (2), QuestionBank (1) |
| a class implementing `ICacheService` | Billing (2), Organization (3), Patient (1), Persons (2), PracticeManagement (1), VideoApplication (1), Workflows (2) |
| `Scrips.Core.HttpApiClients` namespace | AppointmentScheduling (4), Identity (9), Integration.Malaffi (10), Patient (6), Practice.Aggregator (33) |
| `Scrips.Core.Domain` namespace | Inventory (12 files), Integration.Malaffi (13 files) |

[CONFIRMED by grep] Not a full inventory: dynamic use, other branches, and repos that are not cloned are not covered.

## 3. Service dependency edges named in this repo [UPDATED]

From proto and header comments; not verified in the callers.

| Caller | Callee | Contract |
|---|---|---|
| Scrips.Workflows | Scrips.Patient | `CreateCarePlan`, `AddCarePlanGoal`, `EvaluateObservationAgainstGoal`, `RecordGoalAchievement`, `FindPatientsByPhone` |
| Scrips.VideoApplication | Scrips.Patient | `RecordAmbientTranscriptCorrections` |
| Scrips.VideoApplication | Scrips.Master | `SearchAndEnhance` (ambient consumer) |
| Scrips.AppointmentScheduling | Scrips.Billing | `UpdateInvoiceProvider` |
| Scrips.Patient | Scrips.QuestionBank | `FreezeEncounterResponse`, `RecordSignSnapshot`, `GetEncounterNarrative`, `GetUnansweredRequiredItems` |
| Scrips.AppointmentScheduling and others | Scrips.PracticeManagement | resource, presence, room, doctor rpcs |
| every audited service | Scrips.Auditing | topic `SaveAudit` via Dapr `pubsub` |

[CONFIRMED comments at `PatientManagement.proto:82-86, 110-121, 122-152`; `BillingManagement.proto:29-31`; `QuestionBankManagement.proto:6-8`; `MasterManagement.proto:166-169`]

## 4. Messaging topology [UPDATED]

- Pub/sub component: `pubsub`. [CONFIRMED]
- `SaveAudit`: producers are services using the auditable contexts; consumer is Scrips.Auditing (`TopicSubscription` at `AuditingServer.cs:41-52`). [CONFIRMED]
- `Topics.cs` names: Identity references `TenantCreated` (2 files); Organization and PracticeManagement reference the Organization*, OrganizationV1*, Practice* and OrganizationSettings* names (1 file each per name; PracticeManagement has 2 for `PracticeActiveArchive`). `DoctorCreated` has no reference in any of the 20 clones. [CONFIRMED by grep; publisher or subscriber role not checked]
- `Scrips.AppointmentScheduling` references `Topics.AppointmentStatusChangedForAutomation` and `Scrips.Patient` references `Topics.SaveAudit`, `Topics.RunRequested` and others; these names are not in Core's `Topics.cs`, so those repos have their own `Topics` classes. [CONFIRMED: grep output; Core's `Topics.cs` has 10 other names]

## 5. Data ownership [UPDATED]

This repo defines no tables except `OutboxMessages` (one per service database). Each service owns its own tables. A service that uses the outbox needs that table in its own database; the table is not created by this repo. [CONFIRMED]
The outbox table lives in "the context's single (shared) database" (comment `OutboxDrainerService.cs:310`). For a multi-tenant context that means one table for all tenants of that service, with `TenantId` on each row. [CONFIRMED comment; INFERRED consequence]

## 6. PHI flow [UPDATED]

Only the facts this repo shows:
- Audit payloads (`OldValue`, `NewValue`) leave a service on `SaveAudit` and, in outbox mode, rest in `OutboxMessages` of the service database until purged (7 days after publish by default; never for dead letters unless scrub is on). [CONFIRMED]
- Protos and DTOs for patients, encounters, appointments, invoices and ambient transcripts carry PHI. [CONFIRMED]
- A whole-platform PHI map cannot be built from this repo. [NEEDS HUMAN REVIEW]

## 7. Risks [UPDATED]

- Stale pins (section 1): 7 repos are 116 or more commits behind. [CONFIRMED]
- A contract change is source-visible only to repos that bump. No CI in this repo checks that consumers still build. [CONFIRMED absence]
- Single point of drift: the audit classes exist in two copies (`AuditableBaseDbContext`, `AuditableMultiTenantBaseDbContext`) that differ only by tenant handling. A fix applied to one can miss the other. [CONFIRMED by reading both]
- Old text's cascading-failure, bottleneck and multi-tenancy-isolation lists are [REMOVED]: they described the platform, not this repo, and were not re-verifiable here.
