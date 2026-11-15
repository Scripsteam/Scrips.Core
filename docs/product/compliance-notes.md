Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-04-17 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Previous text was verified against cc112c6 (2026-04-15).

# Scrips.Core — Compliance Notes (NABIDH / Malaffi / HIPAA)

**Scope:** Which controls the code in this repo supports, and where. This file does not say that any regulation is met. Every mapping to a clause is a compliance-officer decision. [NEEDS HUMAN REVIEW]
Confidence tags: `[CONFIRMED]` code read this session with `file:line`; `[NEEDS HUMAN REVIEW]` the code shows a mechanism, a person must judge the rule.
NABIDH is Dubai (DHA). Malaffi is Abu Dhabi (DOH). Both are one-way HL7 v2 sinks through MIRTH, and neither is called from this repo. [CONFIRMED: no HL7 or HIE client code here]

## 1. Audit trail [UPDATED]

**Claim:** Every entity change in a service that inherits the base classes is captured as an audit event.
`[CONFIRMED: Scrips.BaseDbContext/AuditableBaseDbContext.cs:30-56, 63-87; AuditableMultiTenantBaseDbContext.cs:41-67, 74-98]`
Two modes (`Audit:UseOutbox`, default off):
- **Inline (default):** the event is published to Dapr topic `SaveAudit` before the data save, 3 attempts (waits 1 s and 2 s), errors swallowed, payload logged after the last failure (`AuditableBaseDbContext.cs:42-55, 89-116`).
- **Outbox:** the event is stored in an `OutboxMessages` row in the same transaction as the data; a background worker publishes it (`:32-40`; `Outbox/OutboxDrainerService.cs`). A failure to stage the row fails the save.
The old text said the code retries "1 s, 2 s, 4 s"; there are two waits. [CONFIRMED]

**Claim:** The code is written to meet a NABIDH audit-trail requirement.
`[CONFIRMED: AuditableBaseDbContext.cs:58-62 comment; AuditableMultiTenantBaseDbContext.cs:69-73]` The comment says every entity change must be published before the save completes, and cites SND-331 and SND-386. It is the only NABIDH reference in the audit code. In inline mode the publish runs before the data save. In outbox mode the event is stored with the data and published later. `[NEEDS HUMAN REVIEW: whether the outbox mode meets the same requirement]`

**Claim:** Audit records include actor, tenant, IP, entity, action, old and new values.
`[CONFIRMED: Scrips.Core/Models/Audit/LogAudit.cs:3-14; AuditLoggingHelper.cs:20-23]` Actor from claim `sub` (a non-GUID value is kept as raw text; a missing value shows an empty GUID), tenant from claim `tenant`, IP from header `ipaddress`. Timestamp is server local time (`AuditEntityEntry.cs:35`). Primary keys are written unmasked (`AuditLoggingHelper.cs:58`).

**Which clause does this satisfy?** `[NEEDS HUMAN REVIEW]` HIPAA §164.312(b) is the likely match; the mapping is not decided in code.

## 2. Audit outbox: retention and PHI at rest [NEW]

- An outbox row holds the audit payload, which includes old and new field values and so can hold PHI. `[CONFIRMED: OutboxMessage.cs:34-35]`
- Published rows are deleted 7 days after publish by default (`PublishedRetentionDays`; 0 or less turns deletion off). `[CONFIRMED: AuditOutboxOptions.cs:32; OutboxDrainerService.cs:108-125]`
- Dead-lettered rows are never deleted. A daily pass at 02:00 UTC re-queues them, up to 5 cycles. `[CONFIRMED: OutboxDrainerService.cs:128-159; AuditOutboxOptions.cs:42-51]`
- After the last cycle the payload can be replaced by `[]` and the row kept, only if `Audit:ScrubTerminalPayload` is true. Default false. The option comment gives the reason: UAE Federal Law 2/2019 (health-data retention, no early destruction) and a sign-off ticket (PROD-1481). `[CONFIRMED: AuditOutboxOptions.cs:56-61; OutboxDrainerService.cs:164-178]` `[NEEDS HUMAN REVIEW: legal reading and sign-off]`
- The outbox schema script is not in this repo. `[CONFIRMED]`
- Which services run with the flag on is not in any repo. `[UNKNOWN]` Ten of 20 local service repos call the wiring method; four of those pin a Core commit that lacks two outbox fixes (see `docs/cursor/08-microservices-topology.md`). `[CONFIRMED pins; flag state not verified]`

