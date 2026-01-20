# Workspace Inventory for Scrips.Core

## 1. PROJECT SUMMARY

- Repository name: Scrips.Core
- Total projects found: 6 (.csproj files)
- Total files: ~274 C# files (not including infrastructure and test files)
- Primary technology: .NET 7.0
- Main language: C#

### Project Structure:
1. **Scrips.Core** - Core DTOs, Models, Enums, HTTP API Clients, and gRPC Protos
2. **Scrips.Core.Domain** - Domain entities and contracts
3. **Scrips.Core.Application** - Application layer with CQR services, interfaces, and common utilities
4. **Scrips.BaseDbContext** - Auditable database context implementations
5. **Scrips.Infrastructure** - Infrastructure layer (auth, middleware, persistence)
6. **Scrips.WebApi** - Web API base controllers and conventions

## 2. ENTRY POINTS

### Controllers (if web API)
| File Path | Controller Name | Purpose |
|-----------|----------------|---------|
| Scrips.WebApi\BaseApiController.cs:8 | BaseApiController | Base controller with MediatR integration for CQRS pattern |
| Scrips.WebApi\VersionedApiController.cs:6 | VersionedApiController | API controller with versioning support |
| Scrips.WebApi\VersionNeutralApiController.cs:7 | VersionNeutralApiController | API controller without version constraints |

### Handlers/Services
| File Path | Handler Name | Purpose |
|-----------|-------------|---------|
| Scrips.Core.Application\Common\Events\IEventNotificationHandler.cs:1 | IEventNotificationHandler | Handles domain event notifications |
| Scrips.Core.Application\Common\Events\IEventPublisher.cs:5 | IEventPublisher | Publishes domain events across system |
| Scrips.Core.Application\Auditing\IAuditService.cs | IAuditService | Manages audit log retrieval |
| Scrips.Core.Application\Common\Caching\ICacheService.cs | ICacheService | Provides caching abstraction |
| Scrips.Core.Application\Common\FileStorage\IFileStorageService.cs | IFileStorageService | Handles file upload/storage operations |

### Jobs/Workers (if background processing)
| File Path | Job Name | Trigger |
|-----------|----------|---------|
| Scrips.Core.Application\Common\Interfaces\IJobService.cs:5 | IJobService | Hangfire-style job scheduling (enqueue, schedule, delete, requeue) |
| Scrips.Core\Shared\Notifications\JobNotification.cs | JobNotification | Job notification message model |

### Event Consumers (if messaging system)
| File Path | Consumer Name | Listens To |
|-----------|--------------|------------|
| Scrips.BaseDbContext\AuditableBaseDbContext.cs:57 | SaveAudit via Dapr | "SaveAudit" topic on "pubsub" pubsub |
| Scrips.Core\Topics.cs | Topics (constants) | TenantCreated, DoctorCreated, OrganizationCreated, OrganizationV1Created, OrganizationV1Updated, PracticeCreated, PracticeUpdated, PracticeActiveArchive, OrganizationSettingsCreated, OrganizationSettingsUpdated |

## 3. DOMAIN AREAS IDENTIFIED

