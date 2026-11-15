Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (275 lines) was replaced by this shorter file.

# Section 8: Master Data

## 1. Scope [UPDATED]

Wire contracts for reference data (value sets, codes, locations, languages, vocabularies). The owning service is Scrips.Master. This repo holds no reference data and no lookup logic. [CONFIRMED: no data files; `IMasterApi` and `MasterManagement` are interfaces and proto only]

## 2. Files [UPDATED]

| Kind | Count | Location |
|---|---|---|
| Refit routes (lookups) | 4 | `IMasterApi.cs:9-15`: `GET /api/Master/Gender`, `IdentityType`, `MaritalStatus`, `OwnerType`. Each returns `List<IdNamePair>` and takes the `Authorization` header as a string parameter. |
| Shared DTO | 1 | `Models/IdNamePair.cs` (`Guid Id`, `string? Name`) |
| Patient-side master DTOs | 1 | `Models/Patient/MasterResponse.cs` |
| gRPC `MasterManagement` | 52 rpcs (45 in March), 930 lines, 91 messages | `Protos/MasterManagement.proto` |

## 3. gRPC `MasterManagement` lookup rpcs [UPDATED]

45 lookup rpcs existed in March; all 45 are still present (the rpc-name diff against 4d8e816 shows only additions). [CONFIRMED] Groups:

- Value sets by id or code: speciality (`GetSpecialityById`, `...ByCode`, `...ByIds`, `GetSpecialitiesByName`, `...ByIds`, `...ByCodes`), service category and type (`GetServiceCategoryByCode(s)`, `GetServiceTypeByCode(s)`), appointment status, type, cancellation reason and reason (`GetAppointmentStatusByCode`, `GetAppointmentStatusList`, `GetAppointmentTypeByCode`, `GetAppointmentTypesList`, `GetAppointmentTypesByCodes`, `GetAppointmentCancellationByCode`, `GetAppointmentReasonByCode`, `GetAppointmentReasonsByCodes`), `GetPracticeCodeList`.
- People and demographics: `GetGenderById`, `GetAllGenders`, `GetLanguageById`, `GetAllLanguages`, `GetMaritalStatusById`, `GetAllMaritalStatus`, `GetRelationById`, `GetRelationionList` (sic), `GetNationalitySystemByCode`, `GetCountryIsoByCountryName`, `GetIdentityVerificationById`, `GetDegreeById`, `GetUniverisytById` (sic), `GetLicenseAuthorityById`.
- Insurance and locations: `GetInsuranceById`, `GetLocations`, `GetLocation`, `GetAllLocations`, `GetAllDepartmentList`.
- Codes and catalogue: `GetServiceByCodeAndSystem`, `GetServiceByCode`, `GetMedicationByCode`, `GetSpecialServiceListByCode`, `GetHCPCSListByCode`, `GetMedicationRouteByDisplay`, `GetVaccinationTimelineData`.
- AI and vocabularies (7, new): see `02-section-4-clinical-ai.md`.

## 4. Message additions since March [NEW]

Commit titles show additive changes to the shared contract for the ambient encounter: order catalogue codes, procedures and structured findings (PROD-3179, PROD-3180; commit 6f349b9), code-match fields `codeStatus` with values matched, ambiguous, unmatched, unchecked (PROD-2611, PROD-2687; commits 7648316, cba5ab8), encounter-from-transcript fields (commit a7ef85b), and `ClinicalAlert.recommendedAction = 5` (PROD-2599; commit 474a6ba). [CONFIRMED by commit titles; message bodies not re-read field by field] The exact string values are in `Scrips.Core/Shared/Ambient/AmbientWireValues.cs` (see section 4).
Code systems named on the wire are the service's concern. Nothing here chooses a code system. [NEEDS HUMAN REVIEW]

## 5. Rules and risks [NEW]

- Many value-set rpcs return the same envelope (`GetValueSetResponse` or `GetValuesSetResponse`). Callers must know the code system per rpc from the service, not from this proto. [INFERRED from rpc signatures]
- Spelling errors live in rpc names (`GetUniverisytById`, `GetRelationionList`). They are part of the wire contract now. [CONFIRMED]
- `GetPatientPackages` was added and reverted in September (see section 2). [CONFIRMED]
