# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

Scrips.Core is a **shared-code library** (not a service) consumed by every Scrips backend microservice. It provides:

- **gRPC proto contracts** for all cross-service communication
- **Audit machinery** — `AuditableBaseDbContext` / `AuditableMultiTenantBaseDbContext` base classes that auto-publish `SaveAudit` events on every `SaveChanges`
- **Shared base classes** — entities, specifications, repository interfaces, API controllers, middleware
- **Shared models** — DTOs for cross-service payloads, audit models, authorization primitives
- **HTTP API client interfaces** (Refit) as an alternative to gRPC for specific clients

There is no `Program.cs`, no port, no database — it builds as a class library only.

## Stack

- **TFM:** `net8.0` (all 6 projects)
- **EF Core:** 8.0.11 (pinned — see Known Constraints)
- **Key packages:** Dapr.Client 1.17.0, Finbuckle.MultiTenant.EntityFrameworkCore 6.10.0, Google.Protobuf 3.33.5, MediatR 12.0.1, Mapster 7.4.0, FluentValidation 11.10.0, Ardalis.Specification 8.0.0, Serilog 4.0.2, Refit 8.0.0, Newtonsoft.Json 13.0.4
- **Microsoft.Extensions.\*:** pinned at 10.0.2 for Dapr 1.17.0 runtime compat
- **Some System.\* packages (IO.Pipelines, Text.Json, Threading.Channels) stay at 8.x** — 10.x breaks Kestrel `PipeWriter.Advance()` in gRPC (commits d0dbf45, 598a2f4, c22643c)

## How It's Consumed

**Git submodule, not NuGet.** Every backend service repo (Scrips.Patient, Scrips.Identity, Scrips.OrganizationOnboarding, etc.) embeds this repo at `./Scrips.Core/` and adds `ProjectReference`s to the individual projects it needs.

To pull changes in a consumer:
```bash
git submodule update --remote Scrips.Core
# or inside Scrips.Core/:
git pull origin master
```

Consumers typically reference:
- `Scrips.Core/Scrips.Core.csproj` — protos, shared models, enums, HTTP API interfaces
- `Scrips.BaseDbContext/Scrips.BaseDbContext.csproj` — audit DbContext bases
- `Scrips.Core.Application/`, `Scrips.Core.Domain/`, `Scrips.Infrastructure/`, `Scrips.WebApi/` — as needed

## Solutions

- `Scrips.Core.sln` — single-project solution (just `Scrips.Core/Scrips.Core.csproj`). Legacy.
- `Scrips.Core.All.sln` — full solution: all 6 projects (`Scrips.BaseDbContext`, `Scrips.Core`, `Scrips.Core.Application`, `Scrips.Core.Domain`, `Scrips.Infrastructure`, `Scrips.WebApi`). Use this for work that spans projects.

## Project Layout

| Project | Purpose |
|---------|---------|
| `Scrips.Core` | Proto definitions (`Protos/`), shared enums, HTTP API interfaces (Refit), cross-service models, Dapr topic constants, shared authorization/multitenancy helpers |
| `Scrips.BaseDbContext` | `AuditableBaseDbContext`, `AuditableMultiTenantBaseDbContext`, `AuditLoggingHelper`, `MaskValueAuditAttribute` |
| `Scrips.Core.Domain` | `BaseEntity`, `AuditableEntity`, `DomainEvent`, `IAggregateRoot`, `ISoftDelete`, `FileType` |
| `Scrips.Core.Application` | `ICacheService`, `IFileStorageService`, `IRepository`, `IDapperRepository`, custom exceptions (`ConflictException`, `NotFoundException`, `ForbiddenException`, etc.), specifications, MediatR event abstractions, `IAuditService` |
| `Scrips.Infrastructure` | `CurrentUser`/`CurrentUserMiddleware`, exception/request/response logging middleware, persistence startup, connection string helpers |
| `Scrips.WebApi` | `BaseApiController`, `VersionedApiController`, `VersionNeutralApiController`, `ScripsApiConventions` |

## Public API Surface

### gRPC Proto Contracts (cross-service)

Located in `Scrips.Core/Protos/`. All are built with `GrpcServices="Client"` — consumers generate client stubs; service implementations live in the respective service repos.

