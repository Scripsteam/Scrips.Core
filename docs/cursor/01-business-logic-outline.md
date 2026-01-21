# Business Logic Outline for Scrips.Core

**Repository:** Scrips.Core (Shared Library - DTOs, Models, API Clients)  
**Purpose:** Cross-cutting data structures and service contracts for healthcare microservices platform  
**Architecture:** Multi-tenant, event-driven, HIPAA-compliant

---

## DOMAIN AREAS IDENTIFIED

### 1. Appointment & Scheduling Management

**Core Entities:**
- Appointment - Scheduled healthcare encounter with status lifecycle, participants, and billing - Models/Scheduling/AppointmentResponse.cs
- Slot - Available time blocks for booking with provider, room, specialty - Models/Scheduling/SlotResponse.cs
- AppointmentProfile - Provider appointment types, durations, service codes - Models/Practice/AppointmentProfileResponse.cs
- ReminderProfile - Automated reminder configuration for appointments - Models/Scheduling/CreateReminderProfileRequest.cs
- Recurring - Appointment series with recurrence rules - Models/Scheduling/RecurringDto.cs

**Key Workflows:**
- **Slot Search & Booking:** Find available time slots by provider/specialty/date → select patient → verify sponsor → calculate price → confirm booking
- **Check-In Process:** Appointment arrival → eligibility verification → patient flag display (allergies, VIP) → copay collection
- **Cancellation:** Cancel request → reason code → notification → slot release → invoice void/adjustment
- **Recurring Series:** Pattern definition → bulk slot reservation → individual modifications → series-wide updates

**Service Integrations:**
- Scheduling Service (ISchedulingApi.cs): Slot search, appointment CRUD, patient flags
- Practice Service (IPracticeApi.cs): Provider calendars, appointment profiles, slot generation
- Billing Service (IBillingApi.cs): Price calculation, invoice linkage
- Email Service (IEmailSenderApi.cs): Appointment reminders and confirmations
- Notifications Service (INotificationsApi.cs): Real-time alerts, video URLs

**Compliance Requirements:**
- **PHI Fields:** Appointment date/time, patient ID, provider name, chief complaint, patient flags (allergies)
- **Audit Requirements:** All appointment status changes logged with user/tenant context
- **Retention:** 10 years for appointment history (linked to medical records)
- **Clinical Safety:** Patient flags (allergies, special needs) must display at check-in to prevent adverse events

---

### 2. Patient Management & Demographics

**Core Entities:**
- Patient - Individual with demographics, identifications, insurance, guardians - Models/Patient/EditPatientResponse.cs
- Guardian - Legal representative for minor patients - Models/Patient/GuardianDto.cs
- HealthInsuranceSponsor - Insurance coverage with policy details - Models/Patient/HealthInsuranceResponse.cs
- PatientCorporateSponsor - Corporate employee health benefits - Models/Patient/PatientCorporateResponse.cs
- EmiratesIdCardData - UAE national ID verification data - Models/Identity/EmiratesIdCardData.cs
- PatientBillingGroup - Sponsor type classification for pricing - Models/Patient/PatientBillingGroupModel.cs

**Key Workflows:**
- **Registration:** Demographics capture → Emirates ID validation → insurance/corporate verification → guardian assignment (if minor) → address entry
- **Eligibility Verification:** Sponsor selection → policy lookup → coverage date validation → benefit structure retrieval
- **Patient Merge:** Duplicate detection → merge initiation → data consolidation → appointment/invoice reassignment

**Service Integrations:**
- Patient Service (IPatientApi.cs): CRUD operations, insurance lookup, guardian management
- Identity Service (IIdentityApi.cs): Emirates ID validation, user linkage
- Billing Service (IBillingApi.cs): Patient balance inquiry, sponsor eligibility

**Compliance Requirements:**
- **PHI Fields:** Name, date of birth, Emirates ID, email, phone, address, MRN, insurance member ID, guardian relationship
- **Audit Requirements:** All patient data access and modifications logged (164.308(a)(1)(ii)(D))
- **Retention:** 10 years from last encounter (UAE healthcare standard)
- **Access Control:** Role-based access, provider can only view assigned patients, audit log for all access
- **Consent:** Required for data sharing (guardian consent for minors)

