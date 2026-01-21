# Appointment & Scheduling Management - Complete Documentation

## OVERVIEW

**Purpose:** Manage appointment booking, scheduling, slot availability, patient check-in, and cancellation workflows for healthcare encounters

**Key Entities:** Appointment, Slot, AppointmentProfile, FlagResponse, ReminderProfile, Recurring

**Key Workflows:** Appointment booking, check-in process, cancellation, recurring appointments, patient flag management

**PHI Scope:** YES - Contains appointment date/time (linked to patient), patient flags (allergies, conditions), provider information

---

## ENTITIES

### AppointmentResponse

**Location:** Models/Scheduling/AppointmentResponse.cs:9

**Purpose:** Represents a scheduled healthcare encounter with patient, provider, time slot, billing, and status lifecycle

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| Id | Guid | Unique appointment identifier | NO |
| Status | AppointmentStatus | Booked/Arrived/Fulfilled/Cancelled | NO |
| OrganizationId | Guid | Multi-tenant organization identifier | NO |
| PracticeId | Guid | Healthcare facility location | NO |
| RoomId | Guid | Exam room assignment | NO |
| Start | DateTime? | Appointment start time | **YES** |
| End | DateTime? | Appointment end time | **YES** |
| Description | string | Shown on appointment list/calendar | NO |
| SupportingInformation | string | Additional appointment context | NO |
| Priority | int | Urgency ranking | NO |
| CancelationReason | AppointmentCancellationReason | Coded cancellation reason | NO |
| ServiceCategory | ServiceCategory | Broad service type (consultation, procedure) | NO |
| ServiceType | ServiceType | Specific service being performed | NO |
| Specialty | Speciality | Required provider specialty | NO |
| ReasonCode | ReasonCode | Clinical reason for visit | **YES** |
| Comment | string | Additional internal notes | NO |
| PatientInstruction | string | Patient-facing instructions | NO |
| Participant | ICollection\<ParticipantResponse\> | Patient, providers, related persons | **YES** |
| Slot | SlotResponse | Linked time slot | NO |
| FlagList | IList\<FlagResponse\> | Patient alerts (allergies, VIP) | **YES** |
| AppointmentProfile | CreateAppointmentProfileRequest | Appointment type configuration | NO |
| ReminderProfile | CreateReminderProfileRequest | Reminder settings | NO |
| Sponsor | Sponsor | Insurance/corporate payer | **YES** |
| PolicyNo | string | Insurance policy number | **YES** |
| BillingTotal | BillingTotal | Calculated charges | NO |
| Invoice | AppointmentInvoiceModel | Linked invoice | NO |
| Recurring | RecurringDto | Recurring appointment series | NO |
| Created | DateTime | Audit timestamp | NO |
| UpdatedBy | Guid | Audit user | NO |

**Relationships:**
- Patient (via Participant): One appointment has one patient participant
- Provider (via Participant): One appointment has one or more provider participants  
- Practice: Many appointments belong to one practice
- Room: Many appointments to one room
- Slot: One-to-one relationship with slot
- Invoice: One-to-one relationship with billing invoice

---

### SlotResponse

**Location:** Models/Scheduling/SlotResponse.cs:6

**Purpose:** Available time block for booking appointments with provider schedule

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| Id | Guid | Unique slot identifier | NO |
| AppointmentType | AppointmentType | Type of appointment that can be booked | NO |
| ScheduleId | Guid | Link to provider schedule | NO |
| Status | SlotStatus | Free/Busy/Tentative/OfficeHours | NO |
| Start | DateTime | Slot start time | NO |
| End | DateTime | Slot end time | NO |
| Comment | string | Constraints or notes | NO |
| Overbooked | bool | Already exceeded capacity | NO |
| PracticeId | Guid | Practice location | NO |
| PracticeName | string | Practice display name | NO |

**Relationships:**
- Schedule: Many slots to one provider schedule
- Practice: Many slots to one practice
- Appointment: One-to-one when booked

---

### FlagResponse

**Location:** Models/Scheduling/FlagResponse.cs (referenced in AppointmentResponse)

