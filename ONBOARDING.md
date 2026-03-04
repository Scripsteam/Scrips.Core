# Welcome to Scrips.Core! 🏥

Your practical guide to getting started with the Scrips healthcare platform's core shared library.

**⏱️ Time to productivity:** ~2-3 hours (following this guide)

---

## What You'll Learn

By the end of this guide, you'll be able to:
- ✅ Build and run the Scrips.Core library locally
- ✅ Understand the architecture and domain structure
- ✅ Navigate the codebase confidently
- ✅ Make your first code change
- ✅ Understand healthcare compliance requirements (PHI, HIPAA)
- ✅ Know where to find help

---

## Prerequisites

### Required Tools
- **Visual Studio 2022** (17.4+) OR **VS Code** with C# extension
- **.NET 8.0 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server** (LocalDB, Express, or Developer Edition)
- **Git** for version control
- **Postman** or similar API testing tool (optional but recommended)

### Optional Tools
- **Docker Desktop** (if running Dapr locally)
- **Azure Storage Emulator** (for file storage testing)
- **SQL Server Management Studio** (SSMS) for database work

### Knowledge Prerequisites
- C# and .NET fundamentals
- REST API basics
- Basic SQL knowledge
- Healthcare context helpful but not required (we'll teach you!)

---

## Quick Start (30 minutes)

### Step 1: Clone the Repository

```bash
# Clone the repository
git clone https://github.com/YourOrg/Scrips.Core.git
cd Scrips.Core

# Create a feature branch (never work on main!)
git checkout -b feature/your-name-getting-started
```

### Step 2: Open the Solution

**Option A: Visual Studio**
```bash
# Open the main solution
start Scrips.Core.sln
```

**Option B: VS Code**
```bash
code .
```

### Step 3: Build the Solution

```bash
# Restore NuGet packages
dotnet restore

# Build all projects
dotnet build

# Verify build succeeded (should show "Build succeeded")
```

**Expected output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

⚠️ **If build fails:** Check that you have .NET 8.0 SDK installed:
```bash
dotnet --list-sdks
# Should show: 8.0.xxx [path]
```

### Step 4: Understanding What You Just Built

**What is Scrips.Core?**

Scrips.Core is a **shared library** (not a standalone application). It provides:
- **DTOs** (Data Transfer Objects) - Models for API communication
- **API Clients** - Refit interfaces for calling other microservices
- **Base Classes** - Database contexts, controllers, domain entities
- **Shared Logic** - Authorization, auditing, multi-tenancy

**It does NOT:**
- Run as a standalone service (no `Program.cs` to execute)
- Have its own database (provides base classes for other services)
- Handle HTTP requests directly (provides base controllers for other services)

**Think of it as:** A toolbox that other microservices import as a NuGet package.

---

## Learning Path (30 minutes)

### 📚 Recommended Reading Order

**Day 1 (2-3 hours total):**

1. **This document** (you're here!) - 30 min ⏱️
2. **Root README.md** - Project overview - 5 min
3. **CONTRIBUTING.md** - Coding standards - 30 min
4. **docs/cursor/ARCHITECTURE-DIAGRAM.md** - Visual guide - 15 min
5. **docs/cursor/01-business-logic-outline.md** - Domain overview - 20 min

**Week 1 (Pick relevant domains):**

6. **docs/cursor/02-section-1-appointments-scheduling.md** - Appointments domain (with audit) - 1 hour
7. **docs/cursor/02-section-2-patient-management.md** - Patients domain (with audit) - 1 hour
8. **docs/cursor/07-framework-audit.md** - Technology stack - 30 min
9. **docs/cursor/09-technical-debt-inventory.md** - Known issues - 1 hour ⚠️ **Read this!**

**As Needed (Reference):**

10. Other domain docs (Billing, Clinical AI, etc.) - 1 hour each
11. **docs/cursor/05-architecture.md** - Deep architectural patterns - 1 hour
12. **docs/cursor/08-microservices-topology.md** - System-wide view - 1 hour

---

## Your First Code Change (30 minutes)

Let's make a realistic change: **Add a new field to an existing DTO**.

### Scenario: Add "PreferredLanguage" to Patient Model

**Business context:** We need to track patient preferred language for better care coordination.

### Step 1: Find the File

Navigate to: `Scrips.Core\Models\Patient\PatientResponse.cs`

```bash
# Use VS Code quick open
Ctrl+P (Windows) or Cmd+P (Mac)
# Type: PatientResponse.cs
```

### Step 2: Understand the Current Code

```csharp
// Scrips.Core\Models\Patient\PatientResponse.cs
namespace Scrips.Core.Models.Patient;

public class PatientResponse
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    // ... other fields
}
```

**⚠️ PHI Alert:** This model contains Protected Health Information (PHI):
- FirstName, LastName = Direct identifiers
- DateOfBirth = Date quasi-identifier
- Email, PhoneNumber = Contact info

### Step 3: Add the New Field

Add this property to the class:

```csharp
/// <summary>
/// Patient's preferred language for communication (e.g., "en", "ar", "ur").
/// ISO 639-1 language codes.
/// </summary>
public string? PreferredLanguage { get; set; }
```

**Why nullable (`?`):** Not all existing patients will have this field populated.

### Step 4: Update Related DTOs (if needed)

Check if there's a corresponding request DTO:

```bash
# Search for related files
Ctrl+Shift+F (Windows) or Cmd+Shift+F (Mac)
# Search: "CreatePatientRequest" or "UpdatePatientRequest"
```

Add the same field to request DTOs for consistency.

### Step 5: Build and Verify

```bash
dotnet build

# Check for errors
# Expected: Build succeeded
```

### Step 6: Check for Healthcare Compliance

**Important questions to ask yourself:**
1. **Is this field PHI?** 
   - Preferred language alone is NOT direct PHI
   - But combined with other data, could be indirect identifier
   - **Decision:** Treat as low-risk PII

2. **Does it need audit logging?**
   - Changes should be logged via `AuditableBaseDbContext`
   - ✅ Automatic (no action needed)

3. **Does it need masking in audit logs?**
   - Not sensitive enough to require `[MaskValueAudit]` attribute
   - ✅ Plain text in audit logs is OK

4. **Does it need validation?**
   - Should validate against ISO 639-1 codes
   - ⚠️ Add to backlog for Phase 2

### Step 7: Document Your Change

Create a simple commit:

```bash
git add Scrips.Core/Models/Patient/PatientResponse.cs
git commit -m "feat(patient): add PreferredLanguage field to PatientResponse

- Add PreferredLanguage property for language tracking
- Field is nullable for backward compatibility
- Uses ISO 639-1 language codes (e.g., 'en', 'ar', 'ur')
- No PHI masking required (low-risk PII)"
```

**✅ You just made your first code change!**

### What Happens Next?

In a real microservice that uses Scrips.Core:
1. Import updated Scrips.Core NuGet package
2. Add database migration for `PreferredLanguage` column
3. Update API endpoints to accept/return the field
4. Update UI to display the field
5. Add validation for ISO 639-1 codes

---

## Codebase Navigation (30 minutes)

### Understanding the Project Structure

```
Scrips.Core/
├── Scrips.Core/                    # Main library (DTOs, Models, API Clients)
│   ├── Models/                     # Domain models (DTOs)
│   │   ├── Patient/                # Patient-related models
│   │   ├── Scheduling/             # Appointment models
│   │   ├── Billing/                # Invoice, payment models
│   │   ├── Practice/               # Provider, practice models
│   │   ├── AIChiefComplaint/       # Clinical AI models
│   │   └── ...                     # 9 domain areas total
│   ├── Enums/                      # Enumerations by domain
│   ├── HttpApiClients/             # Refit API interfaces (11 services)
│   ├── Protos/                     # gRPC proto definitions (9 services)
│   ├── Shared/                     # Cross-cutting concerns
│   │   ├── Authorization/          # Claims, permissions, roles
│   │   └── Notifications/          # Event models
│   └── Topics.cs                   # Dapr pub/sub topic names
│
├── Scrips.Core.Domain/             # Domain entities and contracts
│   ├── Contracts/                  # Base interfaces (ISoftDelete, etc.)
│   └── ...
│
├── Scrips.Core.Application/        # Application layer
│   ├── Common/
│   │   ├── Caching/                # ICacheService (⚠️ not implemented)
│   │   ├── Events/                 # IEventPublisher
│   │   ├── FileStorage/            # IFileStorageService
│   │   └── Interfaces/             # IJobService (Hangfire)
│   └── Auditing/                   # IAuditService
│
├── Scrips.BaseDbContext/           # Database base classes
│   ├── AuditableBaseDbContext.cs   # Auto audit logging (⚠️ has critical issues)
│   ├── AuditableMultiTenantBaseDbContext.cs  # Multi-tenant + audit
│   └── MaskValueAuditAttribute.cs  # PHI masking attribute
│
├── Scrips.Infrastructure/          # Infrastructure layer
│   ├── Auth/                       # JWT configuration
│   ├── Middleware/                 # Tenant resolution, exception handling
│   └── Persistence/                # Database settings
│
├── Scrips.WebApi/                  # Base API controllers
│   ├── BaseApiController.cs        # MediatR integration
│   ├── VersionedApiController.cs   # API versioning support
│   └── ...
│
└── docs/                           # Documentation (28,000+ lines!)
    └── cursor/                     # Comprehensive technical docs
        ├── 00-workspace-inventory.md
        ├── 01-business-logic-outline.md
        ├── 02-section-X-*.md       # 9 domain docs (all audited!)
        ├── 05-architecture.md
        ├── 06-integration-details.md
        ├── 07-framework-audit.md
        ├── 08-microservices-topology.md
        ├── 09-technical-debt-inventory.md
        └── README.md               # Documentation index
```

### Key Files Tour (5 min each)

#### 1. **Scrips.Core/Models/Patient/PatientResponse.cs**
**Purpose:** Patient data transfer object (DTO)  
**When to use:** Returning patient data from API  
**PHI:** ✅ YES - Contains name, DOB, email, phone  
**Key learning:** All patient fields

#### 2. **Scrips.Core/HttpApiClients/IPatientApi.cs**
**Purpose:** Refit interface for calling Patient microservice  
**When to use:** Any service needs to call Patient API  
**Pattern:** Declarative HTTP client (no manual HttpClient code)
```csharp
public interface IPatientApi
{
    [Get("/api/Patient/{patientId}")]
    Task<PatientResponse> GetPatientAsync(Guid patientId);
}
```

#### 3. **Scrips.BaseDbContext/AuditableBaseDbContext.cs**
**Purpose:** Base class for DbContext with automatic audit logging  
**When to use:** Every microservice's DbContext inherits from this  
**⚠️ Critical issue:** Fire-and-forget Dapr publish (audit logs can be lost)
```csharp
// Automatically logs all entity changes to Dapr pub/sub
public override async Task<int> SaveChangesAsync(...)
{
    var changes = AuditLoggingHelper.DetectChanges(ChangeTracker, ...);
    await SaveAudit(changes);  // ⚠️ No retry if Dapr fails
    return await base.SaveChangesAsync(...);
}
```

#### 4. **Scrips.Core/Shared/Authorization/ScripsPermissions.cs**
**Purpose:** Central permission definitions  
**When to use:** Checking if user can perform action  
**Pattern:** Static string constants for permissions
```csharp
public static class ScripsPermissions
{
    public static class Patients
    {
        public const string View = "Permissions.Patients.View";
        public const string Create = "Permissions.Patients.Create";
        // ...
    }
}
```

#### 5. **Scrips.Core/Topics.cs**
**Purpose:** Dapr pub/sub topic names  
**When to use:** Publishing or subscribing to events  
**Pattern:** Domain events for system integration
```csharp
public class Topics
{
    public const string TenantCreated = "TenantCreated";
    public const string DoctorCreated = "DoctorCreated";
    public const string SaveAudit = "SaveAudit";
    // ...
}
```

### Architecture Layers (Clean Architecture)

```
┌─────────────────────────────────────────────────────────┐
│  Scrips.WebApi (Presentation Layer)                     │
│  - BaseApiController.cs                                  │
│  - API Controllers in consuming microservices            │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Scrips.Core.Application (Application Layer)            │
│  - Interfaces (IEventPublisher, ICacheService, etc.)    │
│  - Common utilities                                      │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Scrips.Core.Domain (Domain Layer)                      │
│  - Domain entities                                       │
│  - Contracts (ISoftDelete, IAuditableEntity, etc.)      │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Scrips.Core (Core Layer)                               │
│  - DTOs, Models, Enums                                   │
│  - No dependencies (pure POCO classes)                   │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│  Scrips.Infrastructure (Infrastructure Layer)           │
│  - Database, auth, middleware implementations            │
│  - Can depend on any layer except Presentation           │
└─────────────────────────────────────────────────────────┘
```

---

## Common Tasks

### Task 1: Add a New API Endpoint (in consuming microservice)

**Context:** You're working on a microservice that uses Scrips.Core.

**Step 1:** Define the request/response DTOs in Scrips.Core

```csharp
// Scrips.Core/Models/Patient/GetPatientHistoryRequest.cs
namespace Scrips.Core.Models.Patient;

public class GetPatientHistoryRequest
{
    public Guid PatientId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

// Scrips.Core/Models/Patient/PatientHistoryResponse.cs
public class PatientHistoryResponse
{
    public List<AppointmentSummary> Appointments { get; set; }
    public List<InvoiceSummary> Invoices { get; set; }
    // ...
}
```

**Step 2:** Add to the API client interface

```csharp
// Scrips.Core/HttpApiClients/IPatientApi.cs
public interface IPatientApi
{
    // ... existing methods

    [Post("/api/Patient/History")]
    Task<PatientHistoryResponse> GetPatientHistoryAsync(
        [Body] GetPatientHistoryRequest request);
}
```

**Step 3:** Implement in the microservice (not in Scrips.Core!)

```csharp
// In Patient.API project (separate repository)
[HttpPost("History")]
public async Task<ActionResult<PatientHistoryResponse>> GetHistory(
    [FromBody] GetPatientHistoryRequest request)
{
    // Implementation here
}
```

### Task 2: Add Multi-Tenancy to a New Entity

```csharp
// Scrips.Core.Domain/YourEntity.cs
using Scrips.Core.Domain.Contracts;

public class YourEntity : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }
    
    // ⚠️ CRITICAL: Every entity MUST have OrganizationId for multi-tenancy
    public Guid OrganizationId { get; set; }
    
    // Your fields here
    public string Name { get; set; }
    
    // Audit fields (from IAuditableEntity)
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    
    // Soft delete (from ISoftDelete)
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
}
```

**Why OrganizationId is CRITICAL:**
- Finbuckle.MultiTenant applies global query filters
- Without it, data leaks across tenants
- **Data leak = HIPAA violation**

### Task 3: Mark a Field as PHI (require masking in audit logs)

```csharp
using Scrips.BaseDbContext;

public class PatientEntity
{
    public Guid Id { get; set; }
    
    // ⚠️ PHI: Mask in audit logs
    [MaskValueAudit]
    public string FirstName { get; set; }
    
    [MaskValueAudit]
    public string LastName { get; set; }
    
    [MaskValueAudit]
    public string Email { get; set; }
    
    // Non-PHI: Don't mask
    public Guid OrganizationId { get; set; }
}
```

**When the entity changes, audit logs will show:**
```
OldValue: "*"  // Masked!
NewValue: "*"  // Masked!
```

**⚠️ Known issue:** MaskValueAudit not consistently applied (see Technical Debt #4)

### Task 4: Debug Multi-Tenancy Issues

**Symptom:** User sees data from another organization (CRITICAL BUG!)

**Debug steps:**

1. **Check JWT token has tenant claim:**
```csharp
// In your API controller
var tenantId = User.FindFirst("tenant")?.Value;
if (tenantId == null)
{
    return Unauthorized("Missing tenant claim in JWT");
}
```

2. **Verify entity has OrganizationId:**
```sql
-- Check if entity is missing OrganizationId
SELECT * FROM YourTable WHERE OrganizationId IS NULL
-- ⚠️ Should be ZERO rows!
```

3. **Check global query filter is applied:**
```csharp
// In your DbContext.OnModelCreating()
modelBuilder.Entity<YourEntity>()
    .HasQueryFilter(e => e.OrganizationId == _tenantId);
```

4. **Use SQL Profiler to verify:**
```sql
-- Every query should have WHERE OrganizationId = @tenantId
-- If not, multi-tenancy is BROKEN
```

---

## Where to Find Help

### Documentation

1. **This guide** - Practical getting started
2. **CONTRIBUTING.md** - Coding standards and patterns
3. **docs/cursor/README.md** - Documentation index (comprehensive!)
4. **docs/cursor/09-technical-debt-inventory.md** - Known issues (READ THIS!)

### Common Issues & Solutions

#### Issue: "Build failed - .NET SDK not found"
**Solution:**
```bash
# Check installed SDKs
dotnet --list-sdks

# Install .NET 8.0 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0
```

#### Issue: "Dapr not running - audit logs fail"
**Context:** This is a KNOWN CRITICAL ISSUE (see Technical Debt #2)  
**Workaround:** Ensure Dapr sidecar is running before testing
```bash
dapr run --app-id patient-api --app-port 5000 -- dotnet run
```

#### Issue: "Multi-tenancy data leak - seeing wrong tenant's data"
**Solution:**
1. Verify OrganizationId on all entities
2. Check JWT has "tenant" claim
3. Verify Finbuckle.MultiTenant is configured
4. See docs/cursor/02-section-6-identity-tenancy.md

#### Issue: "Empty catch block swallowing exceptions"
**Context:** KNOWN CRITICAL ISSUE in AuditableBaseDbContext.cs:31-34  
**Impact:** Audit logs silently lost  
**Workaround:** Monitor Dapr logs separately

### Team Contacts

- **Architecture questions:** @Tech-Lead
- **Healthcare compliance:** @Compliance-Officer
- **DevOps / Infrastructure:** @DevOps-Team
- **Security issues:** @Security-Team (report immediately!)

### External Resources

- **.NET 8 Documentation:** https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8
- **Clean Architecture:** https://github.com/jasontaylordev/CleanArchitecture
- **HIPAA Compliance:** https://www.hhs.gov/hipaa/for-professionals/security/laws-regulations/index.html
- **Dapr Documentation:** https://docs.dapr.io/

---

## Important Notes

### ⚠️ Known Critical Issues

Before you start coding, be aware of these CRITICAL issues (from Technical Debt Inventory):

| Issue | Impact | Status | Effort |
|-------|--------|--------|--------|
| **.NET 7.0 End-of-Support** | No security patches for 8+ months | ✅ **FIXED** (2026-03) | -- |
| **Audit log loss (Dapr fire-and-forget)** | HIPAA violation if Dapr unavailable | 🔴 URGENT | 16h |
| **MaskValueAudit incomplete** | PHI exposure in audit logs | 🔴 URGENT | 8h |
| **Zero test coverage** | Cannot verify PHI protection | 🔴 HIGH | 80-120h |
| **No distributed cache** | Performance issues, lookup failures | ⚠️ MEDIUM | 40h |

**Read the full list:** docs/cursor/09-technical-debt-inventory.md

### Healthcare Compliance Reminders

**Every time you write code, ask yourself:**

1. **Is this field PHI?**
   - Direct identifiers: Name, SSN, MRN, Email, Phone
   - Dates: DOB, service dates
   - Indirect: Appointment times, diagnoses
   - **If YES:** Add `[MaskValueAudit]` attribute

2. **Does this query filter by OrganizationId?**
   - Multi-tenancy MUST be enforced
   - Every entity MUST have OrganizationId
   - **If NO:** You have a data leak bug!

3. **Are changes being audited?**
   - All PHI access/changes must be logged
   - Audit logs must be immutable
   - Retention: 6-7 years minimum

4. **Are errors exposing PHI?**
   - Don't log PHI in error messages
   - Don't return PHI in error responses
   - Sanitize all error output

### Performance Considerations

- **No caching implemented:** Lookup data queries hit database every time (see Technical Debt #6)
- **Azure Search has no TTL:** PHI retained indefinitely (see Technical Debt #8)
- **Async/await deadlock risk:** `.Result` in sync SaveChanges() (see Technical Debt #3)

---

## Next Steps

### Week 1: Foundation
- [ ] Complete this onboarding guide (you're almost done!)
- [ ] Read CONTRIBUTING.md (coding standards)
- [ ] Read domain docs for your team's area (1-2 hours each)
- [ ] Make your first small change (like the PreferredLanguage example)
- [ ] Get your PR reviewed

### Week 2: Domain Expertise
- [ ] Deep dive into 2-3 domain documentation files
- [ ] Understand healthcare compliance requirements (HIPAA, PHI)
- [ ] Study multi-tenancy implementation
- [ ] Fix a small bug or add a simple feature

### Week 3: System Understanding
- [ ] Read architecture documentation (docs/cursor/05-architecture.md)
- [ ] Read microservices topology (docs/cursor/08-microservices-topology.md)
- [ ] Understand Dapr pub/sub patterns
- [ ] Understand CQRS pattern (MediatR)

### Month 2: Contributing
- [ ] Take on medium-complexity tasks
- [ ] Review others' pull requests
- [ ] Suggest improvements to documentation
- [ ] Help onboard the next new developer!

---

## Final Checklist

Before you consider yourself "onboarded":

- [ ] I can build Scrips.Core locally
- [ ] I understand what Scrips.Core is (shared library, not standalone app)
- [ ] I know the 9 domain areas (Scheduling, Patient, Billing, etc.)
- [ ] I understand Clean Architecture layers
- [ ] I know what PHI is and how to protect it
- [ ] I know multi-tenancy MUST be enforced (OrganizationId)
- [ ] I know about the CRITICAL issues (audit log loss, MaskValueAudit, etc.)
- [ ] I've made my first code change successfully
- [ ] I know where to find help (docs, team contacts)
- [ ] I've read CONTRIBUTING.md coding standards

---

**✅ Welcome to the team!**

You're now ready to contribute to Scrips.Core. Remember:
- **Ask questions** - especially about healthcare compliance
- **Read the docs** - we have 28,000+ lines for a reason!
- **Test thoroughly** - especially multi-tenancy and PHI handling
- **Be honest** - if you see a compliance issue, speak up immediately

**Next:** Read CONTRIBUTING.md to understand coding standards and PR process.

---

*Last updated: March 2026*  
*Maintained by: Engineering Team*  
*Questions? Create an issue or ask in #engineering-onboarding*
