# Integration Details - Tiered Detail Levels

**Classification Approach:** CRITICAL (PHI, patient safety) → HIGH (business critical) → LOW (lookups, reference)

---

## CLASSIFICATION SUMMARY

| Integration | Classification | Reason |
|-------------|---------------|--------|
| Patient Service | **CRITICAL** | Contains PHI (patient demographics, insurance, guardians) |
| Billing Service | **CRITICAL** | Financial PHI (invoices, payments, diagnoses), revenue impact |
| Scheduling Service | **CRITICAL** | PHI (appointments, patient flags/allergies), patient safety |
| Dapr Pub/Sub (Audit) | **CRITICAL** | HIPAA audit trail, compliance requirement |
| Email Sender Service | **HIGH** | PHI in email body, notification reliability |
| Notifications Service | **HIGH** | Video URLs for telehealth, clinical access |
| Identity Service | **HIGH** | Authentication, multi-tenant isolation |
| Provider Service | **HIGH** | Provider details, chief complaints |
| Practice Service | **HIGH** | Practice configuration, provider schedules |
| Organization Service | **HIGH** | Tenant management, settings |
| Person Service | **LOW** | User demographics (non-PHI) |
| Master Data Service (Lookups) | **LOW** | Reference data (Gender, MaritalStatus, etc.) - 4 endpoints |
| Master Data Service (AI) | **CRITICAL** | Clinical AI search with PHI chief complaints - 4 endpoints |
| Azure Cognitive Search | **CRITICAL** | Vector search with PHI clinical data |
| SQL Server Database | **CRITICAL** | Primary data store with PHI |
| gRPC Services | **HIGH** | High-performance inter-service communication (9 protos) |
| NABIDH | **LOW** | Reference identifier only, no active API |
| SNOMED CT | **LOW** | Terminology reference URL only |

**Totals:** 7 CRITICAL, 7 HIGH, 4 LOW (consolidated)

---

## CRITICAL INTEGRATIONS (Full Detail)

### 1. Patient Service
**Classification:** CRITICAL  
**Reason:** Contains extensive PHI (demographics, insurance, medical record numbers, guardian relationships)

**Purpose:** Patient registration, demographics management, insurance/sponsor verification, guardian relationships

**Type:** Sync HTTP/REST via Refit

**Endpoints:**
| Method | Path | Purpose | File:Line |
|--------|------|---------|-----------|
| GET | /api/Patients/{id} | Retrieve patient by ID | IPatientApi.cs:8 |
| GET | /api/Patients/PatientAddressList | Get patient addresses | IPatientApi.cs:11 |
| GET | /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId} | Insurance details | IPatientApi.cs:16 |
| GET | /api/Patients/{patientId}/PatientCorporateSponsor/{sponsorId} | Corporate sponsor | IPatientApi.cs:22 |
| GET | /api/Patients/GetGuardianDetails | Guardian info for minors | IPatientApi.cs:28 |

**Data Flow:**
- **Request:** PatientId, SponsorId parameters
- **Response:** EditPatientResponse (name, DOB, Emirates ID, MRN, email, phone, address), HealthInsuranceResponse (policy, member ID), GuardianDto (relationship, contact)
- **PHI Fields:** Name, DateOfBirth, EmiratesId, Email, Phone, Address, MRN, InsuranceMemberId, GuardianRelationship

**Authentication:**
- Method: JWT Bearer Token
- Location: Authorization header on all endpoints
- JWT Claims: sub (user ID), tenant (tenant ID), sa (super admin)

**Failure Handling:**
- Retry: **NO** (Refit default behavior)
- Circuit Breaker: **NO**
- Timeout: Not configured
- Fallback: Exception propagates to caller

**Healthcare Impact:**
- **Patient Safety:** Insurance verification failure = wrong billing, patient financial harm
- **Data Loss Risk:** No retry = failed patient updates lost silently
- **Audit Requirements:** All patient access MUST be logged (HIPAA 164.308(a)(1)(ii)(D))
- **Mitigation Needed:** Implement Polly retry policy, circuit breaker, timeout

---