**Purpose:** Patient alerts and warnings for clinical safety (allergies, VIP status, special needs)

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| (Structure not visible, inferred) | | | |
| FlagType | enum/string | Allergy/VIP/SpecialNeeds | **YES** |
| Description | string | Flag details | **YES** |
| Severity | enum/string | Critical/High/Medium/Low | **YES** |

**Relationships:**
- Patient: Many flags to one patient
- Appointment: Flags displayed during appointment

**Clinical Safety:** CRITICAL - Flags must display during check-in to prevent adverse events (e.g., allergy alerts)

---

### AppointmentProfileResponse

**Location:** Models/Practice/AppointmentProfileResponse.cs (referenced)

**Purpose:** Provider-specific appointment types with durations and service codes

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| (Structure referenced) | | | |
| AppointmentTypeName | string | New Patient/Follow-up/Procedure | NO |
| DurationMinutes | int | Standard appointment length | NO |
| ServiceCodes | List | Billable service codes | NO |

**Relationships:**
- Provider: Many profiles to one provider
- Appointment: Used as template for booking

---

### RecurringDto

**Location:** Models/Scheduling/RecurringDto.cs (referenced in AppointmentResponse.cs:258)

**Purpose:** Recurring appointment series configuration

**Fields:**

| Field | Type | Purpose | PHI? |
|-------|------|---------|------|
| (Structure not fully visible) | | | |
| RecurrencePattern | string | Daily/Weekly/Monthly | NO |
| RecurrenceInterval | int | Every N days/weeks | NO |
| SeriesEndDate | DateTime | When series stops | NO |

**Relationships:**
- Appointments: One recurring pattern to many appointments

---

## WORKFLOWS

### Workflow 1: Appointment Booking

**Entry Point:** ISchedulingApi.cs:8, IPracticeApi.cs:28

**Trigger:** Patient or staff initiates appointment booking

**Steps:**

1. **Slot Search** - POST /api/Appointment/Slots
   - Input: Date range, provider ID or specialty, practice
   - Output: List\<SlotResponse\> with available times
   - File: ISchedulingApi.cs:8,14

2. **Patient Selection** - GET /api/Patients/{id}
   - Input: Patient ID
   - Output: EditPatientResponse with demographics
   - File: IPatientApi.cs:8

3. **Sponsor Verification** - GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId}
   - Input: Patient ID, Sponsor ID
   - Output: HealthInsuranceResponse with eligibility
   - File: IPatientApi.cs:16

4. **Price Calculation** - POST /api/Billing/Agreements/CalculatePrice
   - Input: CalculatePriceRequest (service codes, sponsor, provider)
   - Output: FeeSummaryResponse with sponsor/patient split
   - File: IBillingApi.cs:14

5. **Appointment Creation** - POST /api/Appointment
   - Input: AppointmentRequest with slot, patient, sponsor
   - Output: AppointmentResponse
   - File: ISchedulingApi.cs:26

6. **Confirmation Email** - POST /api/Email/Send/{emailKey}
   - Input: EmailData with appointment details
   - Output: void (fire-and-forget)
   - File: IEmailSenderApi.cs:8

**Error Handling:**
- Slot no longer available: Return 409 Conflict, re-search slots
- Insurance not eligible: Return 400 Bad Request, prompt for sponsor change
- Price calculation failure: Blocks booking, return 500 Internal Server Error
- Email service down: Appointment created but no confirmation sent (silent failure)

**Healthcare Compliance:**
- **PHI Accessed:** Appointment date/time, patient name, insurance details
- **Audit Logged:** YES - Appointment creation logged via AuditableBaseDbContext.cs:25
- **Retention:** 10 years (linked to patient medical record)

---

### Workflow 2: Patient Check-In

**Entry Point:** ISchedulingApi.cs:23 (GetById), ISchedulingApi.cs:20 (PatientFlag)

**Trigger:** Patient arrives for appointment

**Steps:**

1. **Load Appointment** - GET /api/Appointment/GetById/{id}
   - Input: Appointment ID
   - Output: AppointmentResponse with full details
   - File: ISchedulingApi.cs:23

