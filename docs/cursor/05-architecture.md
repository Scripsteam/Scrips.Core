# Architecture Documentation

## 1. SYSTEM OVERVIEW

Scrips.Core is a shared library for a healthcare practice management microservices platform. It provides DTOs, HTTP/gRPC API clients, authorization models, and database context abstractions used across multiple healthcare domain services (Patient, Billing, Scheduling, Provider, Practice, Organization, Identity). The library enables communication between microservices via Refit-based REST APIs and gRPC, implements multi-tenant data isolation with Finbuckle, publishes audit logs via Dapr pub/sub, and enforces RBAC with JWT claims. It does not contain domain entities or MediatR handlers - those reside in consuming services. This is a **library codebase**, not a standalone application.

## 2. TECH STACK DISCOVERY

**Runtime & Framework:**
- Framework: .NET 8.0 (LTS)
  - File: Scrips.Core.csproj:4
  - File: Scrips.Core.Application.csproj:4
  - File: Scrips.Infrastructure.csproj:4
- Language: C# (197 .cs files in Scrips.Core/)

**Data Storage:**
- Database: SQL Server (default)
  - File: DatabaseSettings.cs:5-6 (DBProvider, ConnectionString)
  - File: Startup.cs:23 (`const string dbProvider = "mssql"`)
  - File: Startup.cs:47-59 (commented multi-provider switch)
  - Note: Multi-database EF Core support configured but commented out
- Cache: **NOT IMPLEMENTED**
  - File: ICacheService.cs:1-16 (interface defined)
  - Implementation: Missing - abstraction only

**Communication:**
- HTTP Library: Refit v8.0.0
  - File: Scrips.Core.csproj:18
  - Usage: 11 HTTP API clients in HttpApiClients/ folder
- gRPC: Protocol Buffers (9 .proto definitions)
  - File: Scrips.Core.csproj:18-26
  - Services: Appointment, Billing, Identity, Patient, Provider, Practice, Organization, Person, Master
- Message Queue: Dapr Client v1.14.0
  - File: Scrips.BaseDbContext.csproj:15
  - File: AuditableBaseDbContext.cs:61 (PublishEventAsync)
  - File: Topics.cs:1-21 (10 event topics defined)

**Key Dependencies:**
- MediatR v12.0.1 (CQRS) - Scrips.Core.Application.csproj:17
- Entity Framework Core v8.0.11 - Scrips.BaseDbContext.csproj:19
- FluentValidation v11.10.0 - Scrips.Core.Application.csproj:14
- Mapster v7.4.0 (mapping) - Scrips.Core.Application.csproj:16
- Finbuckle.MultiTenant.EntityFrameworkCore v6.10.0 - Scrips.BaseDbContext.csproj:28
- Azure.Search.Documents v11.6.0 - Scrips.Core.csproj:9
- Serilog v4.0.2 (logging) - Scrips.BaseDbContext.csproj:27

## 3. MAJOR COMPONENTS

### Component: Scrips.Core (Models & API Clients)
**Location:** Scrips.Core/

**Responsibility:** DTOs, enums, HTTP/gRPC API client interfaces, event topics

**Key Files:**
- HttpApiClients/IBillingApi.cs - Refit interface for Billing service
- HttpApiClients/IPatientApi.cs - Refit interface for Patient service
- HttpApiClients/ISchedulingApi.cs - Refit interface for Scheduling service
- Models/Patient/EditPatientResponse.cs - Patient data transfer object
- Models/Scheduling/AppointmentDto.cs - Appointment data structure
- Topics.cs:1-21 - Dapr pub/sub event topic constants
- Protos/ - 10 protobuf definitions for gRPC services

### Component: Scrips.Core.Application (Application Layer)
**Location:** Scrips.Core.Application/

**Responsibility:** Interfaces, caching abstraction, file storage, event handlers, common utilities

**Key Files:**
- Common/Interfaces/ICurrentUser.cs:1-21 - User context interface
- Common/Caching/ICacheService.cs:1-16 - Cache abstraction (not implemented)
- Common/FileStorage/IFileStorageService.cs - File upload abstraction
- Common/Events/IEventPublisher.cs - Domain event publisher
- Auditing/IAuditService.cs - Audit log retrieval

### Component: Scrips.BaseDbContext (Audit Infrastructure)
**Location:** Scrips.BaseDbContext/

**Responsibility:** Auditable database context with automatic change tracking, PHI masking, Dapr publishing

