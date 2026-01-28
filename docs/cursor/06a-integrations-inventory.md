# Integration Inventory

## SUMMARY

- **Total integrations:** 15
- **Internal services:** 11 (HTTP/REST via Refit)
- **External APIs:** 2 (Azure Cognitive Search, SNOMED CT reference)
- **Databases:** 1 (SQL Server primary, PostgreSQL/MySQL configurable)
- **Message systems:** 1 (Dapr pub/sub)
- **Cache systems:** 0 (NOT IMPLEMENTED)

## INTERNAL SERVICE INTEGRATIONS

Scrips microservices calling each other via HTTP/REST (Refit interfaces):

| Service | Type | Base Path | File:Line |
|---------|------|-----------|-----------|
| Patient Service | HTTP/REST | /api/Patients | IPatientApi.cs:6-30 |
| Billing Service | HTTP/REST | /api/Billing | IBillingApi.cs:6-45 |
| Scheduling Service | HTTP/REST | /api/Appointment | ISchedulingApi.cs:6-31 |
| Email Sender Service | HTTP/REST | /api/Email | IEmailSenderApi.cs:6-9 |
| Notifications Service | HTTP/REST | /api/Notifications | INotificationsApi.cs:5-8 |
| Identity Service | HTTP/REST | /api/v1/Users, /api/Tenants | IIdentityApi.cs:6-13 |
| Provider Service | HTTP/REST | /api/ProfessionalDetails, /api/Provider | IProviderApi.cs:6-25 |
| Practice Service | HTTP/REST | /api/Practice, /api/Doctor, /api/Staff | IPracticeApi.cs:7-96 |
| Organization Service | HTTP/REST | /api/v1/Organization | IOrganizationApi.cs:6-19 |
| Person Service | HTTP/REST | /api/Persons | IPersonApi.cs:6-11 |
| Master Data Service | HTTP/REST | /api/Master, /api/AISearch | IMasterApi.cs:7-25 |

### Internal Service Communication Details

**Patient Service (IPatientApi.cs:6-30):**
- GET /api/Patients/{id} - Retrieve patient by ID (line 8)
- GET /api/Patients/PatientAddressList - Get patient addresses (line 11)
- GET /api/Patients/{patientId}/HealthInsuranceSponsor/{sponsorId} - Insurance details (line 16)
- GET /api/Patients/{patientId}/PatientCorporateSponsor/{sponsorId} - Corporate sponsor (line 22)
- GET /api/Patients/GetGuardianDetails - Guardian info for minors (line 28)

**Billing Service (IBillingApi.cs:6-45):**
- GET /api/Billing/ProviderBillingProfiles - Provider fee schedules (line 8)
- POST /api/Billing/Agreements/CalculatePrice - Multi-factor pricing (line 14)
- POST /api/Billing/Agreements/PreCalculatePrice - Price preview (line 20)
- GET /api/Billing/Invoice/{invoiceId} - Invoice retrieval (line 26)
- GET /api/Billing/Payment/AvailableBalance - Patient balance (line 29)
- GET /api/Billing/Payment/PaymentReceipts - Receipt history (line 34, 40)

**Scheduling Service (ISchedulingApi.cs:6-31):**
- POST /api/Appointment/Slots - Available time slots (line 8, 14)
- GET /api/Appointment/PatientFlag - Patient flags/alerts (line 20)
- GET /api/Appointment/GetById/{id} - Appointment details (line 23)
- POST /api/Appointment - Appointment search (line 26)

**Email Sender Service (IEmailSenderApi.cs:6-9):**
- POST /api/Email/Send/{emailKey} - Send transactional email (line 8)

**Notifications Service (INotificationsApi.cs:5-8):**
- GET /api/Notifications/GetVideoUrl - Video conference URL (line 7)

**Identity Service (IIdentityApi.cs:6-13):**
- POST /api/v1/Users/ContactDetails - User contact lookup (line 8)
- GET /api/Tenants/{tenantId} - Tenant information (line 11)

**Provider Service (IProviderApi.cs:6-25):**
- GET /api/ProfessionalDetails/GetProviderReasonForVisits/{providerId} - Chief complaints (line 8)
- GET /api/ProfessionalDetails/{providerId} - Provider details (line 12)
- GET /api/Provider/{id} - Provider by ID (line 16, 19)
- GET /api/Provider - Provider search (line 22)

