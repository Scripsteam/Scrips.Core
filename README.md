# Scrips.Core 🏥

> Shared library for the Scrips healthcare platform - DTOs, models, API clients, and base infrastructure for microservices.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-Proprietary-red)](LICENSE)
[![HIPAA](https://img.shields.io/badge/HIPAA-Compliant-green)]()
[![Docs](https://img.shields.io/badge/docs-28k%20lines-blue)](docs/cursor/README.md)

⚠️ **Healthcare Context:** This codebase handles Protected Health Information (PHI). Security and compliance requirements are mandatory.

---

## What is Scrips.Core?

Scrips.Core is the **foundational shared library** for the Scrips healthcare platform. It provides:

- **✅ DTOs & Models** - Data contracts for 9 healthcare domains (Scheduling, Patients, Billing, etc.)
- **✅ API Clients** - Refit-based HTTP clients for 11 microservices
- **✅ Base Classes** - Database contexts with automatic audit logging and multi-tenancy
- **✅ gRPC Definitions** - Protocol buffers for 9 inter-service communications
- **✅ Shared Logic** - Authorization, permissions, audit attributes, event topics

**Think of it as:** The NuGet package that all Scrips microservices import for consistent data structures and shared functionality.

---

## Key Features

- **🏥 Healthcare Compliant** - HIPAA-ready with PHI protection and audit logging
- **🏢 Multi-Tenant** - Built-in tenant isolation via Finbuckle.MultiTenant
- **📊 Automatic Auditing** - All entity changes logged via Dapr pub/sub
- **🔒 Secure by Default** - JWT authentication, role-based permissions
- **🎨 Clean Architecture** - Domain-driven design with clear layer separation
- **⚡ CQRS Ready** - MediatR integration for command/query separation
- **📚 Extensively Documented** - 28,000+ lines of technical documentation

---

## Quick Start

### Prerequisites

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio 2022** or **VS Code** with C# extension
- **SQL Server** (for consuming microservices)

### Installation

```bash
# Clone the repository
git clone https://github.com/YourOrg/Scrips.Core.git
cd Scrips.Core

# Restore dependencies
dotnet restore

# Build the solution
dotnet build
```

### Verify It Works

```bash
# Build should succeed with zero errors
dotnet build

# Expected output:
# Build succeeded.
#     0 Warning(s)
#     0 Error(s)
```

### 📖 **For complete onboarding:** Read [ONBOARDING.md](ONBOARDING.md) (~2 hours, hands-on guide)

---

## Architecture

### System Context

```
┌──────────────────────────────────────────────────────────────┐
│                     Scrips Platform                          │
│                                                              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │  Patient    │  │  Scheduling │  │   Billing   │        │
│  │    API      │  │     API     │  │     API     │  ...   │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘        │
│         │                │                │                 │
│         └────────────────┴────────────────┘                 │
│                         │                                    │
│                         ↓                                    │
│                 ┌───────────────┐                           │
│                 │ Scrips.Core   │ ← You are here            │
│                 │ (This Library)│                           │
│                 └───────┬───────┘                           │
│                         │                                    │
│         ┌───────────────┼───────────────┐                  │
│         ↓               ↓               ↓                   │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐          │
│  │  SQL Server│  │    Dapr    │  │   Azure    │          │
│  │  (Multi-   │  │  (Pub/Sub) │  │   Search   │          │
│  │  Tenant)   │  │            │  │  (AI/RAG)  │          │
│  └────────────┘  └────────────┘  └────────────┘          │
└──────────────────────────────────────────────────────────────┘
```

### Architecture Pattern

**Clean Architecture** with clear layer separation:

- **Domain Layer** - Business rules and entities (no dependencies)
- **Application Layer** - Use cases and interfaces
- **Infrastructure Layer** - Database, auth, external services
- **Presentation Layer** - API controllers (in consuming microservices)

**Communication:**
- **HTTP/REST** - Refit-based typed clients (11 APIs)
- **gRPC** - High-performance inter-service (9 services)
- **Event-Driven** - Dapr pub/sub for domain events

**Data Access:**
- **Entity Framework Core 8.0** - ORM with automatic audit logging
- **Finbuckle.MultiTenant** - Tenant isolation via global query filters
- **Specification Pattern** - Ardalis.Specification for reusable queries

---

## Project Structure

```
Scrips.Core/
│
├── Scrips.Core/                      # ⭐ Main library (DTOs, Models, API clients)
│   ├── Models/                       # DTOs organized by domain
│   │   ├── Patient/                  # Patient-related DTOs
│   │   ├── Scheduling/               # Appointment DTOs
│   │   ├── Billing/                  # Invoice, payment DTOs
│   │   ├── Practice/                 # Provider, practice DTOs
│   │   ├── AIChiefComplaint/         # Clinical AI DTOs
│   │   └── ... (9 domains total)
│   ├── Enums/                        # Enumerations by domain
│   ├── HttpApiClients/               # 11 Refit API interfaces
│   ├── Protos/                       # 9 gRPC proto definitions
│   ├── Shared/                       # Cross-cutting concerns
│   │   ├── Authorization/            # Claims, permissions, roles
│   │   └── Notifications/            # Event models
│   └── Topics.cs                     # Dapr pub/sub topic names
│
├── Scrips.Core.Domain/               # Domain entities & contracts
│   ├── Contracts/                    # Interfaces (ISoftDelete, etc.)
│   └── Common/                       # Base entity classes
│
├── Scrips.Core.Application/          # Application services & interfaces
│   ├── Common/
│   │   ├── Caching/                  # ICacheService (interface only)
│   │   ├── Events/                   # IEventPublisher
│   │   ├── FileStorage/              # IFileStorageService
│   │   └── Interfaces/               # IJobService (Hangfire)
│   └── Auditing/                     # IAuditService
│
├── Scrips.BaseDbContext/             # 🔒 Database base classes (CRITICAL)
│   ├── AuditableBaseDbContext.cs     # Auto audit logging
│   ├── AuditableMultiTenantBaseDbContext.cs  # Multi-tenant + audit
│   └── MaskValueAuditAttribute.cs    # PHI masking attribute
│
├── Scrips.Infrastructure/            # Infrastructure implementations
│   ├── Auth/                         # JWT configuration
│   ├── Middleware/                   # Tenant resolution, exceptions
│   └── Persistence/                  # Database settings
│
├── Scrips.WebApi/                    # Base API controllers
│   ├── BaseApiController.cs          # MediatR integration
│   └── VersionedApiController.cs     # API versioning
│
└── docs/                             # 📚 Comprehensive documentation
    └── cursor/                       # 28,000+ lines of docs
        ├── 00-workspace-inventory.md
        ├── 01-business-logic-outline.md
        ├── 02-section-X-*.md (9 files, all audited)
        ├── 05-architecture.md
        ├── 06-integration-details.md
        ├── 07-framework-audit.md
        ├── 08-microservices-topology.md
        ├── 09-technical-debt-inventory.md
        └── README.md (documentation index)
```

---

## Key Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 (LTS) | Runtime framework |
| C# | 12.0 | Primary language |
| Entity Framework Core | 8.0.11 | ORM |
| MediatR | 12.0.1 | CQRS/mediator pattern |
| Refit | 8.0.0 | HTTP API clients |
| Dapr | 1.14.0 | Distributed pub/sub |
| Finbuckle.MultiTenant | 6.10.0 | Multi-tenancy |
| Azure.Search.Documents | 11.6.0 | AI-powered search |
| FluentValidation | 11.10.0 | Request validation |
| Mapster | 7.4.0 | Object mapping |
| Ardalis.Specification | 8.0.0 | Repository pattern |
| Serilog | 4.0.2 | Structured logging |
| Newtonsoft.Json | 13.0.4 | JSON serialization |
| Google.Protobuf | 3.33.5 | gRPC protocol buffers |
| Asp.Versioning.Mvc | 8.1.0 | API versioning |

---

## Common Tasks

### Query an API (using Refit client)

```csharp
// Inject the API client
public class MyService
{
    private readonly IPatientApi _patientApi;
    
    public MyService(IPatientApi patientApi)
    {
        _patientApi = patientApi;
    }
    
    // Call the API
    public async Task<PatientResponse> GetPatientAsync(Guid patientId)
    {
        return await _patientApi.GetPatientAsync(patientId);
    }
}
```

### Publish a Domain Event (via Dapr)

```csharp
// In your DbContext (inherits from AuditableBaseDbContext)
// Events are automatically published when SaveChangesAsync() is called

// Dapr topics defined in Topics.cs:
// - TenantCreated
// - DoctorCreated
// - OrganizationV1Created
// - PracticeCreated
// - SaveAudit (automatic for all entity changes)
```

### Add a New DTO

```csharp
// 1. Create the DTO in appropriate domain folder
// Scrips.Core/Models/Patient/PatientSearchRequest.cs

namespace Scrips.Core.Models.Patient;

/// <summary>
/// Request for searching patients by criteria.
/// </summary>
public class PatientSearchRequest
{
    /// <summary>
    /// Patient's first name (partial match supported).
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Patient's last name (partial match supported).
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}

// 2. Add to API client interface
// Scrips.Core/HttpApiClients/IPatientApi.cs

public interface IPatientApi
{
    [Post("/api/Patient/Search")]
    Task<List<PatientResponse>> SearchPatientsAsync(
        [Body] PatientSearchRequest request);
}
```

---

## Documentation

### 🚀 **For New Developers**

**Start here (in order):**
1. [README.md](README.md) ← You are here (5 min)
2. [ONBOARDING.md](ONBOARDING.md) - Hands-on first 2 hours (⭐ ESSENTIAL)
3. [CONTRIBUTING.md](CONTRIBUTING.md) - Coding standards (30 min)
4. [docs/cursor/ARCHITECTURE-DIAGRAM.md](docs/cursor/ARCHITECTURE-DIAGRAM.md) - Visual guide (15 min)
5. [docs/cursor/01-business-logic-outline.md](docs/cursor/01-business-logic-outline.md) - Domain overview (20 min)

**Then deep dive:**
- Domain docs (9 files, ~4,600 lines, all quality audited)
- Technical debt inventory (know the issues!)

### 🔒 **For Security Auditors**

- [docs/cursor/09-technical-debt-inventory.md](docs/cursor/09-technical-debt-inventory.md) - 47 debt items, 12 critical
- [docs/cursor/07-framework-audit.md](docs/cursor/07-framework-audit.md) - All dependencies with versions
- [docs/cursor/02-section-7-audit-compliance.md](docs/cursor/02-section-7-audit-compliance.md) - Audit logging (with critical issues)
- [docs/cursor/02-section-6-identity-tenancy.md](docs/cursor/02-section-6-identity-tenancy.md) - Multi-tenancy & JWT

### 🔧 **For DevOps Engineers**

- [docs/cursor/08-microservices-topology.md](docs/cursor/08-microservices-topology.md) - 11 services mapped
- [docs/cursor/07-framework-audit.md](docs/cursor/07-framework-audit.md) - Runtime, packages, upgrade readiness
- [docs/cursor/06-integration-details.md](docs/cursor/06-integration-details.md) - 18 integrations (tiered by criticality)

### 📋 **For Compliance Officers**

- [docs/cursor/02-section-2-patient-management.md](docs/cursor/02-section-2-patient-management.md) - PHI handling (88/100 audit score)
- [docs/cursor/02-section-3-billing-financial.md](docs/cursor/02-section-3-billing-financial.md) - Financial PHI (85/100 audit score)
- [docs/cursor/02-section-7-audit-compliance.md](docs/cursor/02-section-7-audit-compliance.md) - Audit trail (86/100 audit score)
- **HIPAA Status:** See [Known Issues](#known-issues) section below

---

## Known Issues

**⚠️ CRITICAL PRODUCTION READINESS CONCERNS:**

| Issue | Severity | Impact | Effort | Status |
|-------|----------|--------|--------|--------|
| **.NET 7.0 End-of-Support** | 🔴 CRITICAL | No security patches for 8+ months | 48-68h | **FIXED** (2026-03) -- Upgraded to .NET 8.0 LTS |
| **Audit log loss (Dapr fire-and-forget)** | 🔴 CRITICAL | HIPAA 164.308 violation if Dapr fails | 16h | Open |
| **MaskValueAudit incomplete** | 🔴 CRITICAL | PHI in plain text in audit logs | 8h | Open |
| **Zero test coverage** | 🔴 HIGH | Cannot verify PHI protection | 80-120h | Open |
| **Connection strings in plain text** | 🔴 HIGH | Database credentials exposed | 8h | Open |
| **No distributed cache** | ⚠️ MEDIUM | Performance issues, lookup failures | 40h | Open |
| **Azure Search no TTL** | ⚠️ MEDIUM | Indefinite PHI retention | 8h | Open |
| **JWT refresh not documented** | ⚠️ MEDIUM | Session hijacking risk | 8h | Investigation |

**📖 Full inventory:** [docs/cursor/09-technical-debt-inventory.md](docs/cursor/09-technical-debt-inventory.md) (47 items total)

---

## Healthcare Compliance

### HIPAA Status

| Requirement | Status | Notes |
|-------------|--------|-------|
| **PHI Identification** | ✅ PASS | All PHI fields documented |
| **Access Control** | ✅ PASS | JWT + role-based permissions |
| **Audit Trail** | ⚠️ PARTIAL | Implemented but fire-and-forget (can lose logs) |
| **Encryption at Rest** | ⚠️ NOT VERIFIED | Depends on database configuration |
| **Encryption in Transit** | ✅ PASS | TLS required (enforced at API gateway) |
| **Minimum Necessary** | ⚠️ PARTIAL | Not consistently enforced |
| **Data Retention** | ⚠️ PARTIAL | 7-year retention documented but not enforced |
| **Breach Notification** | ❌ NOT IMPLEMENTED | No automated breach detection |

**⚠️ Overall Assessment:** Compliant with gaps. See [Technical Debt Inventory](docs/cursor/09-technical-debt-inventory.md) for remediation plan.

### PHI Handling

**Protected fields (require `[MaskValueAudit]`):**
- Direct identifiers: Name, Email, Phone, SSN, MRN
- Date identifiers: DOB, service dates
- Indirect identifiers: Diagnoses, medications, chief complaints

**Multi-Tenancy:**
- **CRITICAL:** Every entity MUST have `OrganizationId`
- Global query filters enforce tenant isolation
- Cross-tenant data leak = HIPAA violation

**Audit Logging:**
- All entity changes automatically logged via `AuditableBaseDbContext`
- Published to Dapr "SaveAudit" topic
- ⚠️ **Known issue:** Fire-and-forget pattern can lose logs

---

## Contributing

We welcome contributions! Please read:

1. **[CONTRIBUTING.md](CONTRIBUTING.md)** - Coding standards and PR process
2. **[ONBOARDING.md](ONBOARDING.md)** - Get familiar with the codebase
3. **[docs/cursor/09-technical-debt-inventory.md](docs/cursor/09-technical-debt-inventory.md)** - Known issues (avoid introducing new ones!)

**Quick guidelines:**
- ✅ All public APIs must have XML documentation
- ✅ Mark all PHI fields with `[MaskValueAudit]`
- ✅ Every entity must have `OrganizationId` (multi-tenancy)
- ✅ Use async/await (never `.Result` or `.Wait()`)
- ✅ No empty catch blocks
- ✅ Test with multiple tenant IDs

---

## Support

### Internal Resources
- **Documentation:** [docs/cursor/README.md](docs/cursor/README.md) (28,000+ lines)
- **Onboarding:** [ONBOARDING.md](ONBOARDING.md) (2-hour hands-on guide)
- **Slack Channels:**
  - `#engineering` - General engineering discussion
  - `#engineering-onboarding` - New developer questions
  - `#healthcare-compliance` - HIPAA, PHI questions
  - `#devops` - Infrastructure, deployments

### Contacts
- **Technical Lead:** @tech-lead
- **Compliance Officer:** @compliance-officer
- **DevOps Team:** @devops-team
- **Security Team:** @security-team (for urgent security issues)

### Getting Help
1. **Search documentation first** - We have 28,000+ lines for a reason!
2. **Check known issues** - Your problem might already be documented
3. **Ask in Slack** - `#engineering-onboarding` for new developers
4. **Create an issue** - For bugs or feature requests

---

## License

Proprietary - All rights reserved.  
© 2024 Scrips Healthcare Platform  
Not for public distribution.

---

## Roadmap

### Q1 2026 (URGENT)
- ✅ **~~Upgrade to .NET 8 LTS~~** -- **FIXED** (2026-03): All projects now target .NET 8.0 LTS
- 🔴 **Fix audit log loss** (Implement retry + dead letter queue)
- 🔴 **Complete MaskValueAudit application** (Apply to all PHI fields)

### Q2 2026 (HIGH PRIORITY)
- ⚠️ **Implement unit tests** (Current coverage: 0%)
- ⚠️ **Secure connection strings** (Use Azure Key Vault)
- ⚠️ **Distributed cache** (Redis for lookup data)

### Q3 2026 (IMPROVEMENTS)
- 📋 Azure Search TTL policy (PHI retention)
- 📋 JWT refresh token mechanism
- 📋 Multi-tenancy isolation tests (automated)

### Q4 2026 (ENHANCEMENTS)
- 📋 Performance optimization
- 📋 API versioning strategy
- 📋 Telemetry and monitoring improvements

**📖 Full roadmap:** Tracked in JIRA sprint planning

---

## Quick Links

- 📖 **[ONBOARDING.md](ONBOARDING.md)** - Start here (new developers)
- 📋 **[CONTRIBUTING.md](CONTRIBUTING.md)** - Coding standards
- 🏗️ **[docs/cursor/ARCHITECTURE-DIAGRAM.md](docs/cursor/ARCHITECTURE-DIAGRAM.md)** - Visual diagrams
- 📚 **[docs/cursor/README.md](docs/cursor/README.md)** - Documentation index
- ⚠️ **[docs/cursor/09-technical-debt-inventory.md](docs/cursor/09-technical-debt-inventory.md)** - Known issues
- 🏥 **[docs/cursor/01-business-logic-outline.md](docs/cursor/01-business-logic-outline.md)** - Domain overview

---

**Made with ❤️ by the Scrips Engineering Team**

*Building better healthcare software, one commit at a time.*