**Key Files:**
- AuditableBaseDbContext.cs:21-63 - Intercepts SaveChanges, publishes audit logs
- AuditLoggingHelper.cs:13-87 - Detects entity changes, applies MaskValueAudit
- MaskValueAuditAttribute.cs:13 - Attribute to mark PHI fields for masking
- AuditableMultiTenantBaseDbContext.cs - Multi-tenant aware audit context

### Component: Scrips.Infrastructure (Infrastructure Services)
**Location:** Scrips.Infrastructure/

**Responsibility:** Auth, middleware, persistence configuration, error handling

**Key Files:**
- Auth/CurrentUser.cs:7-61 - ICurrentUser implementation from ClaimsPrincipal
- Middleware/ExceptionMiddleware.cs:11-86 - Centralized exception handling
- Middleware/CurrentUserMiddleware.cs - Sets user context per request
- Persistence/DatabaseSettings.cs:3-7 - DB provider and connection string config
- Persistence/Startup.cs:9-95 - Multi-database EF Core setup (commented out)

### Component: Scrips.WebApi (API Controllers)
**Location:** Scrips.WebApi/

**Responsibility:** Base controllers with MediatR integration, API versioning

**Key Files:**
- BaseApiController.cs:8-13 - Base controller with ISender (MediatR)
- VersionedApiController.cs:6 - Versioned API support
- ScripsApiConventions.cs - API conventions

### Component: Scrips.Core.Domain (Domain Abstractions)
**Location:** Scrips.Core.Domain/

**Responsibility:** Domain contracts, base entities, domain events, soft delete

**Key Files:**
- Common/Contracts/BaseEntity.cs - Entity base class
- Common/Contracts/IAuditableEntity.cs - Auditable entity interface
- Common/Contracts/ISoftDelete.cs:3-6 - Soft delete pattern (IsArchived)
- Common/Contracts/DomainEvent.cs - Domain event base
- Common/Events/EntityCreatedEvent.cs - Entity lifecycle events

### Component: Shared Authorization
**Location:** Scrips.Core/Shared/Authorization/

**Responsibility:** Permission model, roles, JWT claims extensions

**Key Files:**
- ScripsPermissions.cs:19-34 - Permission definitions (Action × Resource)
- ClaimsPrincipalExtensions.cs:6-20 - Extract email, tenant, name from JWT
- ScripsAction.cs - Action constants (View, Create, Update, Delete)
- ScripsResource.cs - Resource constants (Tenants, Patients, etc.)

### Component: AI Chief Complaint
**Location:** Scrips.Core/Models/AIChiefComplaint/

**Responsibility:** Azure Cognitive Search document models for clinical NLP

**Key Files:**
- ChiefComplaintDocument.cs:1-38 - Vector search document with 1536-dimension embeddings
- Related: Azure.Search.Documents v11.6.0 package

## 4. CRITICAL DATA FLOW

**Audit Log Processing (Most Critical for Compliance):**

1. **Entity Change Detected**
   - File: AuditableBaseDbContext.cs:21-29
   - Trigger: SaveChangesAsync() called
   - Action: Calls AuditLoggingHelper.DetectChanges()

2. **Change Tracking**
   - File: AuditLoggingHelper.cs:13-17
   - Action: Gets all Added/Modified/Deleted entities from EF ChangeTracker
   - Excludes: LogAudit entities (prevent recursive audit)

3. **User Context Extraction**
   - File: AuditLoggingHelper.cs:20-23
   - Action: Extracts user ID (JWT "sub" claim), tenant ID ("tenant" claim), IP address from HttpContext

4. **PHI Masking**
   - File: AuditLoggingHelper.cs:43-44
   - Action: Checks each property for [MaskValueAudit] attribute via reflection
   - File: AuditLoggingHelper.cs:56,60,68
   - Action: If masked, replaces old/new values with "*" in audit log

5. **Audit Log Creation**
   - File: AuditLoggingHelper.cs:83
   - Action: Converts AuditEntityEntry to LogAudit
   - Returns: List<LogAudit> with user, tenant, IP, entity, old values, new values

6. **Dapr Publishing**
   - File: AuditableBaseDbContext.cs:57-62
   - Action: Publishes audit logs to Dapr pub/sub "SaveAudit" topic
   - Note: Fire-and-forget (no retry on failure - see Section 8.2)

7. **Database Save**
   - File: AuditableBaseDbContext.cs:36
   - Action: Calls base.SaveChangesAsync() to persist entity changes
   - Note: Audit failure does NOT block database save (try-catch wrapper)

## 5. HEALTHCARE COMPLIANCE

