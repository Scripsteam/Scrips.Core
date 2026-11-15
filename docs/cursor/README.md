Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15.

# Scrips.Core Documentation Index

Scrips.Core is a shared library, not a service. These files describe what the code in this repo shows. Every file has a top line with the commit it was checked against. Every section has a change marker: `[UPDATED]`, `[NEW]`, `[REMOVED]` or `[UNCHANGED, re-verified]`. Confidence tags: `[CONFIRMED]`, `[INFERRED]`, `[ASSUMED]`, `[NEEDS HUMAN REVIEW]`, `[UNVERIFIED]`.

## Start here [UPDATED]

| Question | File |
|---|---|
| What is in the repo? Projects, packages, counts | `00-workspace-inventory.md` |
| What rules does the code enforce? | `01-business-logic-outline.md` |
| How is it built? Layers, data flow, risks | `05-architecture.md`, `ARCHITECTURE-DIAGRAM.md` |
| Which services and protocols does it describe? | `06a-integrations-inventory.md`, `06b-integration-details.md` |
| Package versions and upgrade constraints | `07-framework-audit.md` |
| Which repos use it, and which commit they pin | `08-microservices-topology.md` |
| Known problems | `09-technical-debt-inventory.md` |

## Domain files [UPDATED]

| Domain | File |
|---|---|
| Appointments and scheduling | `02-section-1-appointments-scheduling.md` |
| Patient management | `02-section-2-patient-management.md` |
| Billing and financial | `02-section-3-billing-financial.md` |
| Clinical documentation and AI | `02-section-4-clinical-ai.md` |
| Provider, practice, organization | `02-section-5-provider-practice-org.md` |
| Identity and tenancy | `02-section-6-identity-tenancy.md` |
| Audit and compliance (includes the audit outbox) | `02-section-7-audit-compliance.md` |
| Master data | `02-section-8-master-data.md` |
| Notifications and communication | `02-section-9-notifications-communication.md` |

Product-side files are in `docs/product/` (`overview.md`, `business-rules.md`, `compliance-notes.md`).

## What changed in this refresh [NEW]

- The audit outbox (6 files in `Scrips.BaseDbContext/Outbox/`) is new and documented in `02-section-7-audit-compliance.md`.
- The gRPC surface grew from 218 to 254 rpcs and from 9 to 10 services (QuestionBank added). 156 commits landed on master after 2026-03-04.
- Several old statements were wrong or are now out of date: `net7.0`, `Dapr.Client` 1.10.0 and 1.14.0, "SQL Server, PostgreSQL or MySQL", the 1 s / 2 s / 4 s retry, "every `AuditableEntity` gets a NewId", and the CQRS and JWT-pipeline diagrams. They are corrected or removed.
- Sections that carried a 2026-01 quality-audit score table were rewritten; the score tables are gone.
- Not refreshed: root `README.md`, `ONBOARDING.md`, `CONTRIBUTING.md`, `CLAUDE.md`, `audit-report-Scrips.Core.md` (see TD-X1 in `09-technical-debt-inventory.md`).

## How these files were checked [NEW]

Code was read from `origin/master` at 6fc2554 in a docs-only worktree. No build, no test and no service was run. Facts about other repos come from `git show`, `git ls-tree` and `git grep` on their default branches on 2026-11-15, for the 20 repos cloned under `~/scrips-repos`.
