Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (391 lines) was replaced. Its 2026-01 "quality audit" tables, HIPAA score and `Dapr.Client v1.10.0` statements are removed: they described code that no longer exists.

# Section 7: Audit and Compliance

## 1. Scope [UPDATED]

`Scrips.BaseDbContext` gives a service two ways to record who changed what. The legacy way publishes to Dapr inline. The new way (the audit outbox) writes a row in the same database transaction and a background worker publishes it. This repo has no audit store and no query side. Scrips.Auditing consumes the `SaveAudit` topic. [CONFIRMED: Scrips.Auditing `API/Callback/AuditingServer.cs:46-49, 124-129` on origin/master]
`Scrips.Core.Application/Auditing/` holds `IAuditService.GetUserTrailsAsync(Guid)`, `AuditDto` and `GetMyAuditLogsRequest` (a MediatR request). No implementation is in this repo. [CONFIRMED]

## 2. Change detection [UPDATED]

`AuditLoggingHelper.DetectChanges(ChangeTracker, IHttpContextAccessor)` (`AuditLoggingHelper.cs:13-97`):

1. Takes every tracked entry whose state is not Unchanged or Detached and whose entity is not a `LogAudit` (`:16-17`).
2. Reads the user from claim `sub` (`:20`), the tenant from claim `tenant` (`:21`) and the IP from request header `ipaddress` (`:23`).
3. If `sub` parses as a GUID it goes to `AuditEntityEntry.User`; otherwise the raw string goes to `UserRaw` (`:40-43`). This is PROD-1815 (commit 491c662, 2026-07-30). Reason in the comment: on the outbox path an exception here would abort the business write.
4. For each property: primary keys go to `KeyValues`; for Added, Deleted and Modified entries the values go to `NewValues` and/or `OldValues`. A property with `[MaskValueAudit]` is written as `"*"` (`:27, 53-54, 66, 70, 77-78`). A Modified property is recorded only if `IsModified` is true and the original and current values differ (`:73-74`).
5. An entry whose action type stays 0 is dropped (`:90-91`).
6. `ToLogAudit()` builds a `LogAudit` (`AuditEntityEntry.cs:32-44`): `User = UserRaw ?? User.ToString()`, `Timestamp = DateTime.Now` (server local time, not UTC; `:35`), `Action` = Added, Updated or Deleted, `KeyValue`, `OldValue`, `NewValue` as Newtonsoft JSON, with empty old or new dictionaries written as null.

Limits, all [CONFIRMED] by reading the code:
- If `sub` is absent (`user is null`), `User` stays an empty GUID and `UserRaw` stays null, so the row shows `00000000-0000-0000-0000-000000000000` (`AuditLoggingHelper.cs:33-44`; `AuditEntityEntry.cs:36`).
- `[MaskValueAudit]` is applied by property metadata only. `git grep` finds no use of the attribute inside this repo. Consumers use it: Scrips.Patient (2 files), Scrips.Integration.Malaffi (2), Scrips.QuestionBank (1), measured on default branches 2026-11-15. The other consumers sampled have none. [CONFIRMED sample, not exhaustive]
- Masking covers property values. It does not cover `KeyValues` (primary keys are written as is, `:58`).
- `AuditLoggingHelper.DetectChanges` is static and has no hook for a consumer to exclude an entity type.

## 3. The two save paths [UPDATED]

Both `AuditableBaseDbContext` and `AuditableMultiTenantBaseDbContext` override `SaveChanges` and `SaveChangesAsync`. The two files are copies of each other except for the tenant id. [CONFIRMED: `AuditableBaseDbContext.cs:30-87`, `AuditableMultiTenantBaseDbContext.cs:41-98`]

| | Legacy inline path (default) | Outbox path (`Audit:UseOutbox` true) |
|---|---|---|
| Switch | `AuditOutboxRuntime.Enabled == false` | `AuditOutboxRuntime.Enabled == true` (`AuditOutboxOptions.cs:69-72`) |
| What happens | `DetectChanges`, then publish `List<LogAudit>` to topic `SaveAudit` on pubsub `pubsub`, then `base.SaveChanges` | `DetectChanges`, then `Set<OutboxMessage>().Add(OutboxMessage.ForAudit(...))`, then `base.SaveChanges` |
| Atomic with the data | No | Yes: one `SaveChanges`, one transaction (`AuditableBaseDbContext.cs:32-40`) |
| On audit failure | Caught; `Log.Warning`; the business save still runs (`:50-53`) | Not caught; the save fails (comment `:34-35`) |
| Retry | 3 attempts, delays 1 s and 2 s between them (`:15, 93-109`) | Done later by the drainer |
| Last resort | Logs the full audit payload as JSON with `Log.Error` (`:112-115`) | Dead letter row, then recovery pass |
| Sync `SaveChanges()` | `Task.Run(() => SaveAudit(changes)).GetAwaiter().GetResult()` (`:79`) | No publish on the request thread |