**PHI Storage:**
- Tables with PHI: Patient (all demographics), Encounter (clinical notes), Invoice (financial PHI), Appointment (with patient link)
- Encryption: **NOT CONFIGURED IN CODE**
  - Database-level TDE recommended but not enforced by code
  - Connection string in plain text: DatabaseSettings.cs:6
  - Field-level encryption: Not implemented

**Audit Trail:**
- Implementation: AuditableBaseDbContext.cs:21-63, AuditLoggingHelper.cs:13-87
- Events captured:
  - Entity Created (Added state)
  - Entity Updated (Modified state with property-level changes)
  - Entity Deleted (Deleted state)
- Logged data:
  - User ID: AuditLoggingHelper.cs:20 (JWT "sub" claim)
  - Tenant ID: AuditLoggingHelper.cs:21 (JWT "tenant" claim)
  - IP Address: AuditLoggingHelper.cs:23 (from HTTP headers)
  - Timestamp: Implicit (LogAudit entity)
  - Entity type and ID: AuditLoggingHelper.cs:36-48
  - Old/new values: AuditLoggingHelper.cs:56-68 (masked if [MaskValueAudit])
- PHI Masking: MaskValueAuditAttribute.cs:13 marks sensitive properties
  - Masked value: "*" (AuditLoggingHelper.cs:27)
  - Masking applied to: EmiratesId, Email, MobileNumber, Insurance Member ID (where attribute applied)

**Access Control:**
- Authentication: JWT Bearer tokens
  - File: CurrentUser.cs:9 (ClaimsPrincipal)
  - Claims: "sub" (user ID), "tenant" (tenant ID), "sa" (super admin flag)
  - Extraction: ClaimsPrincipalExtensions.cs:8-20
- Authorization: RBAC via ScripsPermissions
  - File: ScripsPermissions.cs:21-31 (All, Root, Admin, Basic permission sets)
  - Pattern: Action (View/Create/Update/Delete) × Resource (Patients/Appointments/etc.)
  - Permission record: ScripsPermissions.cs:34 (Description, Action, Resource, IsBasic, IsRoot flags)
  - Enforcement: Not visible in core library (implemented in consuming services)

**Multi-Tenancy:**
- Implementation: Finbuckle.MultiTenant.EntityFrameworkCore v6.10.0
  - File: Scrips.BaseDbContext.csproj:17
  - File: AuditableMultiTenantBaseDbContext.cs
- Tenant Resolution: JWT "tenant" claim
  - File: AuditLoggingHelper.cs:21
  - File: CurrentUser.cs:36-37
- Data Isolation: Global query filters (configured in consuming services)

**Soft Delete:**
- Implementation: ISoftDelete.cs:3-6
- Pattern: IsArchived flag instead of hard delete
- Purpose: Data retention, historical reporting, compliance

## 6. MICROSERVICES CONTEXT

**This library supports microservices:**

**Service Communication - HTTP APIs (Refit):**
| Target Service | Interface | File |
|----------------|-----------|------|
| Billing | IBillingApi | HttpApiClients/IBillingApi.cs |
| Email Sender | IEmailSenderApi | HttpApiClients/IEmailSenderApi.cs |
| Identity | IIdentityApi | HttpApiClients/IIdentityApi.cs |
| Master Data | IMasterApi | HttpApiClients/IMasterApi.cs |
| Notifications | INotificationsApi | HttpApiClients/INotificationsApi.cs |
| Organization | IOrganizationApi | HttpApiClients/IOrganizationApi.cs |
| Patient | IPatientApi | HttpApiClients/IPatientApi.cs |
| Person | IPersonApi | HttpApiClients/IPersonApi.cs |
| Practice | IPracticeApi | HttpApiClients/IPracticeApi.cs |
| Provider | IProviderApi | HttpApiClients/IProviderApi.cs |
| Scheduling | ISchedulingApi | HttpApiClients/ISchedulingApi.cs |

**Service Communication - gRPC:**
| Service | Proto Definition | File |
|---------|------------------|------|
| Appointment | AppointmentManagement | Protos/AppointmentManagement.proto |
| Billing | BillingManagement | Protos/BillingManagement.proto |
| Identity | IdentityManagement | Protos/IdentityManagement.proto |
| Master Data | MasterManagement | Protos/MasterManagement.proto |
| Organization | OrganizationManagement | Protos/OrganizationManagement.proto |
| Patient | PatientManagement | Protos/PatientManagement.proto |
| Person | PersonManagement | Protos/PersonManagement.proto |
| Practice | PracticeManagement | Protos/PracticeManagement.proto |
| Provider | ProviderManagement | Protos/ProviderManagement.proto |

