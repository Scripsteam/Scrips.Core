Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (328 lines) was replaced by this shorter file.

# Section 5: Provider, Practice and Organization

## 1. Scope [UPDATED]

Wire contracts for practices, practitioners, staff, rooms, devices, appointment profiles and organizations. Owners: Scrips.PracticeManagement (practice), Scrips.Provider (provider), Scrips.Organization (organization). This repo has no logic for any of them. [CONFIRMED: DTOs and protos only]

## 2. Files [UPDATED]

| Kind | Count | Location |
|---|---|---|
| Practice DTOs | 34 | `Scrips.Core/Models/Practice/*.cs` (includes `DeviceSlots.cs`, new) |
| Provider DTOs | 12 | `Scrips.Core/Models/Provider/*.cs` |
| Organization DTOs | 4 | `OrganizationDto`, `OrganizationInfo`, `OrganizationSettingsDto`, `OrganizationV1Dto` in `Models/Organization` |
| Refit routes | 18 practice, 5 provider, 4 organization | `IPracticeApi.cs`, `IProviderApi.cs`, `IOrganizationApi.cs` |
| gRPC `PracticeManagement` | 51 rpcs (42 in March) | `Protos/PracticeManagement.proto` (663 lines) |
| gRPC `ProviderManagement` | 5 rpcs (unchanged) | `Protos/ProviderManagement.proto` (67 lines) |
| gRPC `OrganizationConfiguration` | 14 rpcs (13 in March) | `Protos/OrganizationManagement.proto` (181 lines; no `package` line) |

## 3. Refit routes [UPDATED]

- Practice (`IPracticeApi`): `Practice/ReminderList/{practiceId}`, `Doctor/AppointmentProfileList`, `Practice/PracticeSetupList/{organizationId}`, `Practice/PracticeSetupForPrimary/{organizationId}`, `Practice/PracticeSetupDetails/{practiceId}`, `Doctor/CalendarSlots3`, `Doctor/CalendarProviders`, `Doctor/DoctorSetupList`, `Staff/StaffList`, `Doctor/GetPracticeByProviderId/{id}`, `Doctor/PractitionerRoles/{organizationId}`, `Doctor/CalendarSlots2`, `Doctor/CalendarProviders2`, `Doctor/GetPractitionerRoleDetails/{id}`, `Practice/EditPracticeExamRoom/{practiceId}`, `Doctor/GetPracticeId/{userId}`, `Staff/GetStaffId/{userId}`, `Doctor/GetPractitionerRoleForUser/{userId}` (all under `/api/`). [CONFIRMED: `IPracticeApi.cs:9-91`]
- Provider (`IProviderApi`): `ProfessionalDetails/GetProviderReasonForVisits/{providerId}`, `ProfessionalDetails/{providerId}`, `Provider/{id}`, `Provider/Id/{id}`, `Provider`. [CONFIRMED]
- Organization (`IOrganizationApi`): `v1/Organization/{id}`, `v1/OrganizationSettings/{id}`, `POST v1/Organization/list`, `Organization/UpdateAndGetOrganization/{userId}`. [CONFIRMED]

## 4. gRPC `PracticeManagement` [UPDATED]

Groups of the 51 rpcs, by name. [CONFIRMED]

