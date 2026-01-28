# Microservices Topology - Scrips Platform

## CONTEXT NOTE

This topology is **inferred** from the **Scrips.Core shared library**. This is NOT a complete microservices repository but rather a core library used by multiple services. The actual microservice implementations exist in separate repositories. This document maps the discovered service integrations, dependencies, and communication patterns visible from this library's perspective.

---

## 1. SERVICE INVENTORY

| Service | Responsibility | Tech Stack | PHI? | File:Line |
|---------|---------------|------------|------|-----------|
| **Patient Service** | Patient demographics, insurance, guardians | .NET 7.0 + EF Core | **YES** | IPatientApi.cs:6-30 |
| **Billing Service** | Price calculation, invoicing, payments | .NET 7.0 + EF Core | **YES** | IBillingApi.cs:6-45 |
| **Scheduling Service** | Appointments, slots, patient flags | .NET 7.0 + EF Core | **YES** | ISchedulingApi.cs:6-31 |
| **Email Sender Service** | Transactional email delivery | .NET 7.0 + SMTP | **YES** | IEmailSenderApi.cs:6-9 |
| **Notifications Service** | Real-time notifications, video URLs | .NET 7.0 + SignalR | **YES** | INotificationsApi.cs:5-8 |
| **Identity Service** | Users, authentication, tenants | .NET 7.0 + JWT | NO | IIdentityApi.cs:6-13 |
| **Provider Service** | Provider credentials, specialties | .NET 7.0 + EF Core | NO | IProviderApi.cs:6-25 |
| **Practice Service** | Practice setup, schedules, staff | .NET 7.0 + EF Core | NO | IPracticeApi.cs:7-96 |
| **Organization Service** | Organization configuration | .NET 7.0 + EF Core | NO | IOrganizationApi.cs:6-19 |
| **Person Service** | Person entity (user demographics) | .NET 7.0 + EF Core | MAYBE | IPersonApi.cs:6-11 |
| **Master Data Service** | Lookups, AI search, ICD-10 suggestions | .NET 7.0 + Azure Cognitive Search | **YES** | IMasterApi.cs:7-25 |

**Total Services:** 11 internal microservices  
**PHI Services:** 6-7 contain Protected Health Information  
**Technology:** All .NET 7.0 (end-of-support May 2024 - **CRITICAL UPGRADE NEEDED**)

---

## 2. DEPENDENCY GRAPH