**Practice Service (IPracticeApi.cs:7-96):**
- GET /api/Practice/ReminderList/{practiceId} - Appointment reminders (line 9)
- GET /api/Doctor/AppointmentProfileList - Provider appointment types (line 12)
- GET /api/Practice/PracticeSetupList/{organizationId} - Practice list (line 19)
- GET /api/Practice/PracticeSetupForPrimary/{organizationId} - Primary practice (line 22)
- GET /api/Practice/PracticeSetupDetails/{practiceId} - Practice details (line 25)
- POST /api/Doctor/CalendarSlots3 - Provider availability (line 28)
- POST /api/Doctor/CalendarProviders - Provider search (line 33)
- GET /api/Doctor/DoctorSetupList - Doctor list (line 38)
- GET /api/Staff/StaffList - Staff list (line 47)
- GET /api/Doctor/GetPracticeByProviderId/{id} - Provider practices (line 53)
- GET /api/Doctor/PractitionerRoles/{organizationId} - Practitioner roles (line 57)
- POST /api/Doctor/CalendarSlots2 - Slot availability v2 (line 67)
- POST /api/Doctor/CalendarProviders2 - Provider search v2 (line 72)
- GET /api/Doctor/GetPractitionerRoleDetails/{id} - Role details (line 77)
- GET /api/Practice/EditPracticeExamRoom/{practiceId} - Exam rooms (line 80)
- GET /api/Doctor/GetPracticeId/{userId} - Practice by user (line 85)
- GET /api/Staff/GetStaffId/{userId} - Staff by user (line 88)
- GET /api/Doctor/GetPractitionerRoleForUser/{userId} - User roles (line 91)

**Organization Service (IOrganizationApi.cs:6-19):**
- GET /api/v1/Organization/{id} - Organization by ID (line 8)
- GET /api/v1/OrganizationSettings/{id} - Organization settings (line 11)
- POST /api/v1/Organization/list - Organization list (line 14)
- GET /api/Organization/UpdateAndGetOrganization/{userId} - User's organization (line 17)

**Person Service (IPersonApi.cs:6-11):**
- GET /api/Persons/UserDetails - Person details by user ID (line 8)

**Master Data Service (IMasterApi.cs:7-25):**
- GET /api/Master/Gender - Gender lookup values (line 9)
- GET /api/Master/IdentityType - Identity types (line 11)
- GET /api/Master/MaritalStatus - Marital status options (line 13)
- GET /api/Master/OwnerType - Owner types (line 15)
- POST /api/AISearch/RAGSearch - AI clinical search (line 17)
- POST /api/AISearch/ClinicalSuggestions - ICD-10 suggestions (line 19)
- POST /api/AISearch/GenerateDocumentation - AI documentation (line 21)
- GET /api/AISearch/ByNameAndCategory - Chief complaint search (line 23)

### Authentication Pattern
All HTTP API calls require Authorization header with JWT bearer token:
- Pattern: `[Header("Authorization")] string token`
- Present on all endpoints across all 11 services
- JWT contains: user ID (sub claim), tenant ID (tenant claim), super admin flag (sa claim)

## EXTERNAL INTEGRATIONS

### Healthcare (Verified)

| Provider | Type | Destination | File:Line |
|----------|------|-------------|-----------|
| NABIDH | Reference Only | Practice identifier storage | PracticeSetupDetails.cs:18 |
| Malaffi | NOT FOUND | N/A | N/A |

**NABIDH Integration Details:**
- **File:** PracticeSetupDetails.cs:18
- **Property:** `NabidhAssigningAuthority` (string)
- **Purpose:** Store UAE National Backbone for Integrated Dubai Health practice identifier
- **Type:** Reference data storage only - no active API calls found in core library
- **Usage:** Practice registration compliance for UAE healthcare standards
- **Note:** Actual NABIDH API integration likely in separate microservice

**Malaffi Integration:**
- **Status:** NOT FOUND in codebase
- **Searched for:** "Malaffi", "malaffi", API endpoints, configuration
- **Result:** Zero matches