- Practice, room and staff reads: `GetPracticeById`, `GetPracticeNameById`, `GetPractices`, `GetPracticesList`, `GetPracticesByIds`, `GetPracticeTrnNumbersByIds`, `GetRoomById`, `GetRooms`, `GetRoomsByPractice` (new), `GetStaffByUserId`, `GetStaffsByUserIds`, `GetStaffsByIds`.
- Doctor and provider: `GetProviderById`, `GetProviders`, `GetProvidersList`, `GetProviderBaseInfos`, `GetDoctorByUserId`, `GetDoctorIdByUserId` (new), `GetDoctorsByIds`, `GetDoctorSpecialities`, `GetDoctorSpecialityCodesByUserId` (new, PROD-3442), `UpdateSpecialities`, `IsActivePractitionerOfOrganization` (new, video join, PR #126), `GetDoctorDistanceDetailsById`, `RemovePracticePatientDistance`.
- Resources (devices) (new, PROD-2007/2008): `GetResourceById`, `GetResources`, `GetResourceAppointmentProfiles`, `GetResourceOverbookCalendarSettingsBatch`, `GetActorPresence`.
- Presence and calendar: `GetPractitionerPresence`, `GetPractitionerPresence2`, `GetPractitionerOverbookCalendarSettings`, `GetPractitionerOverbookCalendarSettingsBatch`, `GetAppointmentProfileByProviderId`, `GetAppointmentProfileDuration`, `GetLocationBasedOnAppointmentProfile`, `DeactivateLocations`, `RemoveAppointmentProfilesByBillingProfileId`, `UpdateRFVInAppoinmentProfile` (sic).
- Cashier shift: `UpdateCashierShiftBalance`, `GetCashierShiftStatusByUserId`, `GetCashierShiftOpenByUserId`.
- Credentials (see risks): `GetProviderPracticeEPrescriptionCredentials`, `GetProviderPracticeEPrescriptionCredentialsList`, `GetPracticeEClaimCredential`, `GetAllEnabledEClaimCredentials`.
- Other: `CheckOrgAdminOnboardingById`, `CheckIfQuestionnaireAnsweredByAppointmentId`, `CountTotalNumberOfUsersById`, `CheckPracticeAndProviderProfile`.

### Device slots wire models [NEW]
`Models/Practice/DeviceSlots.cs` (commit 1a14bf0, PROD-2181; `AppointmentProfileId` added by PROD-2456) defines `DeviceSlotsRequest`, `DeviceSlotsResponse`, `DeviceSlotResponse`, `DeviceSlotsAppointmentProfile` and `DeviceWorkingWindow`. The file comments say: this is deliberately not `ProviderSearchRequest`; `WeedayId` uses Mon=1 to Sun=7; the practitioner path emits `(int)DayOfWeek` (Sunday = 0) because an identity cast hides the mismatch. [CONFIRMED: `DeviceSlots.cs` doc comments, diff since 2026-03-04] Resource appointment profiles carry `billingProfileId` (field 10, PROD-2154) and `serviceId` (PROD-2165). [CONFIRMED by commit titles 390e0bf, 1256c1b]

### Room scheduling [NEW]
`GetRoomsByPractice` and room `practiceId` (PR #121); closed rooms on request and `RoomResponse.IsActive` (PR #124). [CONFIRMED by commit titles and `git diff` of `Models/Scheduling/RoomResponse.cs`]

## 5. gRPC `ProviderManagement` [UNCHANGED, re-verified]

`GetProviderRFV`, `GetProviderRFVs`, `GetPractitionerLicenceNo`, `GetPractitionerLicenceNoByIds`, `GetSpecialityNameByIds`. [CONFIRMED: rpc list read 2026-11-15] A field `expirationDate` was added to the licence-number response messages (`ProviderManagement.proto:55, 64`; commit 3d59822) and `DoctorSignatureUrl` to `ProfessionalDetailRequestModel` (commit 73dd07d, PROD-1232), so the service list is unchanged but two messages changed. [CONFIRMED by commit titles]

## 6. gRPC `OrganizationConfiguration` [UPDATED]

14 rpcs: `GetOrganizationImage`, `GetOrganizationDetails`, `GetOrganizationDetail`, `GetOrganizationById`, `GetOrganizationTimeZone`, `UpdateAndGetOrganization`, `GetOrganizationUserInfo`, `CheckOrganizationHasPatientCommunication`, `GetDepartmentIdsByTenantId`, `GetV1OrganizationById`, `GetV1Organizations`, `GetBasicV1OrganizationByIds`, `GetOrganizationResourceBasedIntegration`, `GetOrganizationClinicServices` (new, PR #109). [CONFIRMED]
`GetOrganizationClinicServicesResponse` has five bools: `settingsFound`, `sendsTestsToLabs`, `physiciansMayChangeLab`, `booksDevices`, `documentIntake`. With no settings row, every switch holds its column default (`documentIntake` true, the others false); a caller that must fail closed treats an rpc error as "off". [CONFIRMED: `OrganizationManagement.proto:166-181`]

## 7. NABIDH data field [UPDATED]

`NabidhAssigningAuthority` is on `PracticeSetupDetails` (`:18`) and `PracticeResponse` (`:60`). It is a string that downstream services use; this repo does not call NABIDH. [CONFIRMED]

## 8. Risks [NEW]

- `GetPracticeEClaimCredentialResponse` carries `password`, `userName`, `licenseNumber`, `organizationId` as plain strings (`PracticeManagement.proto:220-225`). `GetProviderPracticeEPrescriptionCredentialsResponse` carries `facilityLogin`, `facilityPwd`, `clinicianLogin`, `clinicianPwd` (`:263-271`). The proto cannot protect them; transport security and caller authorization live elsewhere. [CONFIRMED fields; NEEDS HUMAN REVIEW]
- `OrganizationManagement.proto` has no `package` line and names its service `OrganizationConfiguration`, unlike the other nine services. [CONFIRMED]
- A service named `PersonManagment` (sic) exists in `PersonManagement.proto`; renaming breaks consumers. [CONFIRMED]