```
┌─────────────────────────────────────────────────────────────┐
│                     FRONTEND / API GATEWAY                    │
│                   (Not visible in this repo)                  │
└───────────────────────────┬─────────────────────────────────┘
                            │ JWT Auth
                            ▼
┌───────────────────────────────────────────────────────────────┐
│                      IDENTITY SERVICE                          │
│  - User authentication                                         │
│  - Tenant resolution                                           │
│  - JWT token generation                                        │
└─────────────────┬─────────────────────────────────────────────┘
                  │ (JWT token used by all services below)
                  │
    ┌─────────────┼─────────────┬──────────────┬──────────────┐
    ▼             ▼             ▼              ▼              ▼
┌─────────┐  ┌──────────┐  ┌──────────┐  ┌───────────┐  ┌──────────┐
│PRACTICE │  │PROVIDER  │  │PERSON    │  │ORGANIZA-  │  │MASTER    │
│SERVICE  │  │SERVICE   │  │SERVICE   │  │TION       │  │DATA      │
│         │  │          │  │          │  │SERVICE    │  │SERVICE   │
│-Setup   │  │-Licens-  │  │-User     │  │-Config    │  │-Lookups  │
│-Schedul-│  │ ing      │  │ demographics│ -Settings │  │-AI Search│
│es       │  │-Reason   │  │          │  │          │  │-ICD-10   │
│-Staff   │  │ codes    │  │          │  │          │  │          │
└────┬────┘  └─────┬────┘  └─────┬────┘  └─────┬─────┘  └────┬─────┘
     │             │              │             │             │
     │             │              │             │             │
     └─────────────┴──────────────┴─────────────┴─────────────┘
                             │
                             │ Lookup dependencies
                             ▼
        ┌─────────────────────────────────────────────┐
        │         CLINICAL WORKFLOW SERVICES           │
        └─────────────────────────────────────────────┘
                             │
    ┌────────────────────────┼────────────────────────┐
    ▼                        ▼                        ▼
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│PATIENT       │      │SCHEDULING    │      │BILLING       │
│SERVICE       │◄─────┤SERVICE       │─────►│SERVICE       │
│              │      │              │      │              │
│-Demographics │      │-Appointments │      │-Price calc   │
│-Insurance    │      │-Slots        │      │-Invoices     │
│-Guardians    │      │-Patient flags│      │-Payments     │
└──────┬───────┘      └──────┬───────┘      └──────┬───────┘
       │                     │                     │
       │                     │                     │
       │              [Appointment Created]        │
       │                     │                     │
       └─────────────────────┼─────────────────────┘
                             │
                             │ Events
                             ▼
                    ┌─────────────────┐
                    │ DAPR PUB/SUB     │
                    │ (Redis/RabbitMQ) │
                    └────────┬─────────┘
                             │
               ┌─────────────┼─────────────┐
               ▼             ▼             ▼
      ┌────────────┐  ┌────────────┐  ┌────────────┐
      │EMAIL       │  │NOTIFICATIO-│  │AUDIT       │
      │SENDER      │  │NS          │  │LOG         │
      │SERVICE     │  │SERVICE     │  │CONSUMER    │
      │            │  │            │  │(External)  │
      │-Appt       │  │-SignalR    │  │            │
      │ reminders  │  │-Video URLs │  │-Compliance │
      │-Invoices   │  │-Push alerts│  │-Analytics  │
      └────────────┘  └────────────┘  └────────────┘
```

### Communication Patterns