**Events Published (via Dapr):**
| Topic | Trigger | File:Line |
|-------|---------|-----------|
| SaveAudit | Every entity change | AuditableBaseDbContext.cs:61 |
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

**Events Consumed:**
Not defined in this library - consumers implemented in separate microservices.

**Deployment Pattern:**
This is a **shared library** (project reference), not a deployable service. Consuming microservices include this library and implement:
- Domain entities (not in this library)
- MediatR handlers (not in this library)
- API controllers inheriting from BaseApiController
- Database contexts inheriting from AuditableBaseDbContext or AuditableMultiTenantBaseDbContext

## 7. RISKS & TECHNICAL DEBT

### 1. Audit Log Loss on Dapr Failure
- Evidence: AuditableBaseDbContext.cs:60-61 (fire-and-forget publish, no retry)
- Impact: Compliance audit trail gaps if Dapr unavailable. Try-catch wrapper at line 31-34 logs error but continues. Audit logs silently lost.
- Severity: **CRITICAL** for healthcare compliance (HIPAA, UAE regulations)

### 2. Cache Not Implemented
- Evidence: ICacheService.cs:1-16 (interface only, no implementation)
- Evidence: Startup.cs:102 comment "Cache implementation abstraction defined but not implemented"
- Impact: No distributed caching for multi-instance deployments. In-memory cache per instance causes inconsistencies. Performance degradation for repeated queries.
- Severity: **HIGH** for scalability

### 3. Connection String Security
- Evidence: DatabaseSettings.cs:6 (plain text ConnectionString property)
- Evidence: Startup.cs:17-20 (reads from config without encryption)
- Impact: Credentials potentially stored in appsettings.json. Risk of exposure in source control, logs, config dumps.
- Severity: **CRITICAL** for security (database credentials)

### 4. Multi-Database Support Commented Out
- Evidence: Startup.cs:42-94 (entire UseDatabase method commented)
- Evidence: Startup.cs:23 (hardcoded "mssql")
- Impact: Claims to support PostgreSQL and MySQL but implementation disabled. Migration assemblies referenced but not functional. Misleading documentation.
- Severity: **MEDIUM** for flexibility/vendor lock-in

### 5. No Retry Policies on External Service Calls
- Evidence: HttpApiClients/*.cs (11 Refit interfaces with no Polly policies visible)
- Evidence: AuditableBaseDbContext.cs:61 (no retry on Dapr publish)
- Impact: Transient failures cause immediate errors. Network blips break workflows. No resilience for microservice communication.
- Severity: **HIGH** for reliability in distributed system

### 6. MaskValueAudit Attribute Not Used in Domain
- Evidence: Grep search found ZERO actual usages on domain entities (only example in comment)
- Evidence: No domain entities in Scrips.Core.Domain (only contracts)
- Impact: If consuming services don't apply attribute, PHI logged in plain text. Documentation shows example but no enforcement.
- Severity: **CRITICAL** for PHI protection

### 7. .NET 7.0 End-of-Support
- Evidence: Scrips.Core.csproj:4, All .csproj files target net7.0
- Impact: .NET 7.0 support ended May 2024. No security patches. Must migrate to .NET 8 (LTS).
- Severity: **HIGH** for security and support

### 8. Dual Proto File Locations
- Evidence: Protos/ folder in root AND Scrips.Core/Protos/
- Evidence: Scrips.Core.csproj:18-26 references Scrips.Core/Protos/
- Evidence: Root Protos/ contains Appointment.proto, Common.proto, Identity.proto, Organization.proto
- Impact: Confusion on canonical location. Risk of outdated duplicates. Build inconsistencies.
- Severity: **LOW** for maintainability

### 9. ISoftDelete Without Enforcement
- Evidence: ISoftDelete.cs:3-6 (interface with IsArchived property)
- Evidence: No global query filter in this library (must be configured by consuming services)
- Impact: Soft-deleted records may appear in queries if service forgets to filter. Data leakage risk.
- Severity: **MEDIUM** for data integrity

### 10. No Domain Entities in Core Library
- Evidence: Scrips.Core.Domain/Common/ contains only contracts/interfaces
- Evidence: Models in Scrips.Core/Models/ are DTOs, not domain entities
- Impact: This is intentional (Clean Architecture), but means:
  - No validation at domain level
  - Domain logic scattered across services
  - Inconsistent business rule enforcement
  - Each service must implement own entities
- Severity: **MEDIUM** for consistency (architectural trade-off)