---

### 3. Billing, Invoicing & Claims

**Core Entities:**
- Invoice - Financial document with line items, diagnoses, payments - Models/Billing/InvoiceDetailsDto.cs
- BillingProfile - Fee schedules and pricing rules - Models/Billing/BillingGroupDto.cs
- ProcedureCode - Service codes for billing - Models/Billing/ProcedureCodeModel.cs
- PaymentReceipt - Payment confirmation and tracking - Models/Billing/PaymentReceiptResponse.cs
- PatientCoPayment - Patient responsibility amount - Models/Billing/PatientCoPaymentDto.cs
- CalculatePriceRequest - Multi-factor pricing input - Models/Billing/CalculatePriceRequest.cs

**Key Workflows:**
- **Price Calculation:** Service selection → sponsor identification → billing profile lookup → agreement pricing → discount → tax → split (sponsor/patient)
- **Invoice Generation:** Service capture from appointment → price calculation → line items → diagnosis pointers → sponsor allocation → finalization
- **Payment Processing:** Invoice review → payment method selection → amount entry → receipt generation → balance update → sponsor vs patient tracking
- **Claim Submission:** Invoice finalization → claim generation → standard code mapping → payer submission → status tracking → remittance processing

**Service Integrations:**
- Billing Service (IBillingApi.cs): Price calculation, invoice CRUD, payment recording, balance inquiry
- Scheduling Service (ISchedulingApi.cs): Appointment service linkage
- Patient Service (IPatientApi.cs): Sponsor verification

**Compliance Requirements:**
- **PHI Fields:** Patient ID in invoices, service dates, diagnosis codes (financial PHI)
- **Audit Requirements:** All price calculations, invoice access, payment transactions logged
- **Retention:** 7 years for financial records (HIPAA/UAE requirement)
- **Accuracy:** Multi-factor pricing must be auditable and reproducible
- **Claim Standards:** ICD-10 diagnosis codes, CPT/procedure codes for submission

---

### 4. Clinical Documentation & AI Decision Support

**Core Entities:**
- ChiefComplaint - Patient's primary concern with NLP analysis - Models/AIChiefComplaint/ChiefComplaintDto.cs
- ChiefComplaintDocument - Azure Search indexed document with embeddings - Models/AIChiefComplaint/ChiefComplaintDocument.cs
- HealthcareEntity - Extracted medical entities (symptoms, conditions) - Models/AIChiefComplaint/HealthcareEntity.cs
- RAGSearchResult - Similar case retrieval results - Models/AIChiefComplaint/RAGSearchResult.cs
- RAGSuggestion - ICD-10 code suggestions - Models/AIChiefComplaint/RAGSuggestion.cs
- DocumentationResponse - AI-generated clinical notes - Models/AIChiefComplaint/DocumentationResponse.cs

**Key Workflows:**
- **Chief Complaint Analysis:** Free-text input → NLP entity extraction (symptoms, conditions, anatomical sites, severity, temporal context) → RAG search for similar cases → ICD-10 suggestions → confidence scoring
- **AI Documentation:** Chief complaint + similar cases → assessment generation → plan generation → provider review/edit → clinical note finalization
- **Vector Search:** Complaint embedding (1536 dimensions) → Azure Cognitive Search similarity → top-K results → relevance ranking

**Service Integrations:**
- Master Data Service (IMasterApi.cs): RAG search, clinical suggestions, AI documentation generation
- Azure Cognitive Search: Vector similarity search with OpenAI embeddings
- SNOMED CT: Clinical terminology reference system

**Compliance Requirements:**
- **PHI Fields:** Chief complaint text, symptoms, clinical context (clinical PHI)
- **Audit Requirements:** AI-assisted diagnosis suggestions must be logged for clinical governance
- **Clinical Safety:** AI suggestions are recommendations only, require provider review before acceptance
- **Model Training:** Embeddings must be from de-identified training data
- **Data Residency:** Azure region must comply with UAE data residency requirements

---

### 5. Provider, Practice & Organization Setup