### Other External APIs

| Provider | Type | Destination | File:Line |
|----------|------|-------------|-----------|
| Azure Cognitive Search | Cloud AI Service | AI-powered chief complaint search | Scrips.Core.csproj:8 |
| SNOMED CT | Reference System | https://snomed.info/sct | PracticeValueSet.cs:23, Speciality.cs:18 |

**Azure Cognitive Search:**
- **Package:** Azure.Search.Documents v11.6.0
- **File:** Scrips.Core.csproj:8
- **Model:** ChiefComplaintDocument.cs:1-38
- **Purpose:** Vector search with 1536-dimension embeddings for clinical NLP
- **Features:** Semantic search, RAG (Retrieval Augmented Generation), ICD-10 suggestions
- **Integration:** Via Master Data Service API endpoints (IMasterApi.cs:17-24)
- **Note:** Actual search client implementation in Master Data microservice

**SNOMED CT:**
- **Type:** Medical terminology reference system
- **URL:** https://snomed.info/sct
- **File:** PracticeValueSet.cs:23, Speciality.cs:18
- **Purpose:** Clinical terminology standardization
- **Usage:** System property for practice value sets and specialties
- **Note:** Reference URL only, no active API calls

## DATABASE CONNECTIONS

| Type | Connection | File:Line |
|------|------------|-----------|
| SQL Server (Primary) | ConnectionString from config | DatabaseSettings.cs:6 |
| SQL Server | Default provider "mssql" | Startup.cs:23 |
| PostgreSQL (Npgsql) | Configurable (commented out) | Startup.cs:47-50 |
| MySQL | Configurable (commented out) | Startup.cs:56-59 |
| SQL Connection Validation | SqlConnectionStringBuilder | ConnectionStringValidator.cs:21 |
| SQL Connection Security | SqlConnectionStringBuilder | ConnectionStringSecurer.cs:34,36 |

**Database Configuration:**
- **Primary:** SQL Server (Microsoft.Data.SqlClient v5.1.0)
- **Configuration File:** DatabaseSettings.cs:5-6
- **Properties:**
  - `DBProvider` (string) - Database type identifier
  - `ConnectionString` (string) - Connection string with credentials
- **Default Provider:** "mssql" hardcoded at Startup.cs:23
- **Security:** ConnectionStringSecurer.cs:29-36 attempts to secure connection strings
- **Validation:** ConnectionStringValidator.cs:21 validates SQL Server connection strings
- **Multi-Database Support:** Commented out at Startup.cs:42-94
  - PostgreSQL support prepared (Npgsql) - line 47-50
  - MySQL support prepared (MySQL) - line 56-59
  - Oracle support prepared - line 61-63
  - Migration assemblies: Migrators.PostgreSQL, Migrators.MSSQL, Migrators.MySQL, Migrators.Oracle

**Entity Framework Core:**
- **Package:** Microsoft.EntityFrameworkCore v7.0.4
- **File:** Scrips.BaseDbContext.csproj:13
- **Contexts:**
  - AuditableBaseDbContext.cs - Base context with automatic audit logging
  - AuditableMultiTenantBaseDbContext.cs - Multi-tenant context with Finbuckle

## MESSAGE SYSTEMS

| Platform | Topics | File:Line |
|----------|--------|-----------|
| Dapr Pub/Sub | 10 event topics | Scrips.BaseDbContext.csproj:10 |
| Dapr Client | SaveAudit publishing | AuditableBaseDbContext.cs:61 |
| Topics Definition | Topic constants | Topics.cs:1-21 |

**Dapr Configuration:**
- **Package:** Dapr.Client v1.10.0
- **File:** Scrips.BaseDbContext.csproj:10
- **Publisher:** AuditableBaseDbContext.cs:57-62
- **Pub/Sub Component:** "pubsub" (configured externally)
- **Pattern:** Fire-and-forget (no retry on failure)

**Published Topics:**