### 2. Billing Service
**Classification:** CRITICAL  
**Reason:** Financial PHI (patient invoices, diagnosis codes, payment history), revenue impact

**Purpose:** Price calculation, invoice generation, payment processing, patient balance inquiry

**Type:** Sync HTTP/REST via Refit

**Endpoints:**
| Method | Path | Purpose | File:Line |
|--------|------|---------|-----------|
| POST | /api/Billing/Agreements/CalculatePrice | Multi-factor pricing (sponsor, profile, service) | IBillingApi.cs:14 |
| POST | /api/Billing/Agreements/PreCalculatePrice | Price preview | IBillingApi.cs:20 |
| GET | /api/Billing/Invoice/{invoiceId} | Invoice retrieval | IBillingApi.cs:26 |
| GET | /api/Billing/Payment/AvailableBalance | Patient account balance | IBillingApi.cs:29 |
| GET | /api/Billing/Payment/PaymentReceipts | Receipt history | IBillingApi.cs:34 |
| GET | /api/Billing/ProviderBillingProfiles | Provider fee schedules | IBillingApi.cs:8 |

**Data Flow:**
- **Request:** CalculatePriceRequest (SponsorId, ServiceCode, AppointmentProfileId, AgreementId)
- **Response:** InvoiceDetailsDto (line items, diagnoses, payments, balances), PaymentReceiptResponse
- **PHI Fields:** PatientId in invoice, DiagnosisCodes (ICD-10), ServiceDates, PaymentAmounts

**Authentication:**
- Method: JWT Bearer Token
- Location: Authorization header
- Multi-Tenancy: TenantId from JWT claim

**Failure Handling:**
- Retry: **NO**
- Circuit Breaker: **NO**
- Timeout: Not configured
- Fallback: Price calculation failure = appointment booking blocked

**Healthcare Impact:**
- **Patient Safety:** Incorrect pricing = patient financial harm, potential debt
- **Data Loss Risk:** Payment transaction failure without retry = lost revenue
- **Audit Requirements:** All price calculations, invoice access, payments MUST be logged
- **Retention:** 7 years (HIPAA financial records)
- **Mitigation Needed:** Idempotent price calculation, transaction retry with idempotency keys

---

### 3. Scheduling Service
**Classification:** CRITICAL  
**Reason:** PHI (appointment details, chief complaints), patient safety (allergy flags)

**Purpose:** Appointment booking, slot availability, patient flags/alerts display

**Type:** Sync HTTP/REST via Refit

**Endpoints:**
| Method | Path | Purpose | File:Line |
|--------|------|---------|-----------|
| POST | /api/Appointment/Slots | Find available time slots | ISchedulingApi.cs:8 |
| GET | /api/Appointment/PatientFlag | Patient alerts (allergies, VIP) | ISchedulingApi.cs:20 |
| GET | /api/Appointment/GetById/{id} | Appointment details | ISchedulingApi.cs:23 |
| POST | /api/Appointment | Appointment search/filter | ISchedulingApi.cs:26 |

**Data Flow:**
- **Request:** SlotRequest (ProviderId, Date, Specialty), PatientId
- **Response:** SlotResponse (available times), AppointmentResponse (date/time, patient, provider, status, chief complaint), PatientFlagDto (allergy warnings)
- **PHI Fields:** AppointmentDateTime, PatientId, ChiefComplaint, AllergyFlags, ProviderName

**Authentication:**
- Method: JWT Bearer Token
- Multi-Tenancy: TenantId filtering

**Failure Handling:**
- Retry: **NO**
- Circuit Breaker: **NO**
- Fallback: Slot search failure = no appointments can be booked

**Healthcare Impact:**
- **Patient Safety:** CRITICAL - Patient flag failure (allergies) = adverse drug events, potential death
- **Data Loss Risk:** Appointment creation failure without retry = lost bookings, patient no-shows
- **Audit Requirements:** All appointment status changes logged (10-year retention)
- **Clinical Safety:** Patient flags MUST display at check-in (life-or-death information)
- **Mitigation Needed:** URGENT - Retry for patient flags, fallback to cached flags, alerting on flag API failure

---