2. **Display Patient Flags** - GET /api/Appointment/PatientFlag?patientId={id}
   - Input: Patient ID
   - Output: List\<FlagResponse\> with allergies, VIP status, special needs
   - File: ISchedulingApi.cs:20
   - **CRITICAL:** Flags must display to prevent adverse events

3. **Verify Insurance** - GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId}
   - Input: Patient ID, Sponsor ID
   - Output: HealthInsuranceResponse
   - File: IPatientApi.cs:16

4. **Check Patient Balance** - GET /api/Billing/Payment/AvailableBalance?patientId={id}
   - Input: Patient ID
   - Output: AvailableBalanceResponse
   - File: IBillingApi.cs:29

5. **Update Appointment Status** - Booked → Arrived
   - Status change logged in audit trail

6. **Notify Provider** - Real-time notification
   - File: INotificationsApi.cs (SignalR)

**Error Handling:**
- Appointment not found: Return 404 Not Found
- Flags service down: CRITICAL RISK - Check-in proceeds without allergy alerts
- Insurance verification fails: Allow check-in, flag for billing follow-up
- Notification service down: Provider not alerted (delays care)

**Healthcare Compliance:**
- **PHI Accessed:** Appointment details, patient flags (allergies, conditions), insurance
- **Audit Logged:** YES - Status change to Arrived logged
- **Clinical Safety:** CRITICAL - Patient flags must display to prevent allergic reactions/adverse events

---

### Workflow 3: Appointment Cancellation

**Entry Point:** ISchedulingApi.cs (implied PATCH/DELETE)

**Trigger:** Patient or staff cancels appointment

**Steps:**

1. **Load Appointment** - GET /api/Appointment/GetById/{id}
   - Verify appointment exists and can be cancelled

2. **Record Cancellation Reason** - AppointmentCancellationReason enum
   - Patient request, no-show, emergency, etc.
   - File: AppointmentResponse.cs:67

3. **Update Status** - Booked → Cancelled
   - Status change logged

4. **Release Slot** - Slot status changes from Busy → Free
   - Slot becomes available for rebooking

5. **Invoice Adjustment** - POST /api/Billing/Invoice (void or partial charge)
   - Cancellation fee logic depends on timing
   - File: IBillingApi.cs:26

6. **Notification** - POST /api/Email/Send/{emailKey}
   - Cancellation confirmation to patient
   - File: IEmailSenderApi.cs:8

**Error Handling:**
- Appointment already fulfilled/completed: Cannot cancel, return 400 Bad Request
- Invoice adjustment fails: Appointment cancelled but billing issue flagged
- Email notification fails: Cancelled but no confirmation sent

**Healthcare Compliance:**
- **PHI Accessed:** Appointment details
- **Audit Logged:** YES - Status change and cancellation reason logged
- **Retention:** Cancellation history retained with appointment record (10 years)

---

### Workflow 4: Recurring Appointment Series

**Entry Point:** IPracticeApi.cs:28 (bulk slot reservation)

**Trigger:** Create recurring appointment pattern (e.g., weekly physical therapy)

**Steps:**

1. **Define Recurrence Pattern** - RecurringDto
   - Pattern: Daily/Weekly/Monthly
   - Interval: Every N units
   - End date or occurrence count
   - File: AppointmentResponse.cs:258

2. **Bulk Slot Reservation** - POST /api/Doctor/CalendarSlots3
   - Reserve multiple slots matching pattern
   - File: IPracticeApi.cs:28

3. **Create Appointment Series** - Multiple appointment records
   - All linked via Recurring property
   - Each has individual status lifecycle

4. **Series Modifications:**
   - Cancel one: Affects only that instance
   - Cancel series: Updates all future appointments
   - Reschedule one: Breaks from series pattern

**Error Handling:**
- No available slots for pattern: Partial booking or fail entire series
- Series conflict: Provider schedule change invalidates future slots