| Topic Name | Trigger Event | File:Line |
|------------|---------------|-----------|
| SaveAudit | Entity change (Create/Update/Delete) | AuditableBaseDbContext.cs:61 |
| TenantCreated | New tenant onboarding | Topics.cs:5 |
| DoctorCreated | Provider registration | Topics.cs:7 |
| OrganizationCreated | Organization setup | Topics.cs:9 |
| OrganizationV1Created | Organization (versioned) | Topics.cs:10 |
| OrganizationV1Updated | Organization update | Topics.cs:11 |
| PracticeCreated | Practice registration | Topics.cs:13 |
| PracticeUpdated | Practice changes | Topics.cs:14 |
| PracticeActiveArchive | Practice archived | Topics.cs:15 |
| OrganizationSettingsCreated | Settings created | Topics.cs:17 |
| OrganizationSettingsUpdated | Settings updated | Topics.cs:18 |

**Event Publishing Details:**
- **Audit Events:** Published automatically on every SaveChangesAsync() call
- **Domain Events:** Published by consuming services (topic constants defined here)
- **Payload:** JSON serialized event data
- **Reliability:** No retry mechanism (see Risk #1 in architecture doc)
- **Consumers:** Implemented in separate microservices (not in this library)

**gRPC Services (Inter-Service Communication):**
- **Package:** Protobuf definitions compiled to gRPC clients
- **File:** Scrips.Core.csproj:18-26
- **Proto Definitions:** 9 files in Scrips.Core/Protos/
  - AppointmentManagement.proto (line 18)
  - BillingManagement.proto (line 19)
  - IdentityManagement.proto (line 20)
  - MasterManagement.proto (line 21)
  - PatientManagement.proto (line 22)
  - PersonManagement.proto (line 23)
  - PracticeManagement.proto (line 24)
  - ProviderManagement.proto (line 25)
  - OrganizationManagement.proto (line 26)
- **Client Type:** GrpcServices="Client" (line 18-26)
- **Purpose:** High-performance RPC alternative to REST APIs

## CACHE SYSTEMS

**Status:** NOT FOUND

- **Interface Defined:** ICacheService.cs:1-16
- **Implementation:** Missing
- **Package:** Microsoft.Extensions.Caching.Abstractions v7.0.0 (Scrips.Core.Application.csproj:15)
- **Methods Available:**
  - Get<T>, GetAsync<T> (lines 5-6)
  - Refresh, RefreshAsync (lines 8-9)
  - Remove, RemoveAsync (lines 11-12)
  - Set<T>, SetAsync<T> (lines 14-15)
- **Impact:** No distributed caching for multi-instance deployments
- **Note:** Abstraction exists for future implementation (Redis, Azure Cache, etc.)
- **Evidence:** Zero matches for "Redis", "StackExchange.Redis", "IDistributedCache" implementation

## INTEGRATION PATTERNS

**Authentication:**
- All HTTP service calls require JWT bearer token via Authorization header
- JWT claims: "sub" (user ID), "tenant" (tenant ID), "sa" (super admin)
- No API keys or basic auth found

**Multi-Tenancy:**
- Tenant context passed via JWT "tenant" claim
- Some endpoints use OrganizationId header
- Finbuckle.MultiTenant.EntityFrameworkCore v6.10.0 for data isolation

**Resilience:**
- No retry policies visible on Refit clients (see Risk #5 in architecture doc)
- No circuit breakers found
- No timeout configurations in core library
- Recommendation: Implement Polly policies

**Monitoring:**
- Serilog v2.12.0 for structured logging
- No APM/tracing configuration visible (Dapr provides distributed tracing externally)
- Error logging in exception middleware: ExceptionMiddleware.cs:11-86

## MISSING INTEGRATIONS

Based on healthcare practice management domain, these integrations are NOT FOUND:
- **Malaffi:** UAE health information exchange - NOT IMPLEMENTED
- **Payment Gateway:** Credit card processing - NOT IN CORE LIBRARY
- **Lab Interface:** HL7/FHIR lab orders/results - NOT IN CORE LIBRARY
- **Pharmacy Interface:** E-prescribing (NCPDP SCRIPT) - NOT IN CORE LIBRARY
- **Insurance Eligibility:** Real-time eligibility verification - NOT IN CORE LIBRARY
- **Claims Clearinghouse:** Electronic claim submission - NOT IN CORE LIBRARY

**Note:** These may be implemented in separate microservices not visible in this shared library.