## 3. Multi-tenant isolation [UPDATED]

**Claim:** Tenant isolation is done at the database-context level, not in query code.
`[CONFIRMED: AuditableMultiTenantBaseDbContext.cs:12]` The class extends Finbuckle's `MultiTenantDbContext`. Finbuckle filters only entities that the service marks as multi-tenant. This repo marks none. `[INFERRED from Finbuckle design; consumer entities not checked]` The old text said any entity is scoped "automatically"; that holds only for marked entities.

**Claim:** The tenant comes from a signed claim, not from request data.
`[CONFIRMED: MultitenancyConstants.cs:5; ScripsClaims.cs:5; ClaimsPrincipalExtensions.cs:11-12; AuditLoggingHelper.cs:21]` The repo does not validate tokens; the service does. `[CONFIRMED: no AddAuthentication in the repo]`

**Claim:** Slot search takes an explicit `OrganizationId`.
`[CONFIRMED: Scrips.Core/Models/Scheduling/SlotsRequest.cs:51]` Nothing here rejects an empty id. `[INFERRED]`

**Claim (new):** Internal-only rpcs take the organization from the caller. The comments say "internal gRPC, no token needed" and take the organization from the token's `orgId` claim, or else from the `organizationid` metadata header. A token and header that disagree, or neither, are refused. `[CONFIRMED comment: PatientManagement.proto:91-92, 1266-1267]` `[NEEDS HUMAN REVIEW: whether ingress blocks outside callers]`

**Which clause?** `[NEEDS HUMAN REVIEW]` HIPAA §164.312(a)(1) and §164.502(b) are likely anchors.

## 4. PHI masking in audit logs [UPDATED]

**Claim:** A property marked `[MaskValueAudit]` is replaced with `"*"` before the audit payload leaves the service.
`[CONFIRMED: AuditLoggingHelper.cs:27, 53-54, 66, 70, 77-78; MaskValueAuditAttribute.cs:13]`
**Limit:** the attribute is opt-in. No code in this repo adds it or checks that a PHI field has it. Consumers use it in Patient, Integration.Malaffi and QuestionBank (3 of 20 clones). Primary keys are not masked. `[CONFIRMED]` A PHI property without the attribute appears in clear text in the payload and in the outbox row. `[INFERRED]`

## 5. Authentication claims and access control [UPDATED]

- Claim vocabulary: `tenant`, `fullName`, `permission`, `image_url`, `ipAddress`, `exp`, `orgRoles`, `orgId`. `[CONFIRMED: ScripsClaims.cs:5-12]`
- Helper methods: `GetEmail`, `GetTenant`, `GetFullName`, `GetFirstName`, `GetSurname`, `GetPhoneNumber`, `GetUserId`, `GetImageUrl`, `GetExpiration`. `[CONFIRMED: ClaimsPrincipalExtensions.cs]`
- `CurrentUser` is set once per scope; a second set throws. `[CONFIRMED: CurrentUser.cs:39-60]` `GetUserId()` throws on a non-GUID `sub` (`:15-18`). `[CONFIRMED]`
- Permissions: three root permissions (View, Create, Update on Tenants), roles Admin and Basic. `[CONFIRMED: ScripsPermissions.cs:21-26; ScripsRoles.cs]`
- End-to-end RBAC is in the services. `[NEEDS HUMAN REVIEW]`

## 6. Sensitive fields in contracts [NEW]

`[CONFIRMED that the fields exist; NEEDS HUMAN REVIEW for each handling rule]`
- Practice proto: e-claim `password` and `userName` (`PracticeManagement.proto:220-225`); e-prescription `facilityPwd` and `clinicianPwd` (`:263-271`); one call returns the e-claim set for all enabled practices (`:76`, `:227-229`). The proto cannot protect them; transport security and caller checks are elsewhere.
- Identity proto: a `passwordResetToken` field in a user-details response (`IdentityManagement.proto:199`).
- `EmiratesIdCardData` DTO: ID number, names, dates, passport number, photo and signature bytes (`Scrips.Core/Models/Identity/EmiratesIdCardData.cs`).
- `Coding.PreferredDisplay`: display only; the comment says it must never reach claims or HIE (NABIDH, Malaffi) payloads. Nothing in this repo enforces that (`Scrips.Core/Models/Patient/Coding.cs`).
- Search and suggestion AI calls carry no organization, patient or encounter id; the comment says the consumer bypasses authorization (CRIT-002) (`MasterManagement.proto:166-174, 206-211`).

