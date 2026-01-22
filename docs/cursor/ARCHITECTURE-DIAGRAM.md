# Architecture Diagrams - Scrips.Core

Visual guide to understanding the Scrips.Core architecture with ASCII diagrams.

**Purpose:** Quick visual reference for architecture patterns, data flows, and system interactions.

---

## Table of Contents
1. [System Context](#1-system-context)
2. [Clean Architecture Layers](#2-clean-architecture-layers)
3. [CQRS Flow](#3-cqrs-flow-commandquery-pattern)
4. [Query Flow (REST API)](#4-query-flow-rest-api-request--response)
5. [Multi-Tenancy Flow](#5-multi-tenancy-flow-jwt-validation)
6. [Dependency Graph](#6-dependency-graph-external-services)
7. [Database Schema Overview](#7-database-schema-overview)
8. [Request Pipeline](#8-request-pipeline-middleware-order)
9. [Event Processing](#9-event-processing-dapr-pubsub)
10. [Real-World Data Flow Example](#10-real-world-data-flow-example)
11. [Quick Navigation](#11-quick-navigation-table)

---

## 1. System Context

**Where Scrips.Core fits in the overall platform:**

```
┌────────────────────────────────────────────────────────────────────┐
│                    Scrips Healthcare Platform                       │
│                                                                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐            │
│  │   Patient    │  │  Scheduling  │  │    Billing   │            │
│  │   Service    │  │   Service    │  │   Service    │   + 8 more │
│  │              │  │              │  │              │            │
│  │ PatientDb    │  │ SchedulingDb │  │  BillingDb   │            │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘            │
│         │                 │                 │                      │
│         └─────────────────┴─────────────────┘                      │
│                           │                                         │
│                           │ All import:                             │
│                           ↓                                         │
│                   ┌────────────────┐                               │
│                   │  Scrips.Core   │ ← YOU ARE HERE               │
│                   │  (NuGet Pkg)   │                               │
│                   │                │                               │
│                   │  • DTOs        │                               │
│                   │  • API Clients │                               │
│                   │  • Base Classes│                               │
│                   │  • Audit Logic │                               │
│                   └────────┬───────┘                               │
│                            │                                        │
│         ┌──────────────────┼──────────────────┐                   │
│         ↓                  ↓                  ↓                    │
│  ┌────────────┐     ┌────────────┐    ┌────────────┐            │
│  │ SQL Server │     │    Dapr    │    │   Azure    │            │
│  │ (Each DB)  │     │  (Pub/Sub) │    │   Search   │            │
│  │ Multi-     │     │            │    │  (AI/RAG)  │            │
│  │ Tenant     │     │  Topics:   │    │            │            │
│  └────────────┘     │  SaveAudit │    │ Chief      │            │
│                     │  *Created  │    │ Complaints │            │
│                     └────────────┘    └────────────┘            │
└────────────────────────────────────────────────────────────────────┘

Legend:
  ┌────┐
  │Svc │  = Microservice (runs independently)
  └────┘
  
  ┌────┐
  │ Db │  = Database (one per service)
  └────┘
  
  → = Depends on / Imports
```

**Key Points:**
- Scrips.Core is NOT a service (it's a shared library)
- All microservices import Scrips.Core as a NuGet package
- Provides consistency across 11+ microservices
- Single source of truth for DTOs and contracts

---

## 2. Clean Architecture Layers

**Dependency direction: Inward only (outer → inner, never inner → outer)**

```
┌──────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                            │
│  Scrips.WebApi/                                                  │
│  • BaseApiController.cs (MediatR integration)                    │
│  • VersionedApiController.cs                                     │
│  • VersionNeutralApiController.cs                                │
│                                                                  │
│  Controllers in consuming microservices inherit from these       │
└────────────────────────┬─────────────────────────────────────────┘
                         │ depends on ↓
┌──────────────────────────────────────────────────────────────────┐
│                    APPLICATION LAYER                             │
│  Scrips.Core.Application/                                        │
│  • Interfaces (IEventPublisher, ICacheService, etc.)             │
│  • Common utilities (events, caching, file storage)              │
│  • Auditing (IAuditService)                                      │
│  • Job scheduling (IJobService)                                  │
│                                                                  │
│  ⚠️ Interfaces only - implementations in Infrastructure          │
└────────────────────────┬─────────────────────────────────────────┘
                         │ depends on ↓
┌──────────────────────────────────────────────────────────────────┐
│                      DOMAIN LAYER                                │
│  Scrips.Core.Domain/                                             │
│  • Business entities                                             │
│  • Contracts (ISoftDelete, IAuditableEntity, etc.)               │
│  • Domain logic                                                  │
│                                                                  │
│  ✅ No dependencies on other layers                              │
└────────────────────────┬─────────────────────────────────────────┘
                         │ depends on ↓
┌──────────────────────────────────────────────────────────────────┐
│                       CORE LAYER                                 │
│  Scrips.Core/                                                    │
│  • DTOs (PatientResponse, AppointmentRequest, etc.)              │
│  • Models (9 domain areas)                                       │
│  • Enums (AppointmentStatus, BillingType, etc.)                 │
│  • API Clients (11 Refit interfaces)                             │
│  • gRPC Protos (9 service definitions)                           │
│                                                                  │
│  ✅ Pure POCO objects - ZERO dependencies                        │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER (Cross-Cutting)            │
│  Scrips.Infrastructure/ & Scrips.BaseDbContext/                  │
│  • Database contexts (AuditableBaseDbContext)                    │
│  • Auth (JWT configuration)                                      │
│  • Middleware (tenant resolution, exception handling)            │
│  • Persistence (database settings)                               │
│                                                                  │
│  Can depend on any layer except Presentation                     │
└──────────────────────────────────────────────────────────────────┘
```

**File Location Examples:**
```
Domain     → Scrips.Core.Domain/Contracts/ISoftDelete.cs
Application→ Scrips.Core.Application/Common/Caching/ICacheService.cs
Core       → Scrips.Core/Models/Patient/PatientResponse.cs
Infra      → Scrips.BaseDbContext/AuditableBaseDbContext.cs
Present    → Scrips.WebApi/BaseApiController.cs
```

---

## 3. CQRS Flow (Command/Query Pattern)

**Using MediatR for separation of reads and writes:**

```
┌─────────────────────────────────────────────────────────────────┐
│                    HTTP Request                                  │
│  POST /api/Patient (Create)  or  GET /api/Patient/123 (Read)   │
└────────────────────┬────────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────────┐
│                  Controller (Presentation)                       │
│  public class PatientsController : BaseApiController            │
│  {                                                               │
│      [HttpPost]                                                  │
│      public async Task<PatientResponse> Create(                 │
│          CreatePatientCommand command)                           │
│      {                                                           │
│          return await Mediator.Send(command); ← MediatR          │
│      }                                                           │
│  }                                                               │
└────────────────────┬────────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────────────┐
│                     MediatR Pipeline                             │
│  • Resolves handler                                              │
│  • Can add behaviors (validation, logging, etc.)                 │
└───────────┬──────────────────────────────────┬──────────────────┘
            ↓ COMMAND (Write)                   ↓ QUERY (Read)
┌──────────────────────────────┐  ┌────────────────────────────────┐
│  CreatePatientCommandHandler │  │  GetPatientQueryHandler        │
│                              │  │                                │
│  public async Task<Response> │  │  public async Task<Response>   │
│    Handle(Command, CT)       │  │    Handle(Query, CT)           │
│  {                           │  │  {                             │
│    // 1. Validate             │  │    // 1. Build query           │
│    // 2. Create entity        │  │    // 2. Execute (read-only)   │
│    // 3. Add to context       │  │    // 3. Map to DTO            │
│    // 4. SaveChangesAsync()   │  │    // 4. Return result         │
│    //    → Triggers audit log │  │    //    (no SaveChanges)      │
│    // 5. Return result        │  │  }                             │
│  }                           │  │                                │
└──────────────────────────────┘  └────────────────────────────────┘
            ↓                                   ↓
┌──────────────────────────────┐  ┌────────────────────────────────┐
│  DbContext.SaveChangesAsync()│  │  DbContext.ToListAsync()       │
│  • Detects changes            │  │  • Read-only query             │
│  • Logs to audit              │  │  • No audit logging            │
│  • Publishes to Dapr          │  │  • Fast (no change tracking)   │
└──────────────────────────────┘  └────────────────────────────────┘
            ↓                                   ↓
┌─────────────────────────────────────────────────────────────────┐
│                      HTTP Response                               │
│  Status: 201 Created         or   Status: 200 OK                │
│  Body: PatientResponse DTO   or   Body: PatientResponse DTO     │
└─────────────────────────────────────────────────────────────────┘
```

**Benefits:**
- Clear separation of concerns (reads vs writes)
- Easy to optimize queries separately
- Testable in isolation
- Scalable (can split to different databases if needed)

---

## 4. Query Flow (REST API Request → Response)

**Complete lifecycle of a GET request:**

```
┌──────────────────────────────────────────────────────────────────┐
│  1. HTTP GET /api/Patient/123                                    │
│     Headers: Authorization: Bearer eyJhbGc...                    │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  2. API Gateway / Load Balancer                                  │
│     • TLS termination                                            │
│     • Rate limiting                                              │
│     • Routes to Patient.API service                              │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  3. Middleware Pipeline (in Patient.API)                         │
│     a. ExceptionHandling → Catch and format errors               │
│     b. Authentication → Validate JWT token                       │
│     c. TenantResolution → Extract "tenant" claim from JWT        │
│     d. Authorization → Check user permissions                    │
│     e. Logging → Log request details                             │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  4. Controller (Scrips.WebApi/BaseApiController)                 │
│     [HttpGet("{patientId}")]                                     │
│     [Authorize(ScripsPermissions.Patients.View)]                 │
│     public async Task<PatientResponse> Get(Guid patientId)       │
│     {                                                            │
│         return await Mediator.Send(                              │
│             new GetPatientQuery { PatientId = patientId });      │
│     }                                                            │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  5. MediatR → Routes to GetPatientQueryHandler                   │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  6. Query Handler (Application Layer)                            │
│     public async Task<PatientResponse> Handle(...)               │
│     {                                                            │
│         var patient = await _context.Patients                    │
│             .FirstOrDefaultAsync(p => p.Id == patientId);        │
│                                                                  │
│         // ⚠️ Global query filter automatically adds:            │
│         //    WHERE OrganizationId = @tenantId                   │
│                                                                  │
│         if (patient == null)                                     │
│             throw new NotFoundException();                       │
│                                                                  │
│         return _mapper.Map<PatientResponse>(patient);            │
│     }                                                            │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  7. EF Core → Generates SQL                                      │
│     SELECT * FROM Patients                                       │
│     WHERE Id = @patientId                                        │
│       AND OrganizationId = @tenantId  ← Multi-tenancy filter    │
│       AND DeletedOn IS NULL           ← Soft delete filter      │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  8. SQL Server → Executes query                                  │
│     Returns: { Id, FirstName, LastName, DOB, ... }              │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  9. Mapper → Entity to DTO                                       │
│     Patient entity → PatientResponse DTO                         │
│     (Scrips.Core/Models/Patient/PatientResponse.cs)              │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  10. HTTP Response                                               │
│      Status: 200 OK                                              │
│      Content-Type: application/json                              │
│      Body:                                                       │
│      {                                                           │
│        "patientId": "123...",                                    │
│        "firstName": "John",                                      │
│        "lastName": "Doe",                                        │
│        ...                                                       │
│      }                                                           │
└──────────────────────────────────────────────────────────────────┘
```

**⚠️ Failure Scenario:**

```
Step 7: Patient not found or belongs to different tenant
        ↓
Handler throws NotFoundException
        ↓
Exception middleware catches
        ↓
Returns: 404 Not Found
         { "error": "Patient not found" }
         (No PHI in error message!)
```

---

## 5. Multi-Tenancy Flow (JWT Validation)

**How tenant isolation is enforced at every layer:**

```
┌──────────────────────────────────────────────────────────────────┐
│  1. User Login (Identity Service)                               │
│     POST /api/Auth/Login                                         │
│     Body: { username: "dr.smith", password: "..." }             │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  2. Generate JWT Token                                           │
│     {                                                            │
│       "sub": "user-guid-789",          ← User ID                │
│       "tenant": "org-guid-456",        ← ⚠️ CRITICAL: Tenant ID │
│       "sa": "false",                    ← Super Admin flag       │
│       "email": "dr.smith@clinic.com",                            │
│       "exp": 1234567890,                                         │
│       "iss": "scrips-identity"                                   │
│     }                                                            │
│     Returns: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...           │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  3. Subsequent API Request                                       │
│     GET /api/Patient/123                                         │
│     Headers:                                                     │
│       Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9│
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  4. JWT Middleware (Scrips.Infrastructure/Auth/)                 │
│     • Validates signature                                        │
│     • Checks expiration                                          │
│     • Extracts claims                                            │
│     • Sets User.Identity                                         │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  5. Tenant Resolution Middleware                                 │
│     var tenantId = User.FindFirst("tenant")?.Value;              │
│                                                                  │
│     if (tenantId == null)                                        │
│         return 401 Unauthorized;                                 │
│                                                                  │
│     // Store in HttpContext for Finbuckle                        │
│     HttpContext.Items["__tenant__"] = tenantId;                  │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  6. Controller → Handler → DbContext                             │
│     Query: _context.Patients.Where(p => p.Id == 123)            │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  7. Finbuckle.MultiTenant (Global Query Filter)                  │
│     • Intercepts EF Core query                                   │
│     • Gets tenantId from HttpContext                             │
│     • Adds filter: AND OrganizationId = @tenantId                │
│                                                                  │
│     Original: SELECT * FROM Patients WHERE Id = 123              │
│     Modified: SELECT * FROM Patients                             │
│               WHERE Id = 123                                     │
│                 AND OrganizationId = 'org-guid-456' ← Added!    │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  8. SQL Server                                                   │
│     • Executes filtered query                                    │
│     • IMPOSSIBLE to access other tenants' data                   │
│     • Even if patient ID exists in another tenant, returns NULL  │
└──────────────────────────────────────────────────────────────────┘

⚠️ CRITICAL SECURITY: If OrganizationId is missing from an entity:
   → Global query filter cannot apply
   → Data leak across tenants
   → HIPAA VIOLATION
```

**⚠️ Known Risks:**
- Using `.IgnoreQueryFilters()` bypasses all filtering (NEVER use unless super admin)
- Missing `OrganizationId` on entities = no tenant isolation
- JWT without "tenant" claim = request rejected

---

## 6. Dependency Graph (External Services)

**What Scrips.Core connects to:**

```
                      ┌─────────────────┐
                      │  Scrips.Core    │
                      │  (This Library) │
                      └────────┬────────┘
                               │
         ┌─────────────────────┼─────────────────────┐
         │                     │                     │
         ↓                     ↓                     ↓
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│   SQL Server     │  │      Dapr        │  │  Azure Search    │
│ (Multi-Tenant)   │  │   (Pub/Sub)      │  │    (AI/RAG)      │
│                  │  │                  │  │                  │
│ • Patient DB     │  │ Topics:          │  │ • Chief          │
│ • Scheduling DB  │  │  • SaveAudit     │  │   Complaints     │
│ • Billing DB     │  │  • *Created      │  │ • Vector Search  │
│ • Practice DB    │  │  • *Updated      │  │ • 1536-dim       │
│ • ...11 DBs      │  │                  │  │   embeddings     │
│                  │  │ Components:      │  │                  │
│ Via: EF Core 7.0 │  │  • Redis (msg)   │  │ Via: Azure.      │
│                  │  │  • RabbitMQ      │  │  Search.         │
│                  │  │  • Azure SvcBus  │  │  Documents       │
│                  │  │                  │  │  v11.6.0         │
└──────────────────┘  └──────────────────┘  └──────────────────┘

External HTTP APIs (via Refit):
├── IPatientApi          → Patient microservice
├── ISchedulingApi       → Scheduling microservice
├── IBillingApi          → Billing microservice
├── IPracticeApi         → Practice microservice
├── IProviderApi         → Provider microservice
├── IOrganizationApi     → Organization microservice
├── IIdentityApi         → Identity microservice
├── IPersonApi           → Person microservice
├── IMasterApi           → Master Data microservice
├── IEmailSenderApi      → Email service
└── INotificationsApi    → Notifications service

gRPC Services (9 proto definitions):
├── AppointmentManagement.proto
├── BillingManagement.proto
├── IdentityManagement.proto
├── MasterManagement.proto
├── PatientManagement.proto
├── PersonManagement.proto
├── PracticeManagement.proto
├── ProviderManagement.proto
└── OrganizationManagement.proto

⚠️ Critical Dependencies (Must Be Available):
   • Dapr          → Audit logs lost if unavailable (KNOWN ISSUE)
   • SQL Server    → All operations fail
   • Identity API  → Cannot authenticate users

⚠️ Optional Dependencies (Graceful degradation):
   • Azure Search  → Fallback to keyword search
   • Email API     → Queue for retry
   • Cache         → Direct DB query (slower, NOT IMPLEMENTED)
```

---

## 7. Database Schema Overview

**Common fields on all entities (via base classes):**

```
┌──────────────────────────────────────────────────────────────────┐
│  Every Entity Has (via IAuditableEntity + ISoftDelete):          │
├──────────────────────────────────────────────────────────────────┤
│  Id                    Guid       PRIMARY KEY                     │
│  OrganizationId        Guid       ⚠️ CRITICAL: Multi-tenancy     │
│  CreatedBy             Guid?      Who created                     │
│  CreatedOn             DateTime   When created                    │
│  LastModifiedBy        Guid?      Who last modified               │
│  LastModifiedOn        DateTime?  When last modified              │
│  DeletedBy             Guid?      Who soft-deleted                │
│  DeletedOn             DateTime?  When soft-deleted (NULL = active)│
└──────────────────────────────────────────────────────────────────┘

Example: Patient Table
┌──────────────────────────────────────────────────────────────────┐
│ Patient                                                           │
├──────────────────────────────────────────────────────────────────┤
│ Id                     Guid       PRIMARY KEY                     │
│ OrganizationId         Guid       FOREIGN KEY → Organizations     │
│ FirstName              nvarchar   ⚠️ PHI                          │
│ LastName               nvarchar   ⚠️ PHI                          │
│ DateOfBirth            Date       ⚠️ PHI                          │
│ Gender                 nvarchar                                   │
│ Email                  nvarchar   ⚠️ PHI                          │
│ PhoneNumber            nvarchar   ⚠️ PHI                          │
│ MedicalRecordNumber    nvarchar   ⚠️ PHI (MRN)                   │
│ EmiratesIdNumber       nvarchar   ⚠️ PHI                          │
│ Status                 nvarchar   Active/Inactive                 │
│ CreatedBy              Guid?                                      │
│ CreatedOn              DateTime                                   │
│ LastModifiedBy         Guid?                                      │
│ LastModifiedOn         DateTime?                                  │
│ DeletedBy              Guid?                                      │
│ DeletedOn              DateTime?  (NULL = not deleted)            │
└──────────────────────────────────────────────────────────────────┘
         │
         │ Foreign Keys:
         ├─→ PatientGuardian    (1:many)
         ├─→ PatientInsurance   (1:many)
         ├─→ PatientAddress     (1:many)
         └─→ Appointments       (1:many)

Indexes:
  • UNIQUE (OrganizationId, MedicalRecordNumber, DeletedOn)
  • INDEX (OrganizationId, DeletedOn) ← For tenant queries
  • INDEX (Email, DeletedOn)          ← For search
```

**Global Query Filters (applied automatically):**
```sql
-- Multi-tenancy filter (Finbuckle)
WHERE OrganizationId = @CurrentTenantId

-- Soft delete filter (ISoftDelete)
AND DeletedOn IS NULL

-- Combined in every query:
SELECT * FROM Patients
WHERE Id = @patientId
  AND OrganizationId = @tenantId  ← Added automatically
  AND DeletedOn IS NULL           ← Added automatically
```

---

## 8. Request Pipeline (Middleware Order)

**Order matters! Each middleware can short-circuit the pipeline:**

```
┌──────────────────────────────────────────────────────────────────┐
│  HTTP Request →                                                   │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
         [1] Exception Handling Middleware
             • Catches all unhandled exceptions
             • Returns formatted error response
             • Logs error
             • ⚠️ Must NOT expose PHI in errors
                       ↓
         [2] CORS Middleware
             • Validates origin
             • Adds CORS headers
             • OPTIONS requests handled here
                       ↓
         [3] Authentication Middleware
             • Validates JWT token
             • Sets HttpContext.User
             • If invalid → 401 Unauthorized
                       ↓
         [4] Tenant Resolution Middleware
             • Extracts "tenant" claim from JWT
             • Stores in HttpContext
             • If missing → 401 Unauthorized
                       ↓
         [5] Authorization Middleware
             • Checks [Authorize] attributes
             • Verifies permissions
             • If denied → 403 Forbidden
                       ↓
         [6] Request Logging Middleware
             • Logs request details
             • ⚠️ Must NOT log PHI
             • Records timing
                       ↓
         [7] Routing Middleware
             • Matches URL to controller/action
             • If no match → 404 Not Found
                       ↓
         [8] Endpoint Middleware
             • Executes controller action
             • Runs through MediatR pipeline
             • Accesses DbContext (with filters)
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  ← HTTP Response                                                  │
└──────────────────────────────────────────────────────────────────┘
```

**Short-Circuit Examples:**

```
Scenario 1: Invalid JWT
[1] → [2] → [3] → ❌ 401 Unauthorized (stops here)

Scenario 2: Valid JWT, no permission
[1] → [2] → [3] → [4] → [5] → ❌ 403 Forbidden (stops here)

Scenario 3: Successful request
[1] → [2] → [3] → [4] → [5] → [6] → [7] → [8] → ✅ 200 OK
```

---

## 9. Event Processing (Dapr Pub/Sub)

**How entity changes become audit events:**

```
┌──────────────────────────────────────────────────────────────────┐
│  1. Application Code                                             │
│     var patient = new Patient { FirstName = "John", ... };       │
│     _context.Patients.Add(patient);                              │
│     await _context.SaveChangesAsync();  ← Triggers audit          │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  2. AuditableBaseDbContext.SaveChangesAsync()                    │
│     File: Scrips.BaseDbContext/AuditableBaseDbContext.cs:21      │
│                                                                  │
│     a. Detect changes:                                           │
│        var changes = AuditLoggingHelper.DetectChanges(           │
│            ChangeTracker, _httpContextAccessor);                 │
│                                                                  │
│     b. For each change, create LogAudit:                         │
│        {                                                         │
│          EntityType: "Patient",                                  │
│          EntityId: "123...",                                     │
│          Operation: "Create",                                    │
│          UserId: "789...",                                       │
│          TenantId: "456...",                                     │
│          OldValue: null,                                         │
│          NewValue: "John" → "*" (masked via [MaskValueAudit])   │
│          Timestamp: "2026-01-21T10:30:00Z"                       │
│        }                                                         │
│                                                                  │
│     c. Publish to Dapr:                                          │
│        await SaveAudit(changes);                                 │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  3. SaveAudit() Method (lines 57-63)                             │
│     if (await _daprClient.CheckHealthAsync())                    │
│         await _daprClient.PublishEventAsync(                     │
│             "pubsub",         ← Component name                   │
│             "SaveAudit",      ← Topic name (Topics.cs:11)        │
│             changes);         ← Payload                          │
│                                                                  │
│     ⚠️ CRITICAL ISSUE: Fire-and-forget!                          │
│        If Dapr fails, audit logs are LOST                        │
│        No retry, no dead letter queue                            │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  4. Dapr Sidecar                                                 │
│     • Receives event                                             │
│     • Routes to pub/sub component (Redis/RabbitMQ/Azure SvcBus) │
│     • Publishes to "SaveAudit" topic                             │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────────────────┐
│  5. Audit Consumer Service (separate microservice)              │
│     • Subscribes to "SaveAudit" topic                            │
│     • Receives audit events                                      │
│     • Stores in audit database (immutable)                       │
│     • Retention: 6-7 years (HIPAA requirement)                   │
└──────────────────────────────────────────────────────────────────┘

⚠️ Failure Scenarios:

Scenario A: Dapr unavailable
  SaveAudit() → CheckHealthAsync() → false
  → Audit log NOT published
  → SILENTLY LOST (no exception thrown)
  → HIPAA VIOLATION

Scenario B: Dapr available, but publish fails
  SaveAudit() → PublishEventAsync() → throws exception
  → Caught by try-catch (lines 31-34)
  → Logged but NOT rethrown
  → Database save succeeds, audit log LOST
  → HIPAA VIOLATION

Scenario C: Consumer service down
  → Event stuck in queue (OK, will be processed later)
  → Depends on pub/sub component's durability
```

**⚠️ KNOWN CRITICAL ISSUE:** See Technical Debt #2 - Audit log loss risk

---

## 10. Real-World Data Flow Example

**Complete scenario: "Book an appointment"**

```
┌──────────────────────────────────────────────────────────────────┐
│  User Action: Dr. Smith books appointment for John Doe          │
└──────────────────────┬───────────────────────────────────────────┘
                       ↓
[1] Frontend sends HTTP POST
    POST /api/Scheduling/Appointments
    Headers: Authorization: Bearer <JWT with tenant="clinic-A">
    Body: {
      "patientId": "patient-123",
      "providerId": "provider-789",
      "startTime": "2026-01-25T10:00:00Z",
      ...
    }
                       ↓
[2] API Gateway → Routes to Scheduling.API service
                       ↓
[3] Middleware Pipeline
    a. Auth → Validates JWT ✅
    b. Tenant → Extracts tenant="clinic-A" ✅
    c. Authz → Checks "Permissions.Appointments.Create" ✅
                       ↓
[4] Controller
    [HttpPost]
    public async Task<AppointmentResponse> Create(
        CreateAppointmentCommand command)
    {
        return await Mediator.Send(command);
    }
                       ↓
[5] MediatR → Routes to CreateAppointmentCommandHandler
                       ↓
[6] Handler Business Logic
    a. Validate patient exists (call IPatientApi)
       GET http://patient-api/api/Patient/patient-123
       ✅ Returns PatientResponse (Scrips.Core DTO)
       ⚠️ Filtered by tenant="clinic-A" automatically
       
    b. Validate provider exists (call IProviderApi)
       GET http://provider-api/api/Provider/provider-789
       ✅ Returns ProviderResponse (Scrips.Core DTO)
       
    c. Check slot availability (query local DB)
       SELECT * FROM Slots
       WHERE ProviderId = 'provider-789'
         AND StartTime = '2026-01-25T10:00:00'
         AND OrganizationId = 'clinic-A'  ← Tenant filter
         AND DeletedOn IS NULL            ← Soft delete filter
       ✅ Slot available
       
    d. Calculate price (call IBillingApi)
       POST http://billing-api/api/Billing/CalculatePrice
       Body: { serviceCode: "CONSULTATION", providerId: "..." }
       ✅ Returns FeeSummaryResponse (Scrips.Core DTO)
       
    e. Create appointment entity
       var appointment = new Appointment
       {
           Id = Guid.NewGuid(),
           OrganizationId = tenantId,      ← ⚠️ CRITICAL
           PatientId = command.PatientId,
           ProviderId = command.ProviderId,
           StartTime = command.StartTime,
           Status = AppointmentStatus.Scheduled,
           // Audit fields populated automatically
           CreatedBy = currentUserId,
           CreatedOn = DateTime.UtcNow
       };
       
    f. Save to database
       _context.Appointments.Add(appointment);
       await _context.SaveChangesAsync();  ← Triggers audit
                       ↓
[7] AuditableBaseDbContext.SaveChangesAsync()
    a. Detects change: INSERT Appointment
    b. Creates audit log:
       {
         EntityType: "Appointment",
         EntityId: "appt-456",
         Operation: "Create",
         UserId: "user-789",
         TenantId: "clinic-A",
         OldValue: null,
         NewValue: "{ PatientId: patient-123, ... }",
         Timestamp: "2026-01-21T10:30:00Z"
       }
    c. Publishes to Dapr "SaveAudit" topic
       ⚠️ Fire-and-forget (known issue)
    d. Saves to SQL Server:
       INSERT INTO Appointments VALUES (...)
                       ↓
[8] Post-Save Actions
    a. Send email notification (via IEmailSenderApi)
       POST http://email-api/api/Email/Send/appointment-confirmation
       Body: { 
         to: "john.doe@email.com",
         appointmentDate: "2026-01-25T10:00:00Z",
         providerName: "Dr. Smith"
       }
       ⚠️ Contains PHI in email
       
    b. Publish domain event (via Dapr)
       _daprClient.PublishEventAsync(
           "pubsub",
           Topics.AppointmentCreated,  ← If defined
           appointmentData);
                       ↓
[9] Return Response
    Returns: AppointmentResponse (Scrips.Core DTO)
    {
      "appointmentId": "appt-456",
      "patientId": "patient-123",
      "providerId": "provider-789",
      "startTime": "2026-01-25T10:00:00Z",
      "status": "Scheduled",
      ...
    }
                       ↓
[10] Frontend displays confirmation
     "Appointment booked successfully!"

✅ Success Path Complete

Data Touched:
• Scheduling DB: 1 INSERT (Appointment)
• Patient API: 1 GET (verify patient)
• Provider API: 1 GET (verify provider)
• Billing API: 1 POST (calculate price)
• Email API: 1 POST (send confirmation)
• Dapr: 1 publish (audit log)

PHI Accessed:
• Patient: FirstName, LastName, DOB, Email
• Appointment: PatientId, StartTime, Provider

Audit Trail:
✅ Appointment creation logged
✅ PHI access logged (via API calls)
⚠️ If Dapr fails, audit log lost (KNOWN ISSUE)
```

---

## 11. Quick Navigation Table

| Need to understand... | See section... | Key files |
|-----------------------|----------------|-----------|
| How services connect | [#1 System Context](#1-system-context) | - |
| Layer dependencies | [#2 Clean Architecture](#2-clean-architecture-layers) | All .csproj files |
| Command/Query pattern | [#3 CQRS Flow](#3-cqrs-flow-commandquery-pattern) | BaseApiController.cs |
| HTTP request lifecycle | [#4 Query Flow](#4-query-flow-rest-api-request--response) | Middleware/ |
| Tenant isolation | [#5 Multi-Tenancy](#5-multi-tenancy-flow-jwt-validation) | AuditableMultiTenantBaseDbContext.cs |
| External dependencies | [#6 Dependency Graph](#6-dependency-graph-external-services) | HttpApiClients/, Protos/ |
| Database structure | [#7 Database Schema](#7-database-schema-overview) | Domain/Contracts/ |
| Request pipeline | [#8 Request Pipeline](#8-request-pipeline-middleware-order) | Infrastructure/Middleware/ |
| Audit logging | [#9 Event Processing](#9-event-processing-dapr-pubsub) | AuditableBaseDbContext.cs:21-63 |
| End-to-end scenario | [#10 Real-World Example](#10-real-world-data-flow-example) | - |

**Next Steps:**
- Read [ONBOARDING.md](../ONBOARDING.md) for hands-on tutorial
- Review [05-architecture.md](05-architecture.md) for deep dive
- Check [09-technical-debt-inventory.md](09-technical-debt-inventory.md) for known issues

---

*Last updated: January 2026*  
*Questions? Create an issue or ask in #engineering*
