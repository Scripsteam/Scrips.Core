Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (832 lines) was replaced by this shorter file that lists only what the code in this repo shows.

# Section 2: Patient Management

## 1. Scope [UPDATED]

Scrips.Core holds the wire contracts for patients and encounters. The owning service is Scrips.Patient. This repo holds no patient logic. The clinical meaning of encounter fields belongs to the encounter owner session; nothing in this file adds clinical meaning. [NEEDS HUMAN REVIEW for every clinical statement below]

## 2. Files [UPDATED]

| Kind | Count | Location |
|---|---|---|
| DTOs | 20 | `Scrips.Core/Models/Patient/*.cs` |
| Enums | 2 | `Enums/Patient/EncounterStatus.cs` (Initial, Draft, Completed), `Enums/Patient/PatientComplaintCategory.cs` (ChiefComplaint, AssociatedComplaint) |
| Refit routes | 5 | `IPatientApi.cs:8, 11, 16, 22, 28` (patient by id, address list, health-insurance sponsor, corporate sponsor, guardian details) |
| gRPC service `PatientManagement` | 56 rpcs (45 in March) | `Scrips.Core/Protos/PatientManagement.proto` (1334 lines, 144 messages) |

DTO files: Coding, EditPatientResponse, EmergencyContactResponse, GetPatientFlagResponse, GuardianDto, HealthInsuranceResponse, MasterResponse, OrganizationResponse, PatientAddressListResponse, PatientBillingGroupModel, PatientContactAddressModel, PatientCorporateResponse, PatientEmergencyContactResponse, PayerDetailModel, ProviderInsuranceModel, RelationResponse, ThirdPartyAdministrator, UpdateGuardianIdenitification (sic), UpdateIdenitification (sic), UpdatePatientContactRequest. [CONFIRMED: `git ls-files`]

## 3. gRPC rpc groups [UPDATED]

Counted by name from the proto. [CONFIRMED]

- Patient identity and demographics: `GetPatientById`, `CheckPatientById`, `GetPatientNameById`, `GetPatientDetailsById`, `GetPatientIDNumberById`, `GetPatientIDNumbersByIds`, `GetPatients`, `GetPatientsBySearch`, `GetPatientExtensions`, `GetPatientPreferredModeById`, address and call-back rpcs.
- Insurance and sponsor: `GetPatientInsurances`, `GetPatientInsurance`, `UpdatePatientInsurance`, `GetPatientCorporate`.
- Encounter reads: `GetEncounterByAppointmentId` (new), `GetEncounterNumberById`, `GetEncountersBaseInfoByIds`, `GetEncounterInfoByEncounterId`, `GetEncounterByClaimId`, `GetClaimIdfromEncounterByAppointmentId`, `IsEncounterCreated`, `IsEncounterCreatedAndStarted`, `GetEncounterSignState` (new, PROD-2547), `GetLastTwoSignLockByPatientAndEncounter`, `GetPostVisitSummary`, `UpdatePatientEncounterClaim`.
- Chart data: allergies, conditions, vitals, social and family history, vaccinations, medications, prescriptions, assessment plans, documents, complaints, flags.
- Care plans and goals (new): `CreateCarePlan`, `AddCarePlanGoal`, `EvaluateObservationAgainstGoal`, `RecordGoalAchievement`.
- Ambient recording (new): `CheckAmbientRecordingConsent`, `BeginAmbientRecordingPart`, `CompleteAmbientRecordingPart`, `RecordAmbientTranscriptCorrections`.
- Lookup: `FindPatientsByPhone` (new), `CountTotalNumberOfUsersById`.

## 4. Contract rules written in the proto comments [NEW]

These are statements in comments of `PatientManagement.proto`. This repo does not enforce them; the service does. [CONFIRMED that the comments exist; INFERRED that the service follows them]

- `CheckAmbientRecordingConsent` fails closed. Any error means denied, and `AMBIENT_CONSENT_UNSPECIFIED` (value 0) must be read as denied. [CONFIRMED: `PatientManagement.proto:100-105, 164-166`] The response "carries no names and no PHI". `BeginAmbientRecordingPartRequest.consentNotRequired = 7` is true when the caller runs with `AmbientScribe:Consent:Required=false` (`PatientManagement.proto:183-185`; commit 60fbf77). The comment says `BeginAmbientRecordingPart` and `CompleteAmbientRecordingPart` are "declared here only; they are implemented later". [CONFIRMED: comment at `PatientManagement.proto:103-105`; whether they are implemented now is not verified]
- `RecordAmbientTranscriptCorrections`: internal, Video is the only caller, per-segment results, a malformed request is the only rpc error. [CONFIRMED: comment `PatientManagement.proto:110-113`]
- `FindPatientsByPhone`: exact match on a normalized number, organization-scoped, returns patient ids only, called by Scrips.Workflows only; more than one id means a shared number and the caller must not guess. [CONFIRMED: comment `PatientManagement.proto:115-121`]
- `CreateCarePlan`: instantiates a care plan from a plan definition's goals; tenant-scoped and idempotent. Additive fields since: sub-goals, targets, `reconcileGoals`, `keepSourceNodeIds`, `enrolledByUserId` (field 14, commit 4268a91). [CONFIRMED: comment at `PatientManagement.proto:82-87`, commit titles]
- `AddCarePlanGoal`: adds one goal when a care-model run reaches its step; idempotent on (carePlanId, goal.sourceNodeId) among live goals; internal gRPC with no token, organization taken from the caller's token claim or the `organizationid` metadata. [CONFIRMED: `PatientManagement.proto:89-99`]
- `EvaluateObservationAgainstGoal`: rule-based, no AI; codes only, never a value; fails closed ("no-data", never "met"); off when `CarePlanTargets:Enabled` is false; calls per observation per organization are capped and audited. [CONFIRMED: `PatientManagement.proto:122-131`]
- `RecordGoalAchievement`: Scrips.Patient owns a goal's target and `achievementStatus`; the service re-derives the status from its own target and stores it only when they agree; a no-data never overwrites a stored status; every call is audited; the "derive" mode compares every live goal for a LOINC code, at most 20, in care-plan-id then goal-id order. [CONFIRMED: `PatientManagement.proto:132-152`]
- A goal set by a physician is never retired by `reconcileGoals` (commit ce1ac67). [CONFIRMED by commit title]

## 5. Shared DTO change [NEW]

`Models/Patient/Coding.cs` has a new nullable `PreferredDisplay` (PROD-1656). Its comment says it is presentation-only, must never replace the official display of an ICD-10 or SNOMED coding, and must never reach eClaims or HIE (NABIDH/Malaffi) payloads. [CONFIRMED: `Coding.cs`, file diff since 2026-03-04] Nothing in this repo enforces that last rule. [INFERRED]

## 6. Gaps and risks [NEW]

- `GetPatientPackages` was added (commit a9502cc) and reverted (commit 2862bdf) on 2026-09-10/11; it is not in the proto now. [CONFIRMED: rpc list above has no such name]
- PHI is in these messages (names, ids, contact data). The protos have no field-level sensitivity option; the only PHI markers are free-text comments (for example `PatientManagement.proto:161, 1245`). [CONFIRMED by `grep -in 'sensitive\|phi'`; NEEDS HUMAN REVIEW for the compliance meaning]
- Two DTO files have misspelled names (`UpdateGuardianIdenitification`, `UpdateIdenitification`). Renaming is a breaking change for every consumer. [CONFIRMED names; INFERRED impact]