**Core Entities:**
- PracticeSetupDetails - Facility configuration with NABIDH authority - Models/Practice/PracticeSetupDetails.cs
- DoctorSetupDetails - Provider professional information - Models/Practice/DoctorSetupDetails.cs
- StaffSetupDetails - Non-clinical staff configuration - Models/Practice/StaffSetupDetails.cs
- PractitionerRole - Provider role at practice - Models/Practice/PractitionerRole.cs
- OrganizationV1Dto - Multi-tenant organization entity - Models/Organization/OrganizationV1Dto.cs
- ProviderLicense - Professional licensing information - Models/Practice/ProviderLicense.cs

**Key Workflows:**
- **Organization Setup:** Tenant creation → organization profile → settings configuration → role assignment
- **Practice Registration:** License capture → NABIDH assigning authority → contact details → billing configuration → exam room setup
- **Provider Onboarding:** Professional details → license validation → qualification verification → specialty assignment → appointment profile creation → schedule configuration
- **Staff Management:** Staff registration → role assignment → practice association

**Service Integrations:**
- Practice Service (IPracticeApi.cs): Practice CRUD, provider schedules, staff management, appointment profiles
- Provider Service (IProviderApi.cs): Provider details, reason for visit codes, qualifications
- Organization Service (IOrganizationApi.cs): Organization CRUD, settings management
- Identity Service (IIdentityApi.cs): User/tenant context resolution

**Compliance Requirements:**
- **PHI Fields:** None (practice/provider data is business information, not patient PHI)
- **Audit Requirements:** Practice configuration changes, provider onboarding, NABIDH authority updates logged
- **NABIDH Compliance:** MANDATORY for UAE healthcare practices - assigning authority (OID format) for patient identification across facilities
- **License Validation:** Active license required for clinical services
- **Multi-Tenancy:** Organization data isolation enforced at database level

---

### 6. Identity, Authentication & Multi-Tenancy