## 7. AI and ambient contracts [NEW]

`[CONFIRMED comments; NEEDS HUMAN REVIEW: DoH AI compliance mapping]`
- Consent for ambient recording fails closed (`PatientManagement.proto:100-105`).
- AI answers can carry an explanation (`reasoning`, `dataSources`, `guidelineReference`) and `clinicianReviewRequired`; unset means review required (`MasterManagement.proto:139-146, 183-184`).
- A repo grep for the DoH AI framework names finds nothing; the controls above are comments and fields, not checks. `[CONFIRMED absence]`

## 8. Encryption at rest and in transit [UNCHANGED, re-verified]

`[NEEDS HUMAN REVIEW]` No encryption code in this repo: `git grep -n 'Encrypt\|Cipher\|KeyVault' -- '*.cs'` finds nothing. `Azure.Identity` is referenced in two csproj files and used in no `.cs` file. Encryption at rest is a SQL Server and hosting concern; TLS is a gateway or mesh concern.

## 9. NABIDH and Malaffi integration surface [UPDATED]

- NABIDH: `NabidhAssigningAuthority` is a string on `PracticeSetupDetails` (`:18`) and `PracticeResponse` (`:60`). This repo does not call NABIDH. `[CONFIRMED]`
- Malaffi: no code. The word appears only in the `Coding.PreferredDisplay` comment. `[CONFIRMED: git grep -in malaffi -- '*.cs']`
- Which HIE a message goes to is the integration services' concern. `[NEEDS HUMAN REVIEW if a consumer relies on these fields]`

## 10. Breach notification, retention, minimum necessary [UNCHANGED, re-verified]

`[NEEDS HUMAN REVIEW]` Not in this repo: breach detection, PHI retention rules for the audit store, "minimum necessary" field filtering, tamper evidence on `LogAudit` (no hash or signature field, `LogAudit.cs:3-14`). The only retention rules are the outbox ones in section 2.

## Summary [UPDATED]

| Capability | Status | Evidence |
|---|---|---|
| Automatic DB change audit (inline or outbox) | `[CONFIRMED]` | `AuditableBaseDbContext.cs:30-87` |
| Audit atomic with the data (outbox mode only) | `[CONFIRMED]` | `AuditableBaseDbContext.cs:32-40` |
| Publish retry and payload log (inline mode) | `[CONFIRMED]` | `AuditableBaseDbContext.cs:89-116` |
| Dead-letter recovery, purge of published rows | `[CONFIRMED]` | `OutboxDrainerService.cs:89-178` |
| Scrub of terminal payloads | `[CONFIRMED]`, default off | `AuditOutboxOptions.cs:61` |
| PHI masking attribute (opt-in) | `[CONFIRMED]` | `AuditLoggingHelper.cs:53-54` |
| Multi-tenant context base | `[CONFIRMED]`; per-entity opt-in | `AuditableMultiTenantBaseDbContext.cs:12` |
| JWT claim model | `[CONFIRMED]` | `ScripsClaims.cs` |
| Single-assignment `CurrentUser` | `[CONFIRMED]` | `CurrentUser.cs:39-60` |
| NABIDH audit-trail alignment per code comment | `[CONFIRMED]` comment | `AuditableBaseDbContext.cs:58-62` |
| NABIDH assigning authority field | `[CONFIRMED]` | `PracticeSetupDetails.cs:18` |
| Specific HIPAA clause mapping | `[NEEDS HUMAN REVIEW]` | compliance-officer decision |
| Outbox meets the NABIDH audit comment | `[NEEDS HUMAN REVIEW]` | |
| Malaffi integration | not in this repo | |
| Encryption at rest | `[NEEDS HUMAN REVIEW]` | hosting concern |
| PHI retention for the audit store, tamper evidence | `[NEEDS HUMAN REVIEW]` | not here |
