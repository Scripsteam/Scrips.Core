Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (311 lines) was replaced by this shorter file.

# Section 9: Notifications and Communication

## 1. Scope [UPDATED]

This repo defines notification message shapes, a sender interface, an e-mail DTO and two Refit routes. It sends nothing. Scrips.Notifications owns the sending service. [CONFIRMED: no SignalR, SMTP or SMS code in the repo]

## 2. Contracts [UPDATED]

| Item | Detail | Evidence |
|---|---|---|
| `INotificationMessage` | Marker interface | `Shared/Notifications/INotificationMessage.cs` |
| `BasicNotification` | `Message`, `Label` (Information, Success, Warning, Error) | `BasicNotification.cs` |
| `JobNotification` | `Message`, `JobId`, `Progress` (decimal) | `JobNotification.cs` |
| `StatsChangedNotification` | Empty message type | `StatsChangedNotification.cs` |
| `NotificationConstants.NotificationFromServer` | Name of the method used to push messages from server to client | `NotificationConstants.cs` |
| `INotificationSender` | `BroadcastAsync`, `SendToAllAsync`, `SendToGroupAsync`, `SendToGroupsAsync`, `SendToUserAsync`, `SendToUsersAsync` (with and without excluded connection ids) | `Scrips.Core.Application/Common/Interfaces/INotificationSender.cs:3-15` |
| `IEmailSenderApi` | `POST /api/Email/Send/{emailKey}` with `EmailData` (`ToEmail`, `ToName`, `Data` dictionary) | `HttpApiClients/IEmailSenderApi.cs:8-9`, `Models/EmailSender/EmailData.cs` |
| `INotificationsApi` | `GET /api/Notifications/GetVideoUrl` (appointment id, patient id, notification type) | `HttpApiClients/INotificationsApi.cs:7-8` |

The `INotificationSender` shape (broadcast, group, user) matches a SignalR-style hub. No implementation is in this repo. [INFERRED]

## 3. Reminder profile DTOs [UPDATED]

Reminders are configured through DTOs in two folders: `Models/Scheduling/CreateReminderProfileRequest.cs`, `ReminderProfileTypeRequest.cs` (fields `Type`, `Time`, `WhenToSend`, `Number`, `CustomNote`) and `Models/Practice/ReminderResponse.cs`, `ReminderProfileType.cs`. Refit route `GET /api/Practice/ReminderList/{practiceId}` returns them (`IPracticeApi.cs:9`). [CONFIRMED] The last Refit change in this area added a video URL to the notification reminder (commit d082355, 2025-04-15). [CONFIRMED by `git log`]

## 4. Dapr topics [UPDATED]

`Topics.cs` holds 10 constants: TenantCreated, DoctorCreated, OrganizationCreated, OrganizationV1Created, OrganizationV1Updated, PracticeCreated, PracticeUpdated, PracticeActiveArchive, OrganizationSettingsCreated, OrganizationSettingsUpdated (`Topics.cs:5-17`). [CONFIRMED]
Referenced in consumers (grep `Topics.<name>` on default branches, 2026-11-15): TenantCreated in Scrips.Identity (2 files); the PracticeActiveArchive, Practice*, Organization* constants in both Scrips.Organization and Scrips.PracticeManagement. `Topics.DoctorCreated` has no match in any of the 20 local clones. [CONFIRMED by grep; direction (publish or subscribe) not checked]
The audit topic `SaveAudit` is not in `Topics.cs`; it is `OutboxTopics.SaveAudit` (`OutboxMessage.cs:123`) and a string literal in the legacy path (`AuditableBaseDbContext.cs:97`). [CONFIRMED]

## 5. Gaps [NEW]

- `StatsChangedNotification`, `JobNotification` and `INotificationSender` have no user in this repo. [CONFIRMED by `git grep`]
- `Scrips.Notifications` pins an older Core commit (7 commits behind origin/master on 2026-11-15). [CONFIRMED: submodule pointer `def46c5`]