- **Scheduling**: Appointment management, slots, calendars, participants, reminders
  - Related: `Scrips.Core\Models\Scheduling\`, `Scrips.Core\Enums\Scheduling\`, `Scrips.Core\HttpApiClients\ISchedulingApi.cs`
  
- **Billing**: Invoices, payments, billing groups, procedure codes, claims, remittance advice
  - Related: `Scrips.Core\Models\Billing\`, `Scrips.Core\Enums\Billing\`, `Scrips.Core\HttpApiClients\IBillingApi.cs`
  
- **Patient Management**: Patient records, health insurance, corporate sponsorship, guardians, addresses
  - Related: `Scrips.Core\Models\Patient\`, `Scrips.Core\Enums\Patient\`, `Scrips.Core\HttpApiClients\IPatientApi.cs`
  
- **Practice Management**: Practice setup, doctors, staff, appointment profiles, provider licenses
  - Related: `Scrips.Core\Models\Practice\`, `Scrips.Core\HttpApiClients\IPracticeApi.cs`
  
- **Provider Management**: Practitioner details, qualifications, specialties, addresses
  - Related: `Scrips.Core\Models\Provider\`, `Scrips.Core\HttpApiClients\IProviderApi.cs`
  
- **Organization Management**: Organization structure, settings, multi-tenancy
  - Related: `Scrips.Core\Models\Organization\`, `Scrips.Core\HttpApiClients\IOrganizationApi.cs`
  
- **Identity & Authorization**: User authentication, roles, permissions, claims, Emirates ID
  - Related: `Scrips.Core\Models\Identity\`, `Scrips.Core\Shared\Authorization\`, `Scrips.Core\HttpApiClients\IIdentityApi.cs`, `Scrips.Infrastructure\Auth\`
  
- **AI Chief Complaint Analysis**: Healthcare NLP, entity extraction, clinical documentation, RAG search
  - Related: `Scrips.Core\Models\AIChiefComplaint\`
  
- **Auditing**: Entity change tracking, audit logging, audit retrieval
  - Related: `Scrips.BaseDbContext\`, `Scrips.Core.Application\Auditing\`, `Scrips.Core\Models\Audit\`
  
- **Person Management**: Person info across system
  - Related: `Scrips.Core\Models\Person\`, `Scrips.Core\HttpApiClients\IPersonApi.cs`

## 4. EXTERNAL DEPENDENCIES

### Healthcare Integrations:
- **NABIDH**: Scrips.Core\Models\Practice\PracticeSetupDetails.cs:18, Scrips.Core\Models\Scheduling\PracticeResponse.cs:60
  - `NabidhAssigningAuthority` property indicates integration with UAE's National Backbone for Integrated Dubai Health
- **Malaffi**: NOT FOUND

### Databases:
- **Type**: SQL Server, PostgreSQL (Npgsql), or MySQL
- **Location**: Scrips.Infrastructure\Persistence\DatabaseSettings.cs:6
- **Provider Detection**: Scrips.Infrastructure\Persistence\Startup.cs:47-59
- **Connection**: Configuration-based via `DatabaseSettings.ConnectionString`
- **Multi-tenancy**: Supported via Finbuckle.MultiTenant.EntityFrameworkCore (Scrips.BaseDbContext.csproj:17)

### Message Queues:
- **Type**: Dapr Pub/Sub
- **Location**: Scrips.BaseDbContext\AuditableBaseDbContext.cs:1
- **Usage**: Publishing audit events to "pubsub" component via `_daprClient.PublishEventAsync("pubsub", "SaveAudit", changes)`
- **Topics**: Defined in Scrips.Core\Topics.cs

### Cache Systems:
- **Type**: NOT FOUND (Interface defined but implementation not in this codebase)
- **Note**: `ICacheService` interface exists in Scrips.Core.Application\Common\Caching\ICacheService.cs

### Search & AI Services:
- **Type**: Azure Cognitive Search
- **Location**: Scrips.Core\Models\AIChiefComplaint\ChiefComplaintDocument.cs:1
- **Usage**: Vector search for chief complaint analysis with 1536-dimension embeddings
- **Package**: Azure.Search.Documents v11.6.0 (Scrips.Core\Scrips.Core.csproj:8)

### gRPC Services:
9 proto definitions for inter-service communication:
- AppointmentManagement, BillingManagement, IdentityManagement, MasterManagement
- PatientManagement, PersonManagement, PracticeManagement, ProviderManagement, OrganizationManagement
- Location: Scrips.Core\Protos\

### HTTP API Clients:
11 Refit-based HTTP clients for microservice communication:
- IBillingApi, IEmailSenderApi, IIdentityApi, IMasterApi, INotificationsApi
- IOrganizationApi, IPatientApi, IPersonApi, IPracticeApi, IProviderApi, ISchedulingApi
- Location: Scrips.Core\HttpApiClients\

### Key Technologies:
- **CQRS/Mediator**: MediatR v12.0.1
- **ORM**: Entity Framework Core v7.0.4
- **Validation**: FluentValidation v11.5.1
- **Mapping**: Mapster v7.3.0
- **Specification Pattern**: Ardalis.Specification v6.1.0
- **Distributed Tracing/State**: Dapr.Client v1.10.0
- **ID Generation**: NewId v4.0.1, MassTransit (for entity IDs)
- **Logging**: Serilog v2.12.0
- **API Client**: Refit v6.3.2