| Proto | Service name |
|-------|--------------|
| `AppointmentManagement.proto` | `AppointmentManagement` |
| `BillingManagement.proto` | `BillingManagement` |
| `IdentityManagement.proto` | `IdentityManagement` |
| `MasterManagement.proto` | `MasterManagement` |
| `OrganizationManagement.proto` | `OrganizationConfiguration` |
| `PatientManagement.proto` | `PatientManagement` |
| `PersonManagement.proto` | `PersonManagment` (sic — do not "fix" without coordinated migration; it's the wire contract) |
| `PracticeManagement.proto` | `PracticeManagement` |
| `ProviderManagement.proto` | `ProviderManagement` |

There is also a top-level `Protos/` directory at the repo root (`Appointment.proto`, `Common.proto`, `Identity.proto`, `Organization.proto`) — **these are not compiled by any csproj** and appear to be legacy/reference files. The active protos are the 9 under `Scrips.Core/Scrips.Core/Protos/`.

### HTTP API Client Interfaces (Refit)

In `Scrips.Core/Scrips.Core/HttpApiClients/` — 11 interfaces: `IBillingApi`, `IEmailSenderApi`, `IIdentityApi`, `IMasterApi`, `INotificationsApi`, `IOrganizationApi`, `IPatientApi`, `IPersonApi`, `IPracticeApi`, `IProviderApi`, `ISchedulingApi`.

### Dapr Topic Constants

`Scrips.Core/Topics.cs` — typed constants for commonly-published events (`TenantCreated`, `DoctorCreated`, `OrganizationCreated`, `PracticeCreated`, `OrganizationSettingsCreated`, etc.). Most services use `nameof(SomeEvent)` directly and bypass this file — extend it when you need to share a topic name across services.

## AuditableBaseDbContext / AuditableMultiTenantBaseDbContext

Both live in `Scrips.BaseDbContext/`. Every service's `DbContext` inherits one of these.

**What they provide:**

1. **Change detection** — `AuditLoggingHelper.DetectChanges(ChangeTracker, _httpContextAccessor)` walks the ChangeTracker in `SaveChanges`/`SaveChangesAsync` and emits a `List<LogAudit>` describing every Added/Modified/Deleted entity.
2. **Dapr publish of `SaveAudit` event** — payload goes to `pubsub` topic `SaveAudit` with **3-retry exponential backoff (1s, 2s, 4s)**. On final failure the full audit payload is logged as JSON via Serilog so it can be recovered from log aggregation.
3. **Audit failures never block the save** — `try/catch` around the audit step; the underlying `base.SaveChanges` always runs.
4. **Multi-tenant variant** extends Finbuckle's `MultiTenantDbContext` — automatically scopes queries to the current `ITenantInfo`, preventing cross-tenant data leaks.
5. **Sensitive field masking** via `[MaskValueAuditAttribute]` (applied per-property in entity definitions).

**Sync `SaveChanges()` deliberately uses `Task.Run(...).GetAwaiter().GetResult()`** to publish audits — `DaprClient.PublishEventAsync` has no sync API, `DbContext.SaveChanges()` must stay synchronous, and `Task.Run` sidesteps SynchronizationContext deadlocks. This is NABIDH-compliance-mandated behavior, reviewed in SND-331 / SND-386 (see inline comment in both files).

## Known Constraints

### EF Core 8.0.11 pinned — DO NOT align with Scrips.Identity's 9.0.14

`Scrips.BaseDbContext.csproj` explicitly pins `Microsoft.EntityFrameworkCore` and `Microsoft.EntityFrameworkCore.Relational` at **8.0.11**.

Scrips.Identity upgraded its own projects to EF Core **9.0.14** in commit `6113dfa` (2026-03-18, "Upgrade Duende IdentityServer 6.3→7.4.7, AutoMapper 12.0.1→16.1.1, EF Core→9.0.14") — but its embedded `Scrips.Core/` submodule stayed at 8.0.11. This asymmetry is **deliberate**:

- Every other service (Patient, Practice, Billing, Provider, etc.) still runs EF Core 8.x.
- Bumping Scrips.Core's EF Core to 9.x would force every consuming service to upgrade in lockstep or break at submodule update.
- Only Identity has the appetite/test coverage for EF 9 right now.

**If you are tempted to "align" Scrips.Core's EF Core to 9.x: stop.** Coordinate across all consuming service repos first. See Scrips.Identity commit `6113dfa` for context.

### gRPC / Kestrel package pins

`System.IO.Pipelines`, `System.Text.Json`, `System.Threading.Channels` **must stay at 8.x** in `Scrips.BaseDbContext.csproj`. 10.x breaks `PipeWriter.Advance()` in Kestrel's gRPC pipeline. See commits `d0dbf45` → `598a2f4` (bump then revert) and `c22643c` (downgrading UserSecrets to 8.0.1 to avoid a transitive pull of System.Text.Json 10.x).

`NU1605` is suppressed in `Scrips.Core.csproj` and `Scrips.BaseDbContext.csproj` for the same reason (System.\* stay at 8.x while some Microsoft.Extensions.\* are at 10.x).

### Proto changes require careful rollout

Because protos are the wire contracts between services:
1. **Additive changes first** — new fields, new RPCs, new message types. Keep field numbers stable.
2. **Never reuse a field number** or rename a service / RPC without a versioned migration.
3. **Ship proto changes to Scrips.Core first, merge to master**, then update each consuming service's submodule pointer as that service rolls out.
4. **Typos like `PersonManagment` stay as-is** — renaming is a breaking wire change.

### No CI/CD pipeline in this repo

There is **no `.github/workflows/` directory**. Scrips.Core is not published as a package; it is consumed via git submodule from master. "Release" = merge to master → consumers bump their submodule pointer.

## Testing

**No test projects exist in this repo.** Scrips.Core has no `*.Tests` directory and no test configuration. Coverage for this shared code lives in the **consuming services'** unit test suites (e.g., `Scrips.Patient/Scrips.UnitTests`, `Scrips.Identity/tests/Identity.UnitTests`).

When changing audit machinery or shared base classes, validate by running the consuming service's tests after bumping their submodule pointer — there is no standalone `dotnet test` for this repo.

## Existing Documentation

- `README.md` — high-level overview
- `ONBOARDING.md` — onboarding guide for new engineers
- `CONTRIBUTING.md` — contribution rules
- `audit-report-Scrips.Core.md` — audit findings
- `docs/cursor/` — cursor-generated codebase docs
- `.cursorrules` — editor rules


## Skill Routing

When the request matches a skill, invoke it as your FIRST action.

- Ship, create PR → invoke **ship**
- Code review, review this diff → invoke **review**
- Bug, crash, unexpected behavior → invoke **investigate**
- Security audit, PHI, OWASP → invoke **cso**
- Save progress → invoke **checkpoint**

Methodology (auto-invoke without being asked):
- New feature → invoke **brainstorming** FIRST
- Requirements clear, about to code → invoke **writing-plans**
- About to say "done" or "it works" → invoke **verification-before-completion** FIRST
- Bug or test failure → invoke **systematic-debugging** FIRST

Skills at: `~/.claude/skills/scrips/`