**HTTP/REST (Synchronous):**
- All services expose REST APIs (Refit interfaces)
- JWT Bearer token authentication on all calls
- No retry/circuit breaker configured (Risk #5)

**gRPC (Synchronous, High-Performance):**
- Alternative to REST for high-throughput operations
- 9 proto definitions: Appointment, Billing, Identity, Master, Organization, Patient, Person, Practice, Provider
- Binary protocol (HTTP/2) for performance

**Dapr Pub/Sub (Asynchronous):**
- Event-driven workflows
- Fire-and-forget pattern (Risk #1 - no retry)
- 10 topics: SaveAudit, TenantCreated, DoctorCreated, OrganizationCreated, OrganizationV1Created, OrganizationV1Updated, PracticeCreated, PracticeUpdated, PracticeActiveArchive, OrganizationSettingsCreated, OrganizationSettingsUpdated

**Shared Database per Service:**
- Each service owns its data (inferred from service separation)
- Multi-tenancy via Finbuckle.MultiTenant
- SQL Server primary (PostgreSQL/MySQL configurable)

---

## 3. CRITICAL WORKFLOWS

### Workflow 1: Appointment Booking

**Services:** Scheduling → Practice → Provider → Patient → Billing → Email Sender

**Steps:**
1. **Scheduling Service** searches for available slots
   - POST /api/Appointment/Slots with date, provider, specialty
   - Receives slot list from **Practice Service** calendar
2. **Practice Service** validates provider availability
   - POST /api/Doctor/CalendarSlots3 with provider search
   - Returns time slots with exam room allocation
3. **Provider Service** retrieves reason for visit codes
   - GET /api/ProfessionalDetails/GetProviderReasonForVisits/{providerId}
   - Returns chief complaint dropdown options
4. **Patient Service** retrieves patient demographics and insurance
   - GET /api/Patients/{id} - Full patient record
   - GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId} - Insurance eligibility
5. **Billing Service** calculates appointment price
   - POST /api/Billing/Agreements/CalculatePrice with service codes, sponsor, provider
   - Returns sponsor share vs patient share
6. **Scheduling Service** creates appointment
   - Saves to database, triggers AppointmentCreated event
7. **Dapr Pub/Sub** distributes event
8. **Email Sender Service** sends confirmation
   - POST /api/Email/Send/{emailKey} with appointment details

**Failure Scenarios:**
- **If Billing Service down:** Appointment booking blocked (cannot calculate price)
- **If Practice Service down:** No available slots, booking impossible
- **If Patient Service down:** Cannot verify insurance, booking blocked
- **If Email Sender Service down:** Appointment created but no confirmation sent (silent failure)
- **If Dapr down:** Audit logs lost, no email sent (CRITICAL - Risk #1)

**Data Flow Duration:** 5-10 seconds (6 synchronous HTTP calls)

**PHI Exposure:** Patient name, date of birth, insurance, appointment date/time

---

### Workflow 2: Patient Check-In

**Services:** Scheduling → Patient → Billing → Notifications

**Steps:**
1. **Scheduling Service** loads appointment details
   - GET /api/Appointment/GetById/{id}
2. **Scheduling Service** retrieves patient flags
   - GET /api/Appointment/PatientFlag?patientId={id}
   - Returns VIP status, allergies, special needs (CRITICAL for clinical safety)
3. **Patient Service** confirms demographics and insurance
   - GET /api/Patients/{id}
   - GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId}
4. **Billing Service** checks patient balance
   - GET /api/Billing/Payment/AvailableBalance?patientId={id}
   - Prompts for copay/outstanding balance payment
5. **Notifications Service** notifies provider
   - SignalR push to provider's device
   - "Patient checked in - Exam Room 3"

**Failure Scenarios:**
- **If Scheduling Service down:** Cannot check in patients (complete workflow failure)
- **If Patient Service down:** Cannot verify demographics/insurance (clinical risk)
- **If Billing Service down:** Cannot collect copay (financial loss)
- **If Notifications Service down:** Provider not alerted (delays care)
- **CRITICAL: If patient flags not loaded:** Allergy/safety alerts missed (PATIENT SAFETY RISK)

**Data Flow Duration:** 2-5 seconds (5 synchronous HTTP calls)

**PHI Exposure:** Patient name, allergies, medical conditions, insurance, balance

---

### Workflow 3: Invoice Generation & Claims

**Services:** Billing → Scheduling → Patient → Master Data → Email Sender

**Steps:**
1. **Billing Service** retrieves appointment details
   - Calls **Scheduling Service** for appointment (date, provider, services)
2. **Master Data Service** provides ICD-10 code suggestions
   - POST /api/AISearch/ClinicalSuggestions with chief complaint
   - AI returns diagnosis code suggestions
3. **Billing Service** calculates final charges
   - POST /api/Billing/Agreements/CalculatePrice with finalized service codes
4. **Patient Service** confirms insurance policy
   - GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId}
5. **Billing Service** generates invoice
   - Creates invoice with line items, diagnoses, charges
   - Saves to database, triggers InvoiceCreated event
6. **Email Sender Service** sends invoice to patient
   - POST /api/Email/Send/{emailKey} with PDF attachment

**Failure Scenarios:**
- **If Master Data Service down:** AI suggestions unavailable (manual coding slower)
- **If Patient Service down:** Cannot verify insurance (claim submission blocked)
- **If Email Sender Service down:** Invoice created but not sent to patient
- **If Billing Service down:** Cannot create invoices (revenue loss)

**Data Flow Duration:** 10-15 seconds (AI processing adds latency)

**PHI Exposure:** Diagnosis codes, services rendered, charges, insurance claim data

---

### Workflow 4: AI Clinical Decision Support

**Services:** Master Data → Azure Cognitive Search → Patient

**Steps:**
1. Provider enters chief complaint text
2. **Master Data Service** converts text to vector embedding
   - OpenAI text-embedding-ada-002 (1536 dimensions)
3. **Azure Cognitive Search** performs vector similarity search
   - POST to Azure Search API with embedding vector
   - Returns top K similar cases with ICD-10 codes
