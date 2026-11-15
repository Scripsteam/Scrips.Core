Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (809 lines) was replaced by this shorter file that lists only what the code in this repo shows.

# Section 1: Appointments and Scheduling

## 1. Scope [UPDATED]

Scrips.Core holds the scheduling wire contracts. It holds no scheduling logic, no slot engine, no state machine. The owning service is Scrips.AppointmentScheduling. [CONFIRMED: no handler code under `Scrips.Core/Models/Scheduling`; consumers import via submodule]

## 2. Files [UPDATED]

| Kind | Count | Location |
|---|---|---|
| DTOs | 40 | `Scrips.Core/Models/Scheduling/*.cs` |
| Enums | 9 | `Scrips.Core/Enums/Scheduling/*.cs` |
| Refit routes | 5 | `Scrips.Core/HttpApiClients/ISchedulingApi.cs:8-26` |
| gRPC service `AppointmentManagement` | 26 rpcs | `Scrips.Core/Protos/AppointmentManagement.proto` (594 lines) |

Duplicate type names exist across folders: `AppointmentStatus`, `AppointmentType`, `ParticipantRequired`, `ParticipationStatus`, `ParticipationType`, `SlotStatus`, `AppointmentCancellationReason` each appear in both `Models/Scheduling` and `Enums/Scheduling`. [CONFIRMED: `git ls-files`] A consumer must check which namespace it imports. [INFERRED]

## 3. Enums [UPDATED]

| Enum | Values | Evidence |
|---|---|---|
| `AppointmentStatus` | Proposed, Pending, Booked, Arrived, Fulfilled, Cancelled, NoShow, EnteredInError, CheckedIn, Waitlist, Confirmed, Seen (12) | `Enums/Scheduling/AppointmentStatus.cs` |
| `AppointmentType` | FOLLOWUP, ROUTINE, WALKIN | `Enums/Scheduling/AppointmentType.cs` |
| `SlotStatus` | Busy, Free, BusyUnavailable, BusyTentative, EnteredInError | `Enums/Scheduling/SlotStatus.cs` |
| `ParticipationStatus` | Accepted, Declined, Tentative, NeedsAction | `Enums/Scheduling/ParticipationStatus.cs` |
| `ParticipantRequired` | Required, Optional, InformationOnly | `Enums/Scheduling/ParticipantRequired.cs` |
| `ParticipationType` | ADM, ATND, CALLBCK, CON, DIS, ESC, REF, SPRF, PPRF, PART, Translator, Emergency | `Enums/Scheduling/ParticipationType.cs` |
| `QuestionnaireStatus` | Send, Pending, Answered | `Enums/Scheduling/QuestionnaireStatus.cs` |
| `EnumInvoiceStatus` | Draft, Issued, Balanced, Canceled, Claimed, Reclaimed | `Enums/Scheduling/InvoiceStatusEnum.cs` |
| `AppointmentCancellationReason` | 45 short codes (Pat, PatCrs, Prov, Maint, Other, and others) | `Enums/Scheduling/AppointmentCancellationReason.cs` |

All of these carry `[JsonConverter(typeof(JsonStringEnumConverter))]`. [CONFIRMED]
The code values of `ParticipationType` look like HL7 v3 participation codes. [INFERRED]

## 4. HTTP routes (Refit `ISchedulingApi`) [UPDATED]

`POST /api/Appointment/Slots`, `POST /api/Appointment/Slots2`, `GET /api/Appointment/PatientFlag`, `GET /api/Appointment/GetById/{id}`, `POST /api/Appointment`. [CONFIRMED: `ISchedulingApi.cs:8, 14, 20, 23, 26`] No file in the `HttpApiClients` folder has changed since 2025-04-15. [CONFIRMED: `git log -1 -- Scrips.Core/HttpApiClients`]

## 5. gRPC contract `AppointmentManagement` [UPDATED]

26 rpcs (23 in March). [CONFIRMED]

- Read: `GetAppointmentById`, `GetAppointmentBasicInfo`, `GetAppointmentStatusById`, `GetAppointmentDateById`, `GetAppointmentCustomNote`, `GetAppointmentParticipants`, `GetAppointmentsListByPatientIds`, `GetAppointmentsListByProvider`, `GetAppointmentListForProvider`, `GetAppointmentsSpecialityByIds`, `GetAppointmentSponsorTypeById`, `GetFlagByPatientId`.
- Availability: `GetPractitionerAvailability`, `GetPractitionersAvailability`, `GetResourcesAvailability` (new), `GetDynamicSlots`, `GetDashboardSlotForProviders`.
- Write: `UpdateAppointmentStatus`, `UpdateAppointmentStatusRealtime`, `UpdateQuestionnireStatus` (sic), `CreateFollowUpRequests`, `RealTimeUpdate`.
- Schedules per actor: `CreateScheduleForActor`, `UpdateScheduleForActor`, `ArchiveSchedulesForActor` (new; commit 12c8098, 2026-10-02). `V2Schedule` has `actorType` (HL7 R4 `Schedule.actor` target; empty means Practitioner). [CONFIRMED: `AppointmentManagement.proto:178-190`]
- Group video: `GetGroupSessionForVideo` (new; commits f63d897 and f4209bc, 2026-11-13). Its comment says it is off when `GroupSessions:Enabled` is false. [CONFIRMED: `AppointmentManagement.proto:56-57`]

### New fields since March [NEW]
- `PractitionerParticipant` and a repeated `practitioners` field (joint-slice 1: every practitioner, lead first, declined ones included). [CONFIRMED: `AppointmentManagement.proto:154, 161, 238`; commit 82f81f6]
- `groupSessionId` and `groupSessionStatus` on participants and basic-info responses. [CONFIRMED: `AppointmentManagement.proto:155-156, 239`; commits c86d0a2, def46c5]
- `FollowUpRequest.bookedAppointmentId` (commit 5d28a54). [CONFIRMED by commit title; field not re-read]
- Wire DTOs: `CreatedOn` (lead-time measure) on `ListAppointmentResponseBase` and `AppointmentResponse` (commit 762016f); `PreviousRoomId`, `PreviousRoom`, `RoomChangedAt`, `RoomChangedById` and `RoomResponse.IsActive` (commit cc5dcd2, room scheduling). [CONFIRMED in `git diff` of `Models/Scheduling`]
- `SlotsRequest` has `OrganizationId`. [CONFIRMED: `Models/Scheduling/SlotsRequest.cs`, listed in the diff since 2026-03-04]
- Device slots: see `02-section-5-provider-practice-org.md`.

## 6. Rules defined by these contracts [UPDATED]

- Tenant scope on slot search is an explicit request field, not a claim. [CONFIRMED: `SlotsRequest.OrganizationId`] Nothing in this repo rejects an empty `Guid`. [INFERRED: no validator for `SlotsRequest` found by `git grep`]
- The proto for `GetAppointmentParticipants` is "copied from Scheduling" per commit def46c5. The proto in this repo and the service can drift. [INFERRED]

## 7. Not verified [NEW]

How Scrips.AppointmentScheduling implements any rpc. Which consumers call which rpc. No service code was read for this file.