**Healthcare Compliance:**
- **PHI Accessed:** Recurring appointment dates/times
- **Audit Logged:** YES - Each appointment creation logged individually

---

## INTEGRATIONS

(Brief - details in 06b-integrations-details.md)

**Scheduling Service (ISchedulingApi.cs:6-31):**
- Slot search, appointment CRUD, patient flags
- 5 endpoints: Slots (POST), PatientFlag (GET), GetById (GET), Search (POST)
- Purpose: Core appointment management

**Practice Service (IPracticeApi.cs:7-96):**
- Provider schedules, calendar slots, availability
- Key endpoints: CalendarSlots3 (POST), PracticeSetupDetails (GET)
- Purpose: Provider availability and slot generation

**Billing Service (IBillingApi.cs:6-45):**
- Price calculation, invoice linkage
- Key endpoint: CalculatePrice (POST)
- Purpose: Appointment service pricing

**Patient Service (IPatientApi.cs:6-30):**
- Demographics, insurance verification
- Key endpoints: GetPatient (GET), HealthInsuranceSponsor (GET)
- Purpose: Patient and sponsor lookup

**Email Sender Service (IEmailSenderApi.cs:6-9):**
- Appointment confirmations and reminders
- Endpoint: Send (POST)
- Purpose: Patient notifications

**Notifications Service (INotificationsApi.cs:5-8):**
- Real-time provider alerts, video URLs for telehealth
- Endpoint: GetVideoUrl (GET)
- Purpose: Real-time communication

---

## COMPLIANCE SUMMARY

**PHI Fields in This Domain:**
- **Appointment Timing:** Start, End (date/time linked to patient = PHI)
- **Participants:** Patient name, provider name
- **Clinical Data:** ReasonCode (chief complaint), FlagList (allergies, conditions)
- **Financial:** Sponsor, PolicyNo (insurance identifiers)
- **Contact:** PatientInstruction, Comment (may contain health information)

**Audit Coverage:**
- ✅ Appointment Create/Update/Delete - Logged via AuditableBaseDbContext.cs:25
- ✅ Status Changes (Booked → Arrived → Fulfilled) - Logged
- ✅ Cancellation with reason - Logged
- ⚠️ Patient flag access - May not be explicitly logged (verify in consuming service)
- ⚠️ Slot search - Read operations typically not audited

**Access Controls:**
- Multi-tenancy: OrganizationId, PracticeId fields enforce tenant isolation
- Provider access: Can view only appointments for assigned practice
- Patient access: Can view only own appointments
- Staff access: Can view all appointments within assigned practice
- File: Global query filters via Finbuckle.MultiTenant (referenced in architecture)

**Clinical Safety Requirements:**
- **Patient Flags MUST Display:** FlagList shown at check-in to prevent adverse events
- **Allergy Alerts:** Critical for medication prescribing and procedure planning
- **VIP/Special Needs:** Ensures appropriate care and accommodations

**Retention:**
- Appointment records: 10 years from encounter date (UAE healthcare standard)
- Linked to patient medical record retention policy
- Cannot delete appointments, use IsArchived soft delete (ISoftDelete.cs:5)

**Data Quality:**
- Appointment times must be in practice timezone
- Slots cannot overlap for same provider/room
- Status transitions must follow valid lifecycle
- Cancellation requires reason code

---

## CRITICAL FINDINGS

**Risk #1: Patient Flag Service Failure**
- If ISchedulingApi.PatientFlag endpoint fails during check-in, allergies not displayed
- Impact: CRITICAL - Provider may prescribe contraindicated medication
- Mitigation: Implement circuit breaker with cached patient flags, alert if service unavailable

**Risk #2: Slot Booking Race Condition**
- Multiple users may attempt to book same slot simultaneously
- Impact: Double booking possible without proper locking
- Mitigation: Optimistic concurrency check on slot status (not visible in DTO library)

**Risk #3: Email Notification Failures Silent**
- Email service failures don't block appointment creation (fire-and-forget pattern)
- Impact: Patient doesn't receive confirmation, may miss appointment
- Mitigation: Add retry mechanism or notification status tracking

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