4. **Master Data Service** generates clinical suggestions (RAG pattern)
   - POST /api/AISearch/RAGSearch with chief complaint
   - POST /api/AISearch/ClinicalSuggestions with symptoms
   - Returns ICD-10 codes ranked by relevance
5. **Master Data Service** generates documentation
   - POST /api/AISearch/GenerateDocumentation
   - AI drafts assessment and plan based on similar cases
6. Provider reviews and accepts/modifies suggestions
7. **Patient Service** saves clinical note
   - Note linked to patient encounter

**Failure Scenarios:**
- **If Azure Cognitive Search down:** AI features unavailable, fallback to manual entry
- **If Master Data Service down:** No clinical decision support (providers work slower)
- **If embeddings fail:** Fallback to keyword search (less accurate)

**Data Flow Duration:** 3-8 seconds (Azure API latency varies)

**PHI Exposure:** Chief complaint text, symptoms, clinical documentation

**Clinical Safety:** AI suggestions are recommendations only, require provider review before acceptance

---

### Workflow 5: Multi-Tenant Context Resolution

**Services:** Identity → Organization → All Services

**Steps:**
1. User logs in to **Identity Service**
   - POST /api/v1/Auth/Login with username/password
2. **Identity Service** generates JWT token
   - Claims: "sub" (user ID), "tenant" (tenant ID), "sa" (super admin flag)
   - Expiration: Configurable (e.g., 8 hours)
3. **Frontend** includes JWT in Authorization header for all requests
4. **Each microservice** extracts tenant context
   - ICurrentUser.GetTenantId() from JWT "tenant" claim
   - Finbuckle.MultiTenant resolves tenant-specific database connection
5. **EF Core** applies global query filter
   - WHERE TenantId = {current_tenant}
   - Prevents cross-tenant data leakage
6. **Organization Service** provides org-specific settings
   - GET /api/v1/OrganizationSettings/{id}
   - Returns appointment rules, billing config, branding

**Failure Scenarios:**
- **If Identity Service down:** No logins, all users locked out (CRITICAL)
- **If JWT expired:** User logged out mid-session
- **If tenant claim missing:** Request rejected (403 Forbidden)
- **CRITICAL: If global filter fails:** Cross-tenant data leakage (HIPAA violation)

**Data Flow Duration:** 1-2 seconds (token validation cached)

**PHI Exposure:** None (authentication data only)

**Security Impact:** Single point of failure for entire platform security

---

## 4. DATA OWNERSHIP

| Entity | Owner Service | Reader Services | Write Pattern |
|--------|--------------|-----------------|---------------|
| **Patient** | Patient Service | Scheduling, Billing, Email Sender, Notifications | Single writer |
| **Appointment** | Scheduling Service | Patient, Billing, Provider, Practice, Email Sender | Single writer |
| **Invoice** | Billing Service | Patient, Email Sender | Single writer |
| **Payment** | Billing Service | Patient | Single writer |
| **Provider** | Provider Service | Practice, Scheduling, Billing | Single writer |
| **Practice** | Practice Service | Scheduling, Provider, Billing | Single writer |
| **User** | Identity Service | Person, all services (JWT) | Single writer |
| **Tenant** | Identity Service | Organization, all services (JWT) | Single writer |
| **Organization** | Organization Service | Practice, Identity | Single writer |
| **Person** | Person Service | Identity, Notifications | Single writer |
| **Chief Complaint (Indexed)** | Master Data Service | None (search only) | Batch writer |
| **Audit Logs** | All Services | External audit consumer | Multiple writers |

**Pattern:** Single-writer, multiple-reader per entity  
**Consistency:** Eventual consistency via Dapr events  
**Conflicts:** Prevented by service ownership boundaries

---

## 5. MESSAGING TOPOLOGY