### 4. Dapr Pub/Sub (Audit Logging)
**Classification:** CRITICAL  
**Reason:** HIPAA compliance - complete audit trail required by law

**Purpose:** Publish entity change events for audit trail, domain event broadcasting

**Type:** Async Message Broker (Dapr pub/sub)

**Topics:**
| Topic | Trigger | PHI? | File:Line |
|-------|---------|------|-----------|
| SaveAudit | Every SaveChangesAsync() | YES (OldValue/NewValue) | AuditableBaseDbContext.cs:61 |
| TenantCreated | Tenant onboarding | NO | Topics.cs:5 |
| PracticeCreated | Practice registration | NO | Topics.cs:13 |
| OrganizationCreated | Org setup | NO | Topics.cs:9 |

**Data Flow:**
- **Trigger:** Entity change detection in EF Core ChangeTracker
- **Payload:** AuditLog (TableName, PrimaryKey, Action, OldValue, NewValue, UserId, TenantId, Timestamp)
- **PHI Fields:** OldValue/NewValue may contain PHI (patient names, emails, diagnoses)
- **Masking:** `[MaskValueAudit]` attribute should mask PHI (incomplete implementation - see Debt #25)

**Publishing Pattern:**
```csharp
if (await _daprClient.CheckHealthAsync())
    await _daprClient.PublishEventAsync("pubsub", "SaveAudit", changes);
```
**Location:** AuditableBaseDbContext.cs:60-61

**Failure Handling:**
- Retry: **NO** (fire-and-forget)
- Circuit Breaker: NO
- Fallback: **AUDIT LOGS LOST SILENTLY**
- Health Check: Only checks if Dapr is up, doesn't retry on transient failures

**Healthcare Impact:**
- **HIPAA Violation:** Lost audit logs = non-compliance, potential fines ($100 - $50,000 per violation)
- **Patient Safety:** No audit trail for PHI access = cannot detect breaches
- **Data Loss Risk:** HIGH - Dapr unavailable = all audit logs lost permanently
- **Audit Requirements:** MANDATORY - 6-year retention, immutable, complete trail (164.308(a)(1)(ii)(D))
- **Mitigation URGENT:** Implement retry with exponential backoff, dead letter queue, local fallback logging

---

### 5. Master Data Service (AI Endpoints)
**Classification:** CRITICAL  
**Reason:** Clinical AI with PHI chief complaints, patient safety impact

**Purpose:** AI-powered clinical search, ICD-10 suggestions, automated documentation

**Type:** Sync HTTP/REST via Refit

**Endpoints:**
| Method | Path | Purpose | File:Line |
|--------|------|---------|-----------|
| POST | /api/AISearch/RAGSearch | Vector search for similar cases | IMasterApi.cs:17 |
| POST | /api/AISearch/ClinicalSuggestions | ICD-10 code suggestions | IMasterApi.cs:19 |
| POST | /api/AISearch/GenerateDocumentation | AI clinical note generation | IMasterApi.cs:21 |
| GET | /api/AISearch/ByNameAndCategory | Chief complaint search | IMasterApi.cs:23 |

**Data Flow:**
- **Request:** ChiefComplaintDto (free-text patient complaint), RAGSearchRequest (complaint text, embeddings)
- **Response:** RAGSearchResult (similar cases, confidence scores), RAGSuggestion (ICD-10 codes), DocumentationResponse (AI-generated notes)
- **PHI Fields:** ChiefComplaint text (symptoms, conditions, patient context)

**Authentication:**
- Method: JWT Bearer Token
- Azure Cognitive Search behind the scenes (Azure.Search.Documents v11.6.0)

**Failure Handling:**
- Retry: **NO**
- Circuit Breaker: **NO**
- Fallback: AI search failure = provider manual coding (workflow degradation)

**Healthcare Impact:**
- **Patient Safety:** AI suggestion errors = potential misdiagnosis (mitigated by provider review requirement)
- **Data Loss Risk:** Complaint embeddings lost if not persisted
- **Audit Requirements:** AI-assisted diagnosis MUST be logged for clinical governance
- **Clinical Safety:** AI suggestions are recommendations only, require provider acceptance
- **Data Residency:** Azure region must comply with UAE laws
- **Model Training:** Embeddings from de-identified data required
- **Mitigation Needed:** Fallback to cached suggestions, graceful degradation, audit all AI interactions

---

### 6. Azure Cognitive Search
**Classification:** CRITICAL  
**Reason:** Vector search with PHI clinical data (chief complaints)

**Purpose:** AI-powered semantic search with OpenAI embeddings for clinical NLP

**Type:** Cloud AI Service (Azure)

**Configuration:**
- **Package:** Azure.Search.Documents v11.6.0
- **File:** Scrips.Core.csproj:8
- **Model:** ChiefComplaintDocument.cs:1-38

**Data Flow:**
- **Index:** ChiefComplaintDocument with 1536-dimension vector embeddings
- **Search:** Semantic similarity for chief complaints
- **PHI Fields:** ChiefComplaintText, ExtractedEntities (symptoms, conditions, anatomical sites)

**Features:**
- Vector search (cosine similarity)
- RAG (Retrieval Augmented Generation)
- ICD-10 code suggestions
- Healthcare entity extraction

**Authentication:**
- Method: Azure API Key or Managed Identity
- Location: Master Data Service configuration (not visible in core library)

**Failure Handling:**
- Retry: Handled by Azure SDK (configurable)
- Fallback: Service degrades to manual search

**Healthcare Impact:**
- **Patient Safety:** Search failure = provider loses clinical decision support
- **Data Loss Risk:** Index corruption = historical chief complaints lost
- **Audit Requirements:** Search queries with PHI must be logged
- **Data Residency:** Azure region must be UAE or compliant region
- **Mitigation:** Multi-region replication, backup indexes, cached results

---

### 7. SQL Server Database
**Classification:** CRITICAL  
**Reason:** Primary data store for ALL PHI and business data

**Purpose:** Persistent storage for patient, appointment, billing, audit data

**Type:** Relational Database (Microsoft SQL Server)

**Configuration:**
- **Provider:** Microsoft.Data.SqlClient v5.1.0
- **Default:** "mssql" hardcoded at Startup.cs:23
- **Connection:** DatabaseSettings.cs:6 (ConnectionString property)
- **ORM:** Entity Framework Core v7.0.4

**Data Flow:**
- **Context:** AuditableBaseDbContext, AuditableMultiTenantBaseDbContext
- **Multi-Tenancy:** Finbuckle.MultiTenant.EntityFrameworkCore v6.10.0 (global query filters)
- **Audit:** Automatic change tracking via ChangeTracker

**PHI Storage:**
- Patient demographics (name, DOB, Emirates ID, MRN, email, phone)
- Appointments (date/time, chief complaint, patient flags)
- Invoices (diagnosis codes, service dates, payments)
- Audit logs (PHI in OldValue/NewValue fields)

**Authentication:**
- Method: SQL authentication or Windows authentication
- Location: Connection string (DatabaseSettings.cs:6)
- **CRITICAL ISSUE:** Connection string in plain text (Security Debt #23)

**Failure Handling:**
- Retry: EF Core default retry on transient failures
- Timeout: SQL command timeout (30 seconds default)
- Fallback: Application exception, no graceful degradation

**Healthcare Impact:**
- **Patient Safety:** Database down = all clinical operations stop
- **Data Loss Risk:** No backup = catastrophic data loss
- **Audit Requirements:** Database access logs required
- **Multi-Tenancy:** Query filter failure = cross-tenant PHI exposure (HIPAA violation)
- **Retention:** 10 years for medical records, 7 years for financial, 6 years for audit
- **Mitigation URGENT:** Always Encrypted for PHI columns, connection string in Azure Key Vault, high availability (Always On), automated backups, multi-tenant isolation tests

---

## HIGH PRIORITY INTEGRATIONS (Medium Detail)

### 8. Email Sender Service
**Classification:** HIGH  
**Reason:** PHI in email body (patient names, appointment details), critical for patient communication

**Purpose:** Transactional email sending (appointment reminders, invoice delivery, password reset)

**Type:** Sync HTTP/REST (fire-and-forget pattern)  
**Endpoint:** POST /api/Email/Send/{emailKey} (IEmailSenderApi.cs:8)

**Data Flow:**
- EmailKey: appointment-reminder, invoice, password-reset
- EmailData: To (patient email = PHI), Subject, Body (appointment details = PHI), Attachments (PDF invoices)

**Failure Handling:**
- Returns Task (void) = no confirmation
- Email service down = logged but does not block operation
- Bounces not tracked

**Impact:** Patients miss appointment reminders → no-shows → revenue loss  
**Mitigation:** Retry queue, email status tracking, HIPAA-compliant email service (TLS, BAA)

---

### 9. Notifications Service
**Classification:** HIGH  
**Reason:** Video URLs for telehealth, real-time clinical alerts

**Purpose:** Video conference URL generation, SignalR push notifications

**Type:** Sync HTTP/REST  
**Endpoint:** GET /api/Notifications/GetVideoUrl (INotificationsApi.cs:7)

**Data Flow:**
- Request: AppointmentId, PatientId, NotificationType
- Response: Video URL (string)

**Failure Handling:**
- Video service unavailable = cannot conduct telehealth appointment
- Real-time push failure = provider misses check-in alert

**Impact:** Telehealth appointments blocked, provider workflow disruption  
**Mitigation:** Fallback video provider, SMS backup notification

---

### 10. Identity Service
**Classification:** HIGH  
**Reason:** Authentication, multi-tenant isolation (security critical)

**Purpose:** User authentication, JWT token generation, tenant resolution

**Type:** Sync HTTP/REST  
**Endpoints:**
- POST /api/v1/Users/ContactDetails (IIdentityApi.cs:8)
- GET /api/Tenants/{tenantId} (IIdentityApi.cs:11)

**Data Flow:**
- JWT Claims: sub (user ID), tenant (tenant ID), sa (super admin)
- Token validation, context resolution

**Failure Handling:**
- Identity service down = no authentication = application unusable
- Tenant resolution failure = multi-tenant isolation broken

**Impact:** Authentication failure stops all operations, cross-tenant data leakage  
**Mitigation:** Token caching, fallback authentication, tenant isolation tests

---

### 11. Provider Service
**Classification:** HIGH  
**Reason:** Provider details, reason for visit codes (clinical workflow)

**Purpose:** Provider lookup, chief complaint templates, qualifications

**Type:** Sync HTTP/REST  
**Endpoints:**
- GET /api/ProfessionalDetails/GetProviderReasonForVisits/{providerId} (IProviderApi.cs:8)
- GET /api/ProfessionalDetails/{providerId} (IProviderApi.cs:12)
- GET /api/Provider/{id} (IProviderApi.cs:16)

**Failure Handling:**
- Provider lookup failure = cannot display provider details
- Chief complaint templates unavailable = provider manual entry

**Impact:** Workflow slowdown, no provider selection  
**Mitigation:** Cache provider details, retry policy

---

### 12. Practice Service
**Classification:** HIGH  
**Reason:** Practice configuration, provider schedules (operational critical)

**Purpose:** Practice setup, appointment profiles, slot generation, staff management

**Type:** Sync HTTP/REST  
**Key Endpoints (24 total):**
- POST /api/Doctor/CalendarSlots3 (IPracticeApi.cs:28)
- GET /api/Doctor/AppointmentProfileList (IPracticeApi.cs:12)
- GET /api/Practice/PracticeSetupDetails/{practiceId} (IPracticeApi.cs:25)

**Failure Handling:**
- Calendar slot generation failure = no appointment availability
- Practice setup unavailable = cannot onboard new practices

**Impact:** No appointment booking, onboarding blocked  
**Mitigation:** Cache appointment profiles, retry slot generation

---

### 13. Organization Service
**Classification:** HIGH  
**Reason:** Tenant management, organization settings (multi-tenancy critical)

**Purpose:** Organization CRUD, settings management, user-organization mapping

**Type:** Sync HTTP/REST  
**Endpoints:**
- GET /api/v1/Organization/{id} (IOrganizationApi.cs:8)
- GET /api/v1/OrganizationSettings/{id} (IOrganizationApi.cs:11)
- POST /api/v1/Organization/list (IOrganizationApi.cs:14)

**Failure Handling:**
- Settings unavailable = default configuration used
- Organization lookup failure = tenant context broken

**Impact:** Multi-tenant isolation issues, incorrect settings applied  
**Mitigation:** Cache organization settings, tenant validation

---

### 14. gRPC Services (9 Protos)
**Classification:** HIGH  
**Reason:** High-performance inter-service communication alternative to REST

**Purpose:** Low-latency RPC for high-traffic operations

**Type:** gRPC with Protocol Buffers  
**Proto Files:**
- AppointmentManagement.proto (Scrips.Core.csproj:18)
- BillingManagement.proto (line 19)
- PatientManagement.proto (line 22)
- PracticeManagement.proto (line 24)
- ProviderManagement.proto (line 25)
- IdentityManagement.proto (line 20)
- PersonManagement.proto (line 23)
- MasterManagement.proto (line 21)
- OrganizationManagement.proto (line 26)

**Failure Handling:**
- gRPC client deadlines (timeout)
- Retry policies via gRPC interceptors

**Impact:** High-performance alternative to REST, reduces latency  
**Mitigation:** Fallback to REST APIs if gRPC unavailable

---

## LOW PRIORITY INTEGRATIONS (Brief)

### 15. Master Data Service (Lookup Endpoints)
**Classification:** LOW  
**Reason:** Static reference data, no PHI, graceful degradation

**Endpoints (4 total):**
- GET /api/Master/Gender (IMasterApi.cs:9)
- GET /api/Master/IdentityType (line 11)
- GET /api/Master/MaritalStatus (line 13)
- GET /api/Master/OwnerType (line 15)

**Purpose:** Dropdown lookup values (gender, marital status, identity types)  
**Failure:** Cached locally, graceful degradation, no blocking impact  
**PHI:** None

---

### 16. Person Service
**Classification:** LOW  
**Reason:** User demographics (non-PHI), optional data

**Endpoint:** GET /api/Persons/UserDetails (IPersonApi.cs:8)

**Purpose:** Person demographics separate from patient/provider roles  
**Failure:** User details unavailable, defaults to basic user info  
**PHI:** None (unless person is also a patient)

---

### 17. NABIDH
**Classification:** LOW  
**Reason:** Reference identifier only, no active API integration

**Type:** Reference data storage  
**Property:** NabidhAssigningAuthority (PracticeSetupDetails.cs:18)

**Purpose:** Store UAE National Backbone for Integrated Dubai Health practice identifier  
**Usage:** Practice registration compliance  
**Note:** Actual NABIDH API integration likely in separate microservice

---

### 18. SNOMED CT
**Classification:** LOW  
**Reason:** Terminology reference URL, no active API

**Type:** Medical terminology reference system  
**URL:** https://snomed.info/sct (PracticeValueSet.cs:23, Speciality.cs:18)

**Purpose:** Clinical terminology standardization (System property)  
**Usage:** Reference URL only, no API calls  
**Note:** SNOMED codes for specialties and value sets

---

## INTEGRATION SUMMARY

**Critical Issues Identified:**

1. **Dapr Audit Logging (CRITICAL):** Fire-and-forget = audit logs lost = HIPAA violation  
   **Fix:** Implement retry with dead letter queue, local fallback logging (16 hours)

2. **Patient Flags API (CRITICAL):** No retry = allergy warnings missed = patient death risk  
   **Fix:** URGENT retry policy, cached fallback, alerting (8 hours)

3. **No Resilience Patterns:** Zero Refit retry/circuit breaker configurations  
   **Fix:** Implement Polly policies across all HTTP clients (24 hours)

4. **Connection String Security (CRITICAL):** Plain text = credential exposure  
   **Fix:** Azure Key Vault integration, Always Encrypted for PHI (8 hours)

5. **Multi-Tenant Isolation (CRITICAL):** No automated tests for query filter failures  
   **Fix:** Integration tests for tenant isolation (16 hours)

**Total Critical Effort:** 72 hours (2 sprints)

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
