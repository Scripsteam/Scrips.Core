# Provider, Practice & Organization Setup - Complete Documentation

## OVERVIEW

**Purpose:** Manage provider credentials, practice configuration, organization hierarchy, and NABIDH compliance for UAE healthcare

**Key Entities:** PracticeSetupDetails, DoctorSetupDetails, PractitionerRole, OrganizationV1Dto, StaffSetupDetails, ProviderLicense

**Key Workflows:** Organization setup, practice registration, provider onboarding, staff management, NABIDH authority assignment

**PHI Scope:** NO - Provider/practice credentials are business data, not patient PHI

---

## ENTITIES

### PracticeSetupDetails

**Location:** Models/Practice/PracticeSetupDetails.cs:1

**Purpose:** Healthcare facility configuration with licensing, NABIDH authority, contact details

**Key Fields:**
- PracticeId - Unique identifier
- PracticeName - Facility name
- **NabidhAssigningAuthority** - UAE health information exchange OID (line 18)
- LicenseNumber - Facility license
- ContactDetails - Phone, email, address
- OrganizationId - Parent organization
- BillingConfiguration - Fee schedules, payment methods
- ExamRooms - Room list for appointments

**NABIDH:** MANDATORY for UAE practices, format: OID (2.16.784.1.3.1.XXXXX)

**Relationships:**
- Organization: Many practices to one organization
- Providers: Many providers work at one practice
- Appointments: All appointments linked to practice

---

### DoctorSetupDetails / PractitionerRole

**Location:** Models/Practice/DoctorSetupDetails.cs, PractitionerRole.cs

**Purpose:** Provider professional information, licenses, qualifications, specialties

**Key Fields:**
- ProviderId - Unique identifier
- ProviderLicense - Medical license number, expiration
- Qualifications - Degrees, certifications
- Specialties - Board certifications
- AppointmentProfiles - Appointment types offered
- Schedule - Weekly availability

**Relationships:**
- Practice: One provider to many practices (multi-location)
- PractitionerRole: Provider-practice association with role

---

### OrganizationV1Dto

**Location:** Models/Organization/OrganizationV1Dto.cs:1

**Purpose:** Top-level multi-tenant entity with settings and practice hierarchy

**Key Fields:**
- OrganizationId - Tenant identifier
- OrganizationName
- OrganizationSettings - Appointment rules, billing config, branding
- PrimaryPractice - Default practice

**Multi-Tenancy:** OrganizationId enforces data isolation via Finbuckle.MultiTenant

---

### StaffSetupDetails

**Location:** Models/Practice/StaffSetupDetails.cs

**Purpose:** Non-clinical staff (receptionists, billing staff, administrators)

**Key Fields:**
- StaffId, UserId
- Role - Receptionist/Billing/Admin
- PracticeId - Assigned practice
- Permissions - Access control

---

## WORKFLOWS

### Workflow 1: Organization Setup

**Entry Point:** IOrganizationApi.cs:8,14

**Steps:**
1. **Tenant Creation** - New organization record
2. **Settings Configuration** - IOrganizationApi.cs:11 (GET /api/v1/OrganizationSettings/{id})
3. **Primary Practice** - Create first practice
4. **Admin User** - Assign organization administrator
5. **Welcome Email** - IEmailSenderApi.cs:8

**Dapr Event:** OrganizationV1Created (Topics.cs:10)

---

### Workflow 2: Practice Registration

**Entry Point:** IPracticeApi.cs:25 (GET /api/Practice/PracticeSetupDetails/{practiceId})

**Steps:**
1. **License Capture** - Facility medical license
2. **NABIDH Authority** - Assign UAE health exchange OID (PracticeSetupDetails.cs:18)
3. **Contact Details** - Phone, email, address
4. **Billing Configuration** - Fee schedules, payment methods
5. **Exam Room Setup** - IPracticeApi.cs:80 (GET /api/Practice/EditPracticeExamRoom/{practiceId})

**Compliance:** NABIDH assigning authority MANDATORY for UAE healthcare

**Dapr Event:** PracticeCreated (Topics.cs:13)

---

### Workflow 3: Provider Onboarding

**Entry Point:** IProviderApi.cs:6-25

**Steps:**
1. **Professional Details** - IProviderApi.cs:12 (GET /api/ProfessionalDetails/{providerId})
2. **License Validation** - Medical license number, expiration
3. **Qualification Verification** - Degrees, certifications
4. **Specialty Assignment** - Board certifications
5. **Appointment Profile** - IPracticeApi.cs:12 (GET /api/Doctor/AppointmentProfileList)
6. **Schedule Configuration** - IPracticeApi.cs:28 (POST /api/Doctor/CalendarSlots3)

**Dapr Event:** DoctorCreated (Topics.cs:7)

---

### Workflow 4: Staff Management

**Entry Point:** IPracticeApi.cs:47,88 (GET /api/Staff/StaffList, GetStaffId)

**Steps:**
1. **Staff Registration** - Create staff record
2. **Role Assignment** - Receptionist/Billing/Admin
3. **Practice Association** - Assign to one or more practices
4. **Permission Configuration** - Access control

---

## INTEGRATIONS

**Practice Service (IPracticeApi.cs:7-96):**
- 18 endpoints (most complex service): Practice CRUD, provider schedules, staff management
- Key: PracticeSetupDetails (line 25), CalendarSlots3 (line 28), StaffList (line 47)

**Provider Service (IProviderApi.cs:6-25):**
- 5 endpoints: Provider details, licenses, reason for visit codes

**Organization Service (IOrganizationApi.cs:6-19):**
- 4 endpoints: Organization CRUD, settings

---

## COMPLIANCE SUMMARY

**PHI:** NONE (provider credentials are business data)

**Audit:** ✅ Provider changes, NABIDH authority logged via AuditableBaseDbContext.cs:25

**NABIDH:** `NabidhAssigningAuthority` property (PracticeSetupDetails.cs:18) - MANDATORY for UAE

**License Validation:** Active license required for clinical services

**Multi-Tenancy:** OrganizationId isolation critical

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
