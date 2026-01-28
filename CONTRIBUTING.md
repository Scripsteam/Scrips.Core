# Contributing to Scrips.Core

Engineering standards, coding patterns, and pull request guidelines for the Scrips healthcare platform.

**⚠️ Healthcare Context:** This codebase handles Protected Health Information (PHI). Security and compliance are non-negotiable.

---

## Table of Contents
- [Getting Started](#getting-started)
- [Coding Standards](#coding-standards)
- [Architecture Patterns](#architecture-patterns)
- [Security Requirements](#security-requirements) ⚠️ **CRITICAL**
- [Healthcare Compliance](#healthcare-compliance) ⚠️ **CRITICAL**
- [Testing Guidelines](#testing-guidelines)
- [Pull Request Process](#pull-request-process)
- [Common Mistakes](#common-mistakes)
- [Resources](#resources)

---

## Getting Started

### Branch Naming

Follow this convention:

```
feature/JIRA-123-short-description    # New features
bugfix/JIRA-456-fix-description       # Bug fixes
hotfix/critical-security-fix          # Production hotfixes
refactor/improve-performance          # Code improvements
docs/update-onboarding                # Documentation only
```

**Examples:**
```bash
git checkout -b feature/SCRIPS-789-add-preferred-language
git checkout -b bugfix/SCRIPS-456-fix-tenant-isolation
git checkout -b hotfix/audit-log-loss-critical
```

### Task Selection

**Good first tasks:**
- Add fields to existing DTOs
- Update documentation
- Add XML documentation to public APIs
- Fix compiler warnings

**Avoid as first tasks:**
- Multi-tenancy changes (high risk!)
- Audit logging changes (HIPAA compliance risk!)
- Base class modifications (affects all microservices)

---

## Coding Standards

### C# Naming Conventions

**✅ GOOD:**
```csharp
// Classes: PascalCase
public class PatientResponse { }

// Interfaces: IPascalCase
public interface IPatientRepository { }

// Methods: PascalCase
public async Task<PatientResponse> GetPatientAsync(Guid patientId) { }

// Properties: PascalCase
public string FirstName { get; set; }

// Private fields: _camelCase with underscore prefix
private readonly IPatientRepository _patientRepository;

// Parameters: camelCase
public void UpdatePatient(Guid patientId, string firstName) { }

// Local variables: camelCase
var patientData = await GetPatientAsync(patientId);

// Constants: PascalCase
public const string DefaultLanguage = "en";

// Async methods: Suffix with "Async"
public async Task<bool> SavePatientAsync(Patient patient) { }
```

**❌ BAD:**
```csharp
// Wrong casing
public class patientresponse { }              // Should be PatientResponse
public interface PatientRepository { }        // Should be IPatientRepository
public async Task<Patient> GetPatient() { }   // Should be GetPatientAsync

// Wrong field naming
private IPatientRepository patientRepository; // Should be _patientRepository

// Missing "Async" suffix
public async Task<bool> SavePatient() { }     // Should be SavePatientAsync
```

### File Organization

**One class per file:**
```
✅ GOOD:
PatientResponse.cs        → Contains PatientResponse class only
CreatePatientRequest.cs   → Contains CreatePatientRequest class only

❌ BAD:
PatientModels.cs          → Contains 10 different classes
```

**Namespace matches folder structure:**
```csharp
// File: Scrips.Core/Models/Patient/PatientResponse.cs
namespace Scrips.Core.Models.Patient;  // ✅ Matches folder structure

// File: Scrips.Core/Models/Patient/PatientResponse.cs
namespace Scrips.Core.Models;  // ❌ Doesn't match folder
```

**Organize usings:**
```csharp
// ✅ GOOD: System usings first, then third-party, then project
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Serilog;

using Scrips.Core.Domain.Contracts;
using Scrips.Core.Models.Patient;

namespace Scrips.Core.Application.Patients;

// ❌ BAD: Mixed order, no grouping
using Scrips.Core.Models.Patient;
using System;
using Serilog;
using System.Collections.Generic;
```

### XML Documentation (Required!)

**Every public API MUST have XML documentation:**

```csharp
/// <summary>
/// Retrieves a patient by their unique identifier.
/// </summary>
/// <param name="patientId">The unique identifier of the patient.</param>
/// <returns>A <see cref="PatientResponse"/> containing patient details.</returns>
/// <exception cref="NotFoundException">Thrown when patient is not found.</exception>
/// <remarks>
/// This endpoint returns PHI (Protected Health Information).
/// Requires "Permissions.Patients.View" permission.
/// Multi-tenant filtered by OrganizationId.
/// </remarks>
[HttpGet("{patientId}")]
public async Task<ActionResult<PatientResponse>> GetPatientAsync(Guid patientId)
{
    // Implementation
}
```

**⚠️ For PHI-related code, MUST document:**
- That it contains PHI
- Required permissions
- Multi-tenancy enforcement
- Audit logging behavior

```csharp
/// <summary>
/// Patient demographic information.
/// </summary>
/// <remarks>
/// ⚠️ PHI: Contains Protected Health Information (name, DOB, contact info).
/// Audit logged via AuditableBaseDbContext.
/// Multi-tenant filtered by OrganizationId.
/// </remarks>
public class PatientResponse
{
    /// <summary>
    /// Patient's first name. ⚠️ PHI: Masked in audit logs via [MaskValueAudit].
    /// </summary>
    public string FirstName { get; set; }
}
```

### Async/Await Patterns

**✅ ALWAYS use async/await for I/O operations:**
```csharp
// ✅ GOOD: Async all the way
public async Task<PatientResponse> GetPatientAsync(Guid patientId)
{
    var patient = await _context.Patients
        .FirstOrDefaultAsync(p => p.Id == patientId);
    
    return _mapper.Map<PatientResponse>(patient);
}

// ✅ GOOD: ConfigureAwait(false) in library code
public async Task<PatientResponse> GetPatientAsync(Guid patientId)
{
    var patient = await _context.Patients
        .FirstOrDefaultAsync(p => p.Id == patientId)
        .ConfigureAwait(false);  // ✅ Avoids deadlocks
    
    return _mapper.Map<PatientResponse>(patient);
}
```

**❌ NEVER block async code:**
```csharp
// ❌ BAD: Using .Result (deadlock risk!)
public PatientResponse GetPatient(Guid patientId)
{
    var patient = GetPatientAsync(patientId).Result;  // ❌ NEVER DO THIS
    return patient;
}

// ❌ BAD: Using .Wait() (deadlock risk!)
public PatientResponse GetPatient(Guid patientId)
{
    var task = GetPatientAsync(patientId);
    task.Wait();  // ❌ NEVER DO THIS
    return task.Result;
}
```

**⚠️ KNOWN ISSUE:** AuditableBaseDbContext.cs:46 uses `.Result` (see Technical Debt #3)

---

## Architecture Patterns

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│  Presentation (Scrips.WebApi)                           │
│  → Handles HTTP requests                                 │
│  → Controllers inherit from BaseApiController            │
│  → Calls Application layer via MediatR                   │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Application (Scrips.Core.Application)                  │
│  → Business logic and orchestration                      │
│  → Interfaces only (implementations in Infrastructure)   │
│  → No database or external dependencies                  │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Domain (Scrips.Core.Domain)                            │
│  → Business rules and entities                           │
│  → No dependencies on other layers                       │
│  → Pure C# classes                                       │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌─────────────────────────────────────────────────────────┐
│  Core (Scrips.Core)                                     │
│  → DTOs, Models, Enums                                   │
│  → NO dependencies (POCO objects only)                   │
└─────────────────────────────────────────────────────────┘
```

**Dependency Rule:** Outer layers can depend on inner layers, NEVER the reverse.

### CQRS Pattern (with MediatR)

**Command (writes data):**
```csharp
// Scrips.Core.Application/Patients/Commands/CreatePatient.cs
public class CreatePatientCommand : IRequest<PatientResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    // ... other fields
}

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientResponse>
{
    private readonly IApplicationDbContext _context;
    
    public CreatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<PatientResponse> Handle(
        CreatePatientCommand request, 
        CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            // ... map fields
        };
        
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new PatientResponse { /* map to response */ };
    }
}
```

**Query (reads data):**
```csharp
// Scrips.Core.Application/Patients/Queries/GetPatient.cs
public class GetPatientQuery : IRequest<PatientResponse>
{
    public Guid PatientId { get; set; }
}

public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, PatientResponse>
{
    private readonly IApplicationDbContext _context;
    
    public GetPatientQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<PatientResponse> Handle(
        GetPatientQuery request,
        CancellationToken cancellationToken)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        
        if (patient == null)
            throw new NotFoundException("Patient not found");
        
        return new PatientResponse { /* map to response */ };
    }
}
```

**Controller usage:**
```csharp
// In your API controller
public class PatientsController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<PatientResponse>> CreatePatient(
        [FromBody] CreatePatientCommand command)
    {
        return await Mediator.Send(command);  // MediatR magic!
    }
    
    [HttpGet("{patientId}")]
    public async Task<ActionResult<PatientResponse>> GetPatient(Guid patientId)
    {
        return await Mediator.Send(new GetPatientQuery { PatientId = patientId });
    }
}
```

**Why CQRS?**
- Separation of concerns (reads vs writes)
- Easy to test (handlers are isolated)
- Scalable (can optimize queries separately)
- Clear business intent

### Repository Pattern (Specification Pattern)

**Using Ardalis.Specification:**

```csharp
// Scrips.Core.Application/Patients/Specifications/PatientByIdSpec.cs
public class PatientByIdSpec : Specification<Patient>
{
    public PatientByIdSpec(Guid patientId)
    {
        Query
            .Where(p => p.Id == patientId)
            .Include(p => p.HealthInsurance)
            .Include(p => p.Guardian);
    }
}

// Usage in handler
public async Task<PatientResponse> Handle(...)
{
    var spec = new PatientByIdSpec(request.PatientId);
    var patient = await _repository.FirstOrDefaultAsync(spec);
    // ...
}
```

---

## Security Requirements ⚠️ **CRITICAL**

### SQL Injection Prevention

**✅ ALWAYS use parameterized queries:**

```csharp
// ✅ GOOD: EF Core (automatically parameterized)
var patients = await _context.Patients
    .Where(p => p.LastName == lastName)
    .ToListAsync();

// ✅ GOOD: Parameterized raw SQL
var patients = await _context.Patients
    .FromSqlRaw("SELECT * FROM Patients WHERE LastName = {0}", lastName)
    .ToListAsync();

// ✅ GOOD: Stored procedure with parameters
var patients = await _context.Patients
    .FromSqlRaw("EXEC GetPatientsByName @lastName", 
        new SqlParameter("@lastName", lastName))
    .ToListAsync();
```

**❌ NEVER concatenate user input into SQL:**

```csharp
// ❌ BAD: SQL injection vulnerability!
var sql = $"SELECT * FROM Patients WHERE LastName = '{lastName}'";
var patients = await _context.Patients.FromSqlRaw(sql).ToListAsync();

// ❌ BAD: String interpolation in SQL
var patients = await _context.Patients
    .FromSqlRaw($"SELECT * FROM Patients WHERE LastName = '{lastName}'")
    .ToListAsync();

// Attack example:
// lastName = "Smith' OR '1'='1"
// Results in: SELECT * FROM Patients WHERE LastName = 'Smith' OR '1'='1'
// Returns ALL patients (PHI leak!)
```

### Multi-Tenancy Enforcement

**✅ EVERY entity MUST have OrganizationId:**

```csharp
// ✅ GOOD: Multi-tenant entity
public class Patient : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }
    
    // ⚠️ CRITICAL: Required for multi-tenancy
    public Guid OrganizationId { get; set; }
    
    public string FirstName { get; set; }
    // ... other fields
}
```

**✅ EVERY query MUST filter by OrganizationId:**

```csharp
// ✅ GOOD: Filtered by OrganizationId (automatic via Finbuckle)
var patients = await _context.Patients
    .Where(p => p.Status == "Active")
    .ToListAsync();
// Finbuckle adds: WHERE OrganizationId = @tenantId

// ✅ GOOD: Explicit check for sensitive operations
public async Task<Patient> GetPatientAsync(Guid patientId, Guid currentTenantId)
{
    var patient = await _context.Patients
        .FirstOrDefaultAsync(p => p.Id == patientId);
    
    if (patient == null)
        throw new NotFoundException("Patient not found");
    
    // ⚠️ CRITICAL: Verify tenant ownership!
    if (patient.OrganizationId != currentTenantId)
        throw new UnauthorizedException("Access denied");
    
    return patient;
}
```

**❌ NEVER bypass multi-tenancy filters:**

```csharp
// ❌ BAD: Bypasses global query filter (data leak!)
var allPatients = await _context.Patients
    .IgnoreQueryFilters()  // ❌ NEVER DO THIS (unless you're a super admin)
    .ToListAsync();
```

**Test multi-tenancy:**
```csharp
// ⚠️ CRITICAL: Always test with different tenant IDs
[Fact]
public async Task GetPatient_DifferentTenant_ShouldThrow()
{
    // Arrange
    var patientId = CreatePatientInTenant("tenant-A");
    
    // Act & Assert
    await Assert.ThrowsAsync<UnauthorizedException>(() => 
        GetPatientAsync(patientId, tenantId: "tenant-B"));
}
```

### PHI Handling Rules

**✅ Mark all PHI fields with `[MaskValueAudit]`:**

```csharp
using Scrips.BaseDbContext;

public class Patient
{
    public Guid Id { get; set; }
    
    // ⚠️ PHI: Direct identifiers
    [MaskValueAudit]
    public string FirstName { get; set; }
    
    [MaskValueAudit]
    public string LastName { get; set; }
    
    [MaskValueAudit]
    public string Email { get; set; }
    
    [MaskValueAudit]
    public string PhoneNumber { get; set; }
    
    // ⚠️ PHI: Date quasi-identifier
    [MaskValueAudit]
    public DateTime? DateOfBirth { get; set; }
    
    // Non-PHI: OK to log
    public Guid OrganizationId { get; set; }
    public string Status { get; set; }
}
```

**❌ NEVER log PHI in error messages:**

```csharp
// ❌ BAD: PHI in error message
_logger.LogError($"Failed to update patient {patient.FirstName} {patient.LastName}");

// ✅ GOOD: Use ID instead
_logger.LogError($"Failed to update patient {patient.Id}");

// ❌ BAD: PHI in exception message
throw new Exception($"Invalid email: {patient.Email}");

// ✅ GOOD: Generic error
throw new ValidationException("Invalid email format");
```

### Error Handling (No Empty Catch Blocks!)

**✅ ALWAYS handle exceptions properly:**

```csharp
// ✅ GOOD: Log and rethrow
try
{
    await SaveAuditAsync(changes);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to save audit logs");
    throw;  // ✅ Rethrow to caller
}

// ✅ GOOD: Handle specific exception
try
{
    await _daprClient.PublishEventAsync("pubsub", "SaveAudit", changes);
}
catch (DaprException ex)
{
    _logger.LogError(ex, "Dapr unavailable, queuing audit log locally");
    await QueueAuditLogAsync(changes);  // Fallback strategy
}
```

**❌ NEVER use empty catch blocks:**

```csharp
// ❌ BAD: Swallows exception (KNOWN ISSUE in AuditableBaseDbContext.cs:31-34)
try
{
    _ = SaveAudit(changes).Result;
}
catch (Exception ex)
{
    Log.Error(ex.Message);  // ❌ Logs but doesn't throw
    // Audit logs silently lost!
}

// ✅ GOOD: Fix
try
{
    await SaveAudit(changes);
}
catch (Exception ex)
{
    _logger.LogError(ex, "CRITICAL: Audit log save failed");
    throw new AuditException("Audit logging failed - operation aborted", ex);
}
```

---

## Healthcare Compliance ⚠️ **CRITICAL**

### HIPAA Requirements

**The Minimum Necessary Rule:**
- Only request/return PHI fields that are needed
- Don't return full patient object if you only need ID

```csharp
// ❌ BAD: Returns all PHI
public async Task<PatientResponse> GetPatientForDropdown(Guid patientId)
{
    return await _repository.GetByIdAsync(patientId);
    // Returns: FirstName, LastName, DOB, Email, Phone, SSN, etc.
}

// ✅ GOOD: Returns only necessary fields
public async Task<PatientDropdownItem> GetPatientForDropdown(Guid patientId)
{
    var patient = await _repository.GetByIdAsync(patientId);
    return new PatientDropdownItem 
    { 
        PatientId = patient.Id,
        DisplayName = $"{patient.FirstName} {patient.LastName}"
        // Only what's needed for dropdown
    };
}
```

### PHI Fields to Protect

**Direct Identifiers (Always PHI):**
- Name (First, Last, Middle)
- Address (Street, City, ZIP)
- Email, Phone, Fax
- SSN, Medical Record Number (MRN)
- Health insurance ID
- Account numbers
- IP address (in some cases)
- Biometric identifiers (fingerprints, retina scans)
- Photos of face

**Date Quasi-Identifiers (PHI if combined with other data):**
- Date of Birth
- Admission/discharge dates
- Date of death
- Service dates

**Indirect Identifiers (PHI in healthcare context):**
- Age (if over 89)
- Diagnosis codes (ICD-10)
- Procedure codes (CPT)
- Medication names
- Lab results
- Chief complaint text

### Audit Trail Requirements

**What MUST be audited:**
```csharp
// ✅ Automatically audited via AuditableBaseDbContext
public class Patient : IAuditableEntity
{
    // These are automatically populated
    public Guid? CreatedBy { get; set; }       // Who created
    public DateTime CreatedOn { get; set; }     // When created
    public Guid? LastModifiedBy { get; set; }   // Who last modified
    public DateTime? LastModifiedOn { get; set; } // When last modified
}

// Entity changes automatically logged to Dapr "SaveAudit" topic
// Includes: EntityType, EntityId, Operation, OldValue, NewValue, UserId, TenantId, Timestamp
```

**Audit log requirements:**
- **Immutable:** Once written, cannot be changed
- **Complete:** All PHI access and modifications
- **Retention:** 6-7 years minimum
- **Protected:** Audit logs themselves are PHI

**⚠️ KNOWN ISSUE:** Fire-and-forget Dapr publish can lose audit logs (see Technical Debt #2)

---

## Testing Guidelines

### Manual Testing Checklist

**Before submitting PR:**

- [ ] **Build succeeds** - `dotnet build` (no warnings)
- [ ] **Multi-tenancy verified** - Tested with 2+ different tenant IDs
- [ ] **PHI protected** - Verified `[MaskValueAudit]` on all PHI fields
- [ ] **Error handling** - Tested error scenarios (null inputs, invalid IDs)
- [ ] **Audit logging** - Verified changes are logged to Dapr
- [ ] **Documentation** - XML comments on all public APIs
- [ ] **No sensitive data** - No hardcoded passwords, connection strings, etc.

### Unit Test Patterns (When Implemented)

⚠️ **Current status:** Zero test coverage (see Technical Debt #4)

**When tests are added, follow these patterns:**

```csharp
// Arrange-Act-Assert pattern
[Fact]
public async Task CreatePatient_ValidData_ShouldSucceed()
{
    // Arrange
    var command = new CreatePatientCommand 
    { 
        FirstName = "John",
        LastName = "Doe",
        // ...
    };
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("John", result.FirstName);
}
```

---

## Pull Request Process

### Before Creating PR

**Checklist:**
- [ ] Code builds without warnings
- [ ] All public APIs have XML documentation
- [ ] PHI fields marked with `[MaskValueAudit]`
- [ ] Multi-tenancy enforced (OrganizationId on entities)
- [ ] No hardcoded values (use configuration)
- [ ] No empty catch blocks
- [ ] No `.Result` or `.Wait()` on async code
- [ ] Tested with multiple tenant IDs
- [ ] Tested error scenarios

### PR Title Format

```
<type>(<scope>): <short description>

Types:
- feat: New feature
- fix: Bug fix
- refactor: Code change that neither fixes a bug nor adds a feature
- docs: Documentation only
- style: Code style (formatting, missing semicolons, etc.)
- perf: Performance improvement
- test: Adding tests
- chore: Maintenance (dependencies, build, etc.)

Examples:
feat(patient): add preferred language field
fix(audit): prevent audit log loss on Dapr failure
refactor(multi-tenancy): improve tenant resolution performance
docs(onboarding): add first code change tutorial
```

### PR Description Template

```markdown
## Description
Brief description of what this PR does.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update
- [ ] Refactoring

## Healthcare Compliance
- [ ] Contains PHI: YES / NO
- [ ] If YES, PHI fields marked with [MaskValueAudit]: YES / NO / N/A
- [ ] Multi-tenancy verified: YES / NO / N/A
- [ ] Audit logging verified: YES / NO / N/A

## Changes Made
- Change 1
- Change 2
- Change 3

## Testing Done
- [ ] Built successfully
- [ ] Manual testing completed
- [ ] Tested with multiple tenant IDs
- [ ] Tested error scenarios

## Related Issues
Closes #123
Related to #456

## Screenshots (if applicable)
```

### Code Review Checklist

**Reviewers must verify:**

**Security & Compliance:**
- [ ] No SQL injection vulnerabilities
- [ ] PHI properly protected (`[MaskValueAudit]` applied)
- [ ] Multi-tenancy enforced (OrganizationId present)
- [ ] No PHI in error messages or logs
- [ ] Audit logging works correctly

**Code Quality:**
- [ ] Follows naming conventions
- [ ] XML documentation on public APIs
- [ ] No empty catch blocks
- [ ] Proper async/await usage
- [ ] No `.Result` or `.Wait()`

**Architecture:**
- [ ] Correct layer (Domain/Application/Infrastructure)
- [ ] Follows CQRS pattern (if applicable)
- [ ] Dependencies point inward (Clean Architecture)

**Healthcare Specific:**
- [ ] HIPAA compliance maintained
- [ ] Minimum necessary principle followed
- [ ] Audit trail complete

### Addressing Feedback

**Be responsive:**
- Address all reviewer comments
- Ask questions if unclear
- Mark conversations as resolved when fixed

**Make changes:**
```bash
# Make the requested changes
git add .
git commit -m "refactor: address PR feedback - improve error handling"
git push origin feature/your-branch
# PR automatically updates
```

---

## Common Mistakes to Avoid

### Mistake 1: Missing OrganizationId

**❌ BAD:**
```csharp
public class MyNewEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // ❌ Missing OrganizationId - DATA LEAK!
}
```

**✅ GOOD:**
```csharp
public class MyNewEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }  // ✅ Multi-tenancy
    public string Name { get; set; }
}
```

### Mistake 2: Not Masking PHI

**❌ BAD:**
```csharp
public class Patient
{
    public string FirstName { get; set; }  // ❌ PHI not masked!
}
```

**✅ GOOD:**
```csharp
using Scrips.BaseDbContext;

public class Patient
{
    [MaskValueAudit]  // ✅ Masked in audit logs
    public string FirstName { get; set; }
}
```

### Mistake 3: Empty Catch Blocks

**❌ BAD:**
```csharp
try
{
    await SaveChangesAsync();
}
catch
{
    // ❌ Silent failure!
}
```

**✅ GOOD:**
```csharp
try
{
    await SaveChangesAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to save changes");
    throw;
}
```

### Mistake 4: Blocking Async Code

**❌ BAD:**
```csharp
var result = GetDataAsync().Result;  // ❌ Deadlock risk!
```

**✅ GOOD:**
```csharp
var result = await GetDataAsync();  // ✅ Proper async
```

### Mistake 5: PHI in Error Messages

**❌ BAD:**
```csharp
throw new Exception($"Patient {patient.FirstName} not found");
// ❌ PHI in error message!
```

**✅ GOOD:**
```csharp
throw new NotFoundException($"Patient {patientId} not found");
// ✅ Use ID, not PHI
```

---

## Additional Resources

**Internal Documentation:**
- [ONBOARDING.md](./ONBOARDING.md) - Getting started guide
- [docs/cursor/README.md](./docs/cursor/README.md) - Documentation index
- [docs/cursor/09-technical-debt-inventory.md](./docs/cursor/09-technical-debt-inventory.md) - Known issues
- [docs/cursor/05-architecture.md](./docs/cursor/05-architecture.md) - Architecture details

**External Resources:**
- [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
- [HIPAA for Developers](https://www.hhs.gov/hipaa/for-professionals/security/laws-regulations/index.html)
- [CQRS Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)

---

**Remember:** Security and compliance are everyone's responsibility. If you see something, say something!

*Last updated: January 2026*  
*Questions? Ask in #engineering or create an issue*