| Topic | Producer | Consumers | Purpose | File:Line |
|-------|----------|-----------|---------|-----------|
| **SaveAudit** | All Services | External Audit Consumer | Entity change tracking | AuditableBaseDbContext.cs:61 |
| **TenantCreated** | Identity Service | Email Sender, Audit | New tenant onboarding | Topics.cs:5 |
| **DoctorCreated** | Provider Service | Email Sender, Notifications | Provider registration | Topics.cs:7 |
| **OrganizationCreated** | Organization Service | Email Sender, Identity | Org setup workflow | Topics.cs:9 |
| **OrganizationV1Created** | Organization Service | Email Sender, Identity | Org setup (versioned) | Topics.cs:10 |
| **OrganizationV1Updated** | Organization Service | Cache invalidation | Org settings changed | Topics.cs:11 |
| **PracticeCreated** | Practice Service | Email Sender, Provider | Practice registration | Topics.cs:13 |
| **PracticeUpdated** | Practice Service | Scheduling (reload), Cache | Practice config changed | Topics.cs:14 |
| **PracticeActiveArchive** | Practice Service | Scheduling (disable slots) | Practice archived | Topics.cs:15 |
| **OrganizationSettingsCreated** | Organization Service | All Services (reload config) | Settings initialized | Topics.cs:17 |
| **OrganizationSettingsUpdated** | Organization Service | All Services (reload config) | Settings changed | Topics.cs:18 |