Notes:
- In the legacy path the publish happens before `base.SaveChanges`. If the data save then fails, an audit event exists for a change that never committed. [CONFIRMED by line order `:44-55`]
- With three attempts the waits are 1 s and 2 s (after attempt 1 and 2; none after attempt 3). The old docs said "1 s, 2 s, 4 s". [CONFIRMED: `:105-108`]
- The legacy publish payload is a typed `List<LogAudit>`, which Dapr serializes with its own defaults (camelCase). The outbox stores `JsonSerializer.Serialize(changes)` with default options (PascalCase) and the drainer deserializes it back to `List<LogAudit>` before publishing, to keep the old wire format. [CONFIRMED: `OutboxMessage.cs:73`; `OutboxDrainerService.cs:238-242`]

## 4. Audit outbox [NEW]

Introduced by PROD-1477, 1479, 1481 and 1868 (commits 12ee6ee 2026-07-03 to 200ba1d 2026-08-11) and hardened on 2026-10-15 (commit 1ba4c4f).

### 4.1 Wiring [NEW]
`services.AddAuditOutbox<TContext>(configuration)`: binds `AuditOutboxOptions` from config section `Audit`, sets the process-wide flag from `Audit:UseOutbox`, and only when true registers `OutboxStartupValidator<TContext>` then `OutboxDrainerService<TContext>`. With the flag off it only binds options. [CONFIRMED: `AuditOutboxServiceCollectionExtensions.cs:16-33`]
Consumers that call it (default branches, 2026-11-15): AppointmentScheduling, Billing, Identity, Organization, Patient, Persons, PracticeManagement, Provider, QuestionBank, Inventory (10 of the 20 local clones that embed Core). Scrips.Master, Notifications, Reporting, Workflows, EClaim, Auditing, VideoApplication, Integration.Nabidh, Integration.Malaffi and Practice.Aggregator have no match. [CONFIRMED by grep for `AddAuditOutbox`]
Committed settings (`git grep` for `UseOutbox` in `*.json`, default branches): 9 of those 10 commit a dev settings file with `Audit:UseOutbox` true and `Audit:ScrubTerminalPayload` true (AppointmentScheduling `API/appsettings.dev.json:12-13`, Billing `src/Billing.API/appsettings.dev.json:13-14`, Identity `:64-65`, Inventory `:5-6`, Organization `Scrips.Organization.Api/appsettings.dev.json:18-19`, Patient `API/appsettings.dev.json:79-80`, Persons `:14-15`, PracticeManagement `API/Scrips.API.PracticeManagement/appsettings.dev.json:40-41`, Provider `:10-11`). Billing also commits `UseOutbox` true in `appsettings.uat.json:17` and `appsettings.prod.json:17`. QuestionBank has no committed value. [CONFIRMED: file reads on origin default branches] The settings that actually run in each environment can differ from the files and were not verified. [NEEDS HUMAN REVIEW] The default in code stays false for both options (`AuditOutboxOptions.cs:10, 61`), so on dev those services have terminal dead-letter payload scrubbing ON by committed setting.

### 4.2 Table [NEW]
`OutboxMessages` (`OutboxModelConfig.cs:11`). The context maps it in `OnModelCreating` (both bases call `OutboxModelConfig.Apply`). Columns required at startup: Id, EventId, TenantId, Topic, Payload, CreatedUtc, Status, Attempts, ClaimCount, ClaimedUtc, ProcessedUtc, RecoveryAttempts, ScrubbedUtc (`OutboxStartupValidator.cs:32-36`). A filtered index covers `Status IN (0,1)` (`OutboxModelConfig.cs:27-29`). The validator message names an `OutboxMessages.sql` script; no `.sql` file is in this repo, so each consumer must create the table itself. [CONFIRMED: `git ls-files` has no `.sql`]

`Status`: 0 Pending, 1 Claimed, 2 Published, 3 DeadLetter (`OutboxMessage.cs:126-132`).

### 4.3 Options (`AuditOutboxOptions.cs`) [NEW]

| Key under `Audit:` | Default | Line |
|---|---|---|
| `UseOutbox` | false | 10 |
| `PubSubName` | `pubsub` | 12 |
| `BatchSize` | 200 | 15 |
| `MaxAttempts` | 5 | 18 |
| `PollSeconds` | 5 | 21 |
| `PublishTimeoutSeconds` | 30 | 25 |
| `VisibilityTimeoutMinutes` | 5 | 28 |
| `PublishedRetentionDays` | 7 (0 or less turns purge off) | 32 |
| `PurgeBatchSize` | 1000 (hard cap 5000 in the drainer, `OutboxDrainerService.cs:104, 120`) | 35 |
| `RecoveryEnabled` | true | 42 |
| `RecoveryRunAtUtcHour` | 2 | 46 |
| `RecoveryMaxAttempts` | 5 | 51 |
| `RecoveryBatchSize` | 200 | 54 |
| `ScrubTerminalPayload` | false (compliance decision, comment cites PROD-1481 sign-off) | 61 |

