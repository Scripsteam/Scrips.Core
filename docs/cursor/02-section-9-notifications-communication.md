# Notifications & Communication - Complete Documentation

## OVERVIEW

**Purpose:** Manage email notifications, real-time alerts, video conference URLs for telehealth

**Key Entities:** EmailData, NotificationRequest (inferred), VideoConferenceUrl

**Key Workflows:** Email sending (appointment reminders, invoices), real-time notifications, video URL generation

**PHI Scope:** YES - Emails contain patient names, appointment details, provider information

---

## ENTITIES

### EmailData

**Location:** Referenced in IEmailSenderApi.cs:8

**Purpose:** Email message with recipient, subject, body, attachments

**Key Fields (Inferred):**
- To - Recipient email address (**YES** - patient email = PHI)
- Subject - Email subject line
- Body - Email content (**YES** - likely contains appointment details, patient name)
- Attachments - PDF invoices, appointment confirmations
- EmailKey - Template identifier (appointment-reminder, invoice, password-reset)

---

### Video Conference URL

**Location:** INotificationsApi.cs:7 (GetVideoUrl)

**Purpose:** Generate video call link for telehealth appointments

**Request Parameters:**
- AppointmentId - Linked appointment
- PatientId - Patient identifier
- NotificationType - Type of video session

**Response:** string (video URL)

---

## WORKFLOWS

### Workflow 1: Email Sending

**Entry Point:** IEmailSenderApi.cs:8 (POST /api/Email/Send/{emailKey})

**Steps:**
1. **Template Selection** - EmailKey (appointment-reminder, invoice, etc.)
2. **Data Binding** - Populate template with patient name, appointment date/time
3. **Send Email** - Via SMTP or email service
4. **Fire-and-Forget** - Returns Task (void), no confirmation

**Use Cases:**
- Appointment confirmation after booking
- Appointment reminder (24 hours before)
- Invoice delivery after service
- Password reset for patient portal
- Welcome email for new users

**Error Handling:**
- Email service down: Logged but does not block operation
- Invalid email address: Bounces not tracked in this library
- Delivery failure: Silent failure (no retry visible)

**PHI:** Patient name, email, appointment date/time, provider, invoice amounts

---

### Workflow 2: Real-Time Notifications

**Entry Point:** INotificationsApi.cs (implied SignalR push)

**Steps:**
1. **Event Trigger** - Patient checked in, appointment status changed
2. **SignalR Push** - To connected provider devices
3. **Alert Display** - "Patient checked in - Exam Room 3"

**Technology:** SignalR (referenced in Section 5.7 - not explicitly in this library)

---

### Workflow 3: Video URL Generation

**Entry Point:** INotificationsApi.cs:7 (GET /api/Notifications/GetVideoUrl)

**Steps:**
1. **Request** - AppointmentId, PatientId, NotificationType
2. **Generate URL** - Video conference service (Zoom, Teams, custom)
3. **Return URL** - Patient and provider use for telehealth

**Error Handling:**
- Video service unavailable: Return error, cannot conduct telehealth
- URL expiration: Generate new URL

**PHI:** Appointment ID, Patient ID linked to video session

**Compliance:**
- Video recordings (if any) must be HIPAA-compliant
- Session encryption required
- Consent for recording required

---

## INTEGRATIONS

**Email Sender Service (IEmailSenderApi.cs:6-9):**
- 1 endpoint: Send (POST)
- Fire-and-forget pattern

**Notifications Service (INotificationsApi.cs:5-8):**
- 1 endpoint: GetVideoUrl (GET)
- Real-time push (SignalR)

---

## COMPLIANCE SUMMARY

**PHI Fields:** Patient email, name, appointment details in email body, video session identifiers

**Audit:** ✅ Email sending should be logged (verify in consuming service), video session initiation logged

**Email Security:** Must use HIPAA-compliant email service (encrypted transmission)

**Video Compliance:** Session encryption, consent for recording, retention policy

---

## CRITICAL FINDINGS

**Risk #1: Email PHI Protection Unclear (HIGH)**
- **Issue:** Email transmission security depends on email service (not enforced in code)
- **Impact:** PHI exposure if non-compliant email service used
- **Mitigation:** Document email service requirements (TLS, HIPAA Business Associate Agreement)

**Risk #2: Email Failures Silent (MEDIUM)**
- **Issue:** Fire-and-forget pattern (no retry, no status tracking)
- **Impact:** Patients miss appointment reminders
- **Mitigation:** Implement retry or notification status tracking

**Risk #3: Video Recording Compliance (MEDIUM)**
- **Issue:** No video recording policy visible
- **Impact:** Compliance violation if recordings not managed properly
- **Mitigation:** Document video recording consent, retention, deletion policies

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