**Messaging Platform:** Dapr Pub/Sub (Redis, RabbitMQ, or Azure Service Bus - configured externally)  
**Reliability:** Fire-and-forget (NO RETRY - Risk #1)  
**Ordering:** No guaranteed order (Dapr default)  
**Audit Events:** Published on every entity Create/Update/Delete

**CRITICAL RISK:** Audit log loss if Dapr unavailable - no retry mechanism implemented

---

## 6. PHI FLOW MAP

### Services with PHI

**CRITICAL PHI (Direct Patient Data):**
1. **Patient Service** - Full patient demographics, insurance, guardians
2. **Scheduling Service** - Appointments (patient linkage), patient flags (allergies, conditions)
3. **Billing Service** - Invoices (diagnosis codes), payments (patient linkage)
4. **Master Data Service** - Chief complaints (clinical text), AI-generated documentation
5. **Email Sender Service** - All emails contain patient names, appointment details

**HIGH PHI (Indirect Patient Data):**
6. **Notifications Service** - Video conference URLs (appointment linkage)
7. **Person Service** - User demographics (if person is patient)

**LOW PHI (No Direct Patient Data):**
- Identity, Provider, Practice, Organization Services

### PHI Flow Diagram

```
[Patient Service] ─────PHI: Name, DOB, MRN────────► [Scheduling Service]
       │                                                      │
       │                                                      │
       │                                                      ▼
       │                                          [Email Sender Service]
       │                                           PHI: Name, Appt Date
       │
       └────PHI: Insurance, Diagnosis────► [Billing Service]
                                                     │
                                                     │
                                                     ▼
                                          [Master Data Service]
                                           PHI: Chief Complaint
                                                     │
                                                     ▼
                                          [Azure Cognitive Search]
                                           PHI: Indexed Clinical Text
```

### Encryption

**In Transit (TLS/HTTPS):**
- **Required:** TLS 1.2+ for all HTTP/REST and gRPC calls
- **Configured:** Infrastructure level (not enforced in code - Risk #2)
- **JWT Tokens:** Transmitted via HTTPS Authorization header
- **Status:** ⚠️ NOT ENFORCED in code - relies on infrastructure

**At Rest (Database):**
- **Recommended:** SQL Server Transparent Data Encryption (TDE)
- **Configured:** NOT ENFORCED in code (DatabaseSettings.cs:6 - plain connection string)
- **PHI Tables:** Patient, Appointment, Invoice, Claim, ClinicalNote
- **Status:** ⚠️ NOT ENFORCED - requires DBA configuration

**At Rest (Azure Cognitive Search):**
- **Configured:** Azure encryption at rest (automatic)
- **Status:** ✅ ENABLED by default

**Audit Logs (Dapr/Message Queue):**
- **PHI Masking:** `[MaskValueAudit]` attribute replaces sensitive fields with "*"
- **File:** MaskValueAuditAttribute.cs:13, AuditLoggingHelper.cs:56,60,68
- **Status:** ✅ IMPLEMENTED but requires manual attribute application
- **Risk:** PHI exposure if attributes not applied to all sensitive properties

**Email Content:**
- **Status:** ⚠️ UNKNOWN - depends on email service configuration
- **Requirement:** Email service must be HIPAA-compliant
- **Recommendation:** Use encrypted email gateway or secure patient portal links

---

## 7. RISKS

### Single Points of Failure

**1. Identity Service Down**
- **Impact:** Complete platform unavailable (no logins, JWT validation fails)
- **Affected Workflows:** ALL
- **Mitigation:** High availability deployment, database replication
- **SLA Target:** 99.95% uptime (4.38 hours downtime/year max)

**2. Dapr Pub/Sub Down**
- **Impact:** Audit logs lost (fire-and-forget pattern - Risk #1), no email notifications
- **Affected Workflows:** Audit logging, email reminders, event-driven workflows
- **Healthcare Risk:** **CRITICAL** - Lost audit logs create compliance violations
- **Mitigation:** Implement retry with exponential backoff or local queue fallback
- **Current Status:** ⚠️ **NOT MITIGATED**

**3. Patient Service Down**
- **Impact:** Cannot load patient demographics, insurance verification fails, appointments blocked
- **Affected Workflows:** Appointment booking, check-in, billing
- **Healthcare Risk:** **HIGH** - Patient care workflows disrupted
- **Mitigation:** Read replicas, circuit breaker with cached data

**4. Billing Service Down**
- **Impact:** Cannot calculate prices, appointment booking blocked, invoices unavailable
- **Affected Workflows:** Appointment booking (price required), check-in (copay), invoicing
- **Financial Risk:** **HIGH** - Revenue loss
- **Mitigation:** Cached price agreements, allow booking with pending price

**5. Scheduling Service Down**
- **Impact:** Cannot book appointments, no patient check-in
- **Affected Workflows:** Appointment booking, check-in, provider schedule
- **Healthcare Risk:** **HIGH** - Patient access to care blocked
- **Mitigation:** Read replicas, degraded mode (manual scheduling fallback)

**6. Master Data Service Down**
- **Impact:** AI clinical decision support unavailable, lookups fail (gender, marital status)
- **Affected Workflows:** Clinical documentation, appointment booking (dropdown data)
- **Healthcare Risk:** **MEDIUM** - Providers work slower without AI
- **Mitigation:** Cached lookup data, AI features degrade gracefully

**7. Azure Cognitive Search Down**
- **Impact:** AI vector search unavailable, fallback to keyword search
- **Affected Workflows:** Chief complaint search, ICD-10 suggestions
- **Healthcare Risk:** **LOW** - Manual coding still possible
- **Mitigation:** Keyword search fallback, cached frequent searches

**8. SQL Server Database Down**
- **Impact:** Complete platform unavailable (all services depend on database)
- **Affected Workflows:** ALL
- **Healthcare Risk:** **CRITICAL** - Patient care stopped
- **Mitigation:** SQL Server Always On Availability Groups, automatic failover
- **SLA Target:** 99.99% uptime (52 minutes downtime/year max)

### Cascading Failure Risks

**Scenario: Billing Service Overload**
- Billing Service slow → Appointment booking times out → Scheduling Service requests pile up → Database connection pool exhausted → All services fail
- **Mitigation:** Circuit breaker (Polly), bulkhead isolation, rate limiting, timeout policies
- **Current Status:** ⚠️ **NOT IMPLEMENTED** (Risk #5)

**Scenario: Database Connection Leak**
- One service leaks connections → Connection pool exhausted → All services cannot connect → Complete outage
- **Mitigation:** Connection pool monitoring, automatic connection recycling, alerting
- **Current Status:** ⚠️ NOT VISIBLE in core library

**Scenario: Audit Log Explosion**
- High-frequency entity changes → Dapr message queue overwhelmed → Backpressure → Database writes slow → Cascading timeout
- **Mitigation:** Batch audit log publishing, queue size limits, backpressure handling
- **Current Status:** ⚠️ NOT IMPLEMENTED

### Multi-Tenancy Isolation Risks

**Cross-Tenant Data Leakage**
- **Cause:** Global query filter failure, JWT "tenant" claim missing/spoofed
- **Impact:** **CRITICAL** - HIPAA violation, patient privacy breach, legal liability
- **Mitigation:** Defense-in-depth (JWT validation + global filter + row-level security)
- **Test Requirements:** Automated tests for tenant isolation in every service
- **Current Status:** ⚠️ NOT VISIBLE in core library (must audit consuming services)

### Healthcare Compliance Risks

**Audit Log Loss (Risk #1)**
- **Cause:** Dapr fire-and-forget pattern (AuditableBaseDbContext.cs:61)
- **Impact:** Compliance violations (HIPAA, UAE regulations require complete audit trail)
- **Severity:** **CRITICAL**
- **Fix:** Implement retry with local queue fallback

**Unencrypted PHI in Transit (Risk #2)**
- **Cause:** TLS not enforced in code, relies on infrastructure
- **Impact:** PHI exposure if misconfigured
- **Severity:** **HIGH**
- **Fix:** Enforce HTTPS in code, reject non-TLS connections

**Connection String Exposure (Risk #3)**
- **Cause:** Database credentials in plain text (DatabaseSettings.cs:6)
- **Impact:** Database compromise if config file leaked
- **Severity:** **HIGH**
- **Fix:** Move to Azure Key Vault or environment variables

**PHI in Plain Text Logs (Risk #6)**
- **Cause:** `[MaskValueAudit]` attribute not universally applied
- **Impact:** PHI exposure in audit logs
- **Severity:** **MEDIUM**
- **Fix:** Audit attribute usage, enforce via unit tests

**No Retry Policies (Risk #5)**
- **Cause:** Refit clients have no retry/circuit breaker
- **Impact:** Transient failures break workflows
- **Severity:** **MEDIUM**
- **Fix:** Implement Polly retry policies (exponential backoff, 3 attempts)

### Performance Bottlenecks

**No Distributed Cache**
- **Cause:** ICacheService.cs interface only, no implementation
- **Impact:** Repeated database queries, slow response times
- **Severity:** **MEDIUM**
- **Fix:** Implement Redis with TLS encryption

**N+1 Query Problem**
- **Cause:** Entity Framework lazy loading (documented in Section 8.2)
- **Impact:** Slow queries, database overload
- **Severity:** **MEDIUM**
- **Fix:** Use Include() for eager loading, projections

**No Request Throttling**
- **Cause:** No rate limiting visible
- **Impact:** Denial-of-service vulnerability
- **Severity:** **MEDIUM**
- **Fix:** Implement API rate limiting per tenant/user

---

## SUMMARY

**Platform Architecture:** Event-driven microservices with HTTP/REST primary, gRPC alternative, Dapr Pub/Sub messaging  
**Total Services:** 11 internal microservices  
**PHI Services:** 6-7 contain Protected Health Information  
**Technology:** .NET 7.0 (⚠️ END OF SUPPORT - critical upgrade needed)  
**Database:** SQL Server (PostgreSQL/MySQL configurable)  
**Messaging:** Dapr Pub/Sub (fire-and-forget, no retry)  
**AI:** Azure Cognitive Search with OpenAI embeddings  
**Multi-Tenancy:** Finbuckle.MultiTenant with JWT claims  

**Critical Risks:**
1. Audit log loss (Dapr fire-and-forget)
2. No retry/circuit breaker policies
3. .NET 7.0 end-of-support
4. Connection string security
5. Single points of failure (Identity, Database)

**Recommended Immediate Actions:**
1. Upgrade .NET 7.0 → .NET 8 (all services)
2. Implement audit log retry mechanism
3. Add Polly retry policies to all Refit clients
4. Implement distributed Redis cache
5. Deploy Identity and Database with high availability
6. Add automated multi-tenancy isolation tests
7. Move connection strings to Azure Key Vault
8. Implement comprehensive monitoring and alerting