**Core Entities:**
- UserIdentity - Authentication with roles, permissions, claims - Models/Identity/*
- Tenant - Isolated organizational workspace - Referenced via JWT claims
- Person - User demographics separate from patient/provider - Models/Person/PersonInfoResponse.cs
- ScripsPermissions - Granular permission structure - Shared/Authorization/ScripsPermissions.cs
- ScripsClaims - JWT claim keys - Shared/Authorization/ScripsClaims.cs

**Key Workflows:**
- **User Authentication:** Login → JWT generation with claims (sub, tenant, sa) → token validation → context resolution
- **Multi-Tenant Access:** Request → JWT extraction → tenant claim → global query filter → data isolation enforcement
- **Role-Based Authorization:** Permission check → resource access → role validation → tenant scope verification

**Service Integrations:**
- Identity Service (IIdentityApi.cs): User authentication, tenant resolution, JWT token management
- Person Service (IPersonApi.cs): User demographic information
- All Services: JWT validation, tenant context extraction

**Compliance Requirements:**
- **PHI Fields:** None (authentication data only)
- **Audit Requirements:** All login attempts, permission failures, tenant switches logged
- **Multi-Tenancy:** CRITICAL - data isolation failure = HIPAA violation (cross-tenant PHI exposure)
- **Session Management:** Token expiration, refresh mechanisms
- **Super Admin:** System-level access across all tenants (high risk - strict access control)

---

### 7. Audit Logging & Compliance Tracking

**Core Entities:**
- AuditLog - Entity change tracking - Scrips.Core.Models.Audit/LogAudit
- MaskValueAuditAttribute - PHI field masking - BaseDbContext/MaskValueAuditAttribute.cs
- AuditableBaseDbContext - Change detection and publishing - BaseDbContext/AuditableBaseDbContext.cs
- Topics - Dapr pub/sub event topics - Topics.cs

**Key Workflows:**
- **Entity Change Tracking:** SaveChanges() → change detection → property masking (MaskValueAudit) → Dapr publish → audit consumer
- **Audit Log Querying:** Search by user, tenant, entity type, date range → compliance reporting
- **Event Publishing:** Domain events (TenantCreated, PracticeCreated, etc.) → Dapr pub/sub → consuming services

**Service Integrations:**
- Dapr Pub/Sub: SaveAudit topic, domain event topics
- External Audit Consumer: Receives and stores audit logs

**Compliance Requirements:**
- **PHI Fields:** Audit logs may contain PHI in OldValue/NewValue - MUST be masked with `[MaskValueAudit]` attribute
- **Audit Requirements:** MANDATORY - Complete audit trail for HIPAA 164.308(a)(1)(ii)(D)
- **Retention:** 6 years minimum (HIPAA requirement)
- **Integrity:** Immutable audit logs, no deletion allowed
- **Completeness:** All PHI access and modifications logged
- **CRITICAL ISSUE:** Fire-and-forget Dapr publishing = audit logs lost if Dapr unavailable (compliance violation)

---

### 8. Master Data & Reference Systems

**Core Entities:**
- Gender, MaritalStatus, IdentityType, OwnerType - Lookup values via IMasterApi
- Speciality - Medical specialties with SNOMED codes - Models/Scheduling/Speciality.cs
- PracticeValueSet - Clinical terminology value sets - Referenced SNOMED CT system

**Key Workflows:**
- **Lookup Retrieval:** Request reference data → cache lookup → return values for dropdowns
- **Code Validation:** Verify ICD-10, CPT, SNOMED codes against standard terminologies

**Service Integrations:**
- Master Data Service (IMasterApi.cs): Gender, marital status, countries, medical codes
- SNOMED CT: Medical terminology reference (https://snomed.info/sct)

**Compliance Requirements:**
- **PHI Fields:** None (reference data only)
- **Audit Requirements:** Minimal (reference data access not typically audited)
- **Standards:** ICD-10 (diagnoses), SNOMED CT (clinical findings), CPT (procedures)
- **Caching:** Should be cached to reduce database load (NOT IMPLEMENTED in this repo)

---

### 9. Notifications & Communication

**Core Entities:**
- EmailData - Email messages with templates, recipients, attachments - Referenced in IEmailSenderApi.cs
- VideoConferenceUrl - Telehealth video call links - INotificationsApi.cs:7
- NotificationRequest - Real-time alerts for providers - Inferred from INotificationsApi

**Key Workflows:**
- **Email Sending:** Template selection (appointment-reminder, invoice) → data binding → SMTP delivery → fire-and-forget
- **Real-Time Notifications:** Event trigger (patient check-in) → SignalR push → provider alert display
- **Video URL Generation:** Telehealth request → video service integration → URL generation → session tracking

**Service Integrations:**
- Email Service (IEmailSenderApi.cs): Templated email sending for appointments, invoices, reminders
- Notifications Service (INotificationsApi.cs): Video URL generation, real-time push notifications
- Scheduling Service: Appointment status changes trigger notifications
- Billing Service: Invoice delivery via email

**Compliance Requirements:**
- **PHI Fields:** Patient email, name, appointment details in email body, video session identifiers
- **Audit Requirements:** Email sending should be logged, video session initiation logged
- **Email Security:** HIPAA-compliant email service required (TLS encryption, Business Associate Agreement)
- **Video Compliance:** Session encryption, consent for recording, retention policy
- **CRITICAL ISSUE:** Fire-and-forget email pattern = no retry, patients may miss critical reminders

---

## DOCUMENTATION MAPPING

Each domain area above will have **ONE consolidated documentation file** in the next phase:

- `02-section-1-appointments-scheduling.md` - Domain Area 1
- `02-section-2-patient-management.md` - Domain Area 2
- `02-section-3-billing-financial.md` - Domain Area 3
- `02-section-4-clinical-ai.md` - Domain Area 4
- `02-section-5-provider-practice-org.md` - Domain Area 5
- `02-section-6-identity-tenancy.md` - Domain Area 6
- `02-section-7-audit-compliance.md` - Domain Area 7
- `02-section-8-master-data.md` - Domain Area 8
- `02-section-9-notifications-communication.md` - Domain Area 9

**Total:** 9 consolidated files instead of 68 separate section files

---

## VERIFICATION CHECKLIST

- [x] 9 domain areas identified (including Notifications & Communication)
- [x] Each domain has entities + workflows + integrations + compliance
- [x] PHI/compliance noted where applicable
- [x] Total output: 263 lines
- [x] Domains grouped by business function, not code structure
- [x] Each domain area maps to ONE future consolidated doc