### 4.4 Drainer loop (`OutboxDrainerService.cs:40-77`) [NEW]
Every `PollSeconds`: open a raw `SqlConnection` from the registered `DbContextOptions` (not from the context, so multi-tenant contexts work; `:312-326`); set `READ COMMITTED` (`:79-86`, needed because `READPAST` fails with error 650 under other isolation levels); reap stale claims; drain a batch; purge published rows; once a day at `RecoveryRunAtUtcHour` run recovery. An exception in one iteration is logged and the loop continues.
- Claim: `UPDATE TOP (@batch) ... WITH (ROWLOCK, READPAST) SET Status = Claimed ... OUTPUT` (`:187-191`). Several pods can drain in parallel without claiming the same row twice. Delivery is at-least-once, not exactly-once: if a pod stops after Dapr accepts a publish but before `MarkPublishedAsync` runs (`:238-248, 269-275`), the reaper returns the row to Pending and another pod publishes it again, and the scheduled recovery re-queue repeats rows too. Consumers must deduplicate on the CloudEvent id (the row's `EventId`); that is stated for the audit lane and applies to the integration-event lane too. [CONFIRMED by reading the order of publish and mark] `ClaimCount` goes up, `Attempts` does not.
- Publish: per-row timeout; CloudEvent id set to the row's `EventId` so the consumer can dedupe (`:218`); audit lane re-deserializes to `List<LogAudit>` (`:238-242`); every other topic is published verbatim as a `JsonElement` (`:243-247`).
- On failure: `Attempts + 1`; at `MaxAttempts` the row becomes DeadLetter and an Error is logged (`:254-265, 280-295`).
- Reaper: Claimed rows older than `VisibilityTimeoutMinutes` go back to Pending without touching `Attempts` (`:89-101`).
- Purge: one bounded `DELETE TOP` of Published rows older than the retention days, using `COALESCE(ProcessedUtc, CreatedUtc)`; DeadLetter rows are never purged (`:108-125`).
- Recovery: DeadLetter rows with `RecoveryAttempts < RecoveryMaxAttempts` go back to Pending with `Attempts = 0` and `RecoveryAttempts + 1` (`:146-159`).
- Scrub (only if `ScrubTerminalPayload`): DeadLetter rows with `RecoveryAttempts >= RecoveryMaxAttempts` get `Payload = "[]"` and `ScrubbedUtc` set; the row is kept (`:164-178`; `OutboxMessage.cs:66`). Comment cites UAE Federal Law 2/2019 for the keep-forever rule. [NEEDS HUMAN REVIEW: legal reading]

### 4.5 Integration events [NEW]
`OutboxMessage.ForIntegrationEvent(topic, payload, tenantId)` stores a non-audit event so a service can publish it durably. It refuses an empty topic and the reserved `SaveAudit` topic, and serializes with web defaults so the wire format matches an inline `PublishEventAsync` (`OutboxMessage.cs:94-115`). Measured use: Scrips.Patient (11 files mention it). No other sampled consumer. [CONFIRMED by grep on default branches]

### 4.6 Fail-fast checks [NEW]
`OutboxStartupValidator<TContext>` refuses to start if the table or a required column is missing, using the registered connection string (`OutboxStartupValidator.cs:44-68`). It then checks that the model maps `OutboxMessage`, but skips that check with a warning when the context cannot be built at startup (multi-tenant contexts) (`:72-88`). [CONFIRMED]

## 5. Compliance observations (code facts only) [UPDATED]

- The inline comment in the legacy path says every entity change must be published before the save completes, for NABIDH audit-trail compliance, approved in SND-331 / SND-386 (`AuditableBaseDbContext.cs:58-62`). That sentence is the only explicit NABIDH reference in the audit code. [CONFIRMED] Whether the outbox path still meets that requirement is a compliance decision. [NEEDS HUMAN REVIEW]
- `LogAudit` carries old and new values. For PHI entities these values contain PHI in clear text unless a property is marked. Outbox rows hold the same payload at rest in the service database for 7 days after publish (default) and indefinitely for dead letters. [CONFIRMED by code; NEEDS HUMAN REVIEW for the compliance meaning]
- Retention of audit records in the audit store, tamper evidence and alerting are not in this repo. [CONFIRMED absence]
- Which regulation clause each mechanism satisfies is not decided in code. [NEEDS HUMAN REVIEW]

## 6. Gaps [NEW]
- No schema script for `OutboxMessages` in the repo (above).
- No unit or integration test in the repo for any of this (`git ls-files` has no test project). [CONFIRMED]
- `AuditEntityEntry` uses `DateTime.Now`; mixed server time zones would give mixed timestamps. [INFERRED impact]
- Missing-`sub` rows have an empty GUID user (above).
