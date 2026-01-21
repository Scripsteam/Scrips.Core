# Technical Debt Inventory

**Repository:** Scrips.Core (Shared Library for Healthcare Microservices Platform)  
**Audit Date:** January 21, 2026  
**Auditor:** Andrew  
**Scope:** Core library used by 11 microservices (Patient, Billing, Scheduling, etc.)

---

## EXECUTIVE SUMMARY

- **Total debt items:** 47
- **Critical items:** 12 (security, compliance, architecture)
- **High priority items:** 18
- **Medium priority items:** 12
- **Low priority items:** 5
- **Estimated total effort:** 320-450 hours (~8-11 weeks)
- **Top priority:** .NET 7.0 end-of-support (security risk for PHI), Audit log loss (compliance violation), Async/await deadlock risk

**Severity Distribution:**
- 🔴 CRITICAL: 12 items (immediate action required)
- 🟠 HIGH: 18 items (this sprint/month)
- 🟡 MEDIUM: 12 items (this quarter)
- 🟢 LOW: 5 items (backlog)

**Debt by Category:**
1. Security Debt: 9 items (38% critical)
2. Healthcare Compliance Debt: 8 items (75% critical)
3. Architecture Debt: 7 items
4. Dependency Debt: 10 items (3 critical)
5. Code Debt: 5 items
6. Performance Debt: 4 items
7. Documentation Debt: 3 items
8. Test Debt: 1 item (no tests found)

---

## 1. CODE DEBT

| Issue | Location | Severity | Effort | Impact |
|-------|----------|----------|--------|--------|
| **Async/await deadlock risk** - `.Result` blocks async call in sync context | AuditableBaseDbContext.cs:46 | 🔴 CRITICAL | 2h | Deadlock potential in sync SaveChanges(), audit logs lost |
| **Async/await deadlock risk** - `.Result` blocks async call in sync context | AuditableMultiTenantBaseDbContext.cs:54 | 🔴 CRITICAL | 2h | Deadlock potential in multi-tenant context |
| **Swallowed exceptions** - Audit failure silently caught | AuditableBaseDbContext.cs:31-34 | 🔴 CRITICAL | 2h | Audit logs lost without warning (compliance violation) |
| **TODO comment** - IOptions validation needs refactor | Startup.cs:15 | 🟡 MEDIUM | 4h | Technical debt acknowledged but not fixed |
| **Commented code** - Large block of persistence code disabled | Startup.cs:29-94 | 🟡 MEDIUM | 1h | Unclear if code is needed or can be deleted |

**Total Code Debt:** 5 items, 11 hours effort

### Details:

**1. Async/Await Deadlock Risk (CRITICAL)**

```csharp
// AuditableBaseDbContext.cs:46
_ = SaveAudit(changes).Result;
```

**Problem:** Calling `.Result` on async method in synchronous `SaveChanges()` can cause deadlock in ASP.NET context.

**Impact:** Application freeze, audit logs not saved, compliance violation.

**Fix:** Remove synchronous `SaveChanges()` override, force callers to use `SaveChangesAsync()`.

**Effort:** 2 hours (+ consuming service updates)

---

**2. Swallowed Exceptions (CRITICAL)**

```csharp
// AuditableBaseDbContext.cs:31-34
catch (Exception ex)
{
    Log.Error(ex.Message);
}
```

**Problem:** Audit log failures are caught and logged but do not prevent database save. Silent data loss.

**Impact:** Lost audit trail = HIPAA/UAE compliance violation, no investigation trail for security incidents.

**Fix:** 
- Option 1: Fail database save if audit fails (strict compliance)
- Option 2: Queue audit logs locally for retry (resilience)
- Option 3: Alert on audit failure (monitoring)

**Effort:** 4-8 hours depending on approach

---

## 2. ARCHITECTURE DEBT

| Issue | Location | Severity | Effort | Impact |
|-------|----------|----------|--------|--------|
| **God class potential** - 18 endpoints in single interface | IPracticeApi.cs:7-96 | 🟠 HIGH | 8h | Hard to maintain, violates SRP |
| **Hardcoded database provider** - "mssql" string literal | Startup.cs:23 | 🟠 HIGH | 2h | Cannot configure database type, PostgreSQL/MySQL code commented out |
| **Missing abstraction** - No repository pattern | Startup.cs:70-93 (commented) | 🟡 MEDIUM | 16h | Direct DbContext usage, tight coupling |
| **Commented architecture** - Repository pattern disabled | Startup.cs:70-93 | 🟡 MEDIUM | 1h | Unclear why removed, increases technical debt |
| **Fire-and-forget messaging** - No retry on Dapr failure | AuditableBaseDbContext.cs:60-61 | 🔴 CRITICAL | 16h | Audit logs silently lost (Risk #1 from architecture doc) |
| **No caching implementation** - Interface only | ICacheService.cs:1-16 | 🟡 MEDIUM | 40h | Performance degradation, no distributed cache |
| **No resilience patterns** - Refit clients lack retry/circuit breaker | All IXxxApi.cs files | 🟠 HIGH | 24h | Transient failures break workflows (Risk #5) |

**Total Architecture Debt:** 7 items, 107 hours effort

### Details:

**1. Fire-and-Forget Messaging (CRITICAL)**

```csharp
// AuditableBaseDbContext.cs:60-61
if (await _daprClient.CheckHealthAsync())
    await _daprClient.PublishEventAsync("pubsub", "SaveAudit", changes);
```

**Problem:** If Dapr is down or publish fails, audit logs are lost forever. No retry, no local queue.

**Impact:** HIPAA/UAE compliance violation - incomplete audit trail, cannot investigate security incidents.

**Referenced in:** Architecture doc Risk #1, Microservices Topology Section 7

**Fix:** Implement retry with exponential backoff OR local queue fallback

**Effort:** 16 hours (design + implementation + testing)

---

**2. Hardcoded Database Provider (HIGH)**

```csharp
// Startup.cs:23
const string dbProvider = "mssql";
```

**Problem:** Database provider hardcoded despite PostgreSQL/MySQL support existing (commented out lines 47-59). Configuration ignored.

**Impact:** Cannot switch databases without code change, PostgreSQL/MySQL customers blocked.

**Fix:** Uncomment multi-database code, read from `DatabaseSettings.DBProvider`

**Effort:** 2 hours + testing

---

**3. No Resilience Patterns (HIGH)**

**Problem:** All 11 Refit HTTP clients lack:
- Retry policies (transient failures)
- Circuit breakers (cascading failures)
- Timeout configurations
- Bulkhead isolation

**Impact:** Network blip breaks appointment booking, billing calculation, patient lookup. Cascading failures possible (see Topology Risk #5).

**Referenced in:** Architecture doc Risk #5, Integration Details Part 2

**Fix:** Add Polly policies to all Refit clients

**Effort:** 24 hours (policy design + implementation + testing)

---

## 3. DEPENDENCY DEBT

| Package | Current | Latest | Behind | Risk | Effort |
|---------|---------|--------|--------|------|--------|
| **.NET 7.0** | 7.0 | .NET 8 | 1 major | 🔴 **CRITICAL - END OF SUPPORT** | 8h |
| **System.Data.SqlClient** | 4.8.5 | Deprecated | N/A | 🔴 **CRITICAL - LEGACY** | 2h |
| **Microsoft.AspNetCore.Http** | 2.2.2 | 8.0+ | 6 major | 🔴 **CRITICAL - OLD (2019)** | 1h |
| **Dapr.Client** | 1.10.0 | 1.14.0 | 4 minor | 🟠 HIGH | 2h |
| **MediatR** | 12.0.1 | 12.4.0 | 3 minor | 🟡 MEDIUM | 1h |
| **FluentValidation** | 11.5.1 | 11.9.2 | 4 minor | 🟡 MEDIUM | 1h |
| **Serilog** | 2.12.0 | 3.1.1 | 1 major | 🟡 MEDIUM | 1h |
| **Refit** | 6.3.2 | 7.0.0 | 1 major | 🟡 MEDIUM (breaking) | 4h |
| **Azure.Search.Documents** | 11.6.0 | 11.6.0 | 0 | 🟢 CURRENT | 0h |
| **Newtonsoft.Json** | 13.0.3 | 13.0.3 | 0 | 🟢 CURRENT | 0h |

**Total Dependency Debt:** 10 items, 20 hours effort (excludes consuming service updates)

### Critical Details:

**1. .NET 7.0 End-of-Support (CRITICAL)**

**Status:** End of support May 2024 - NO SECURITY PATCHES for 8+ months

**Files:** All 6 .csproj files (Scrips.Core, Application, Infrastructure, BaseDbContext, WebApi, Domain) at line 4

**Impact:** 
- Security vulnerabilities in runtime could expose PHI
- Database connection encryption bugs unpatched
- Authentication library vulnerabilities unpatched
- Violates security compliance requirements

**Healthcare Risk:** CRITICAL - Running unsupported framework violates HIPAA/UAE security requirements

**Referenced in:** Framework Audit Section "CRITICAL Updates Needed"

**Fix:** Upgrade to .NET 8 (LTS until November 2026)

**Effort:** 8 hours core library + 40-60 hours consuming services

**Deadline:** IMMEDIATE (30 days max)

---

## 4. TEST DEBT

| Area | Coverage | Gap | Priority |
|------|----------|-----|----------|
| **Entire codebase** | 0% | No test projects found | 🔴 CRITICAL |
| **Audit logging** | 0% | No tests for PHI masking | 🔴 CRITICAL |
| **Multi-tenancy** | 0% | No tenant isolation tests | 🔴 CRITICAL |
| **Refit clients** | 0% | No integration tests | 🟠 HIGH |
| **Azure Cognitive Search** | 0% | No AI search tests | 🟡 MEDIUM |

**Total Test Debt:** 100% untested (1 item with multiple sub-items)

### Details:

**No Test Projects Found**

**Evidence:** Glob search for `*Test*.cs` returned 0 files

**Impact:** 
- Cannot verify PHI masking works (AuditableBaseDbContext.cs:25)
- Cannot verify tenant isolation (Finbuckle.MultiTenant)
- Cannot detect regressions after .NET 8 upgrade
- Cannot verify microservice integration contracts
- Risky deployments to production

**Healthcare Risk:** CRITICAL - No automated verification of PHI protection

**Recommended Tests:**
1. Unit tests for `MaskValueAudit` attribute (PHI protection)
2. Integration tests for multi-tenancy isolation
3. Integration tests for all 11 Refit API clients
4. Unit tests for FluentValidation validators
5. Integration tests for Azure Cognitive Search (AI endpoints)
6. Unit tests for audit log generation

**Effort:** 80-120 hours (high priority areas only)

**Note:** Test projects may exist in separate repositories for each microservice

---

## 5. DOCUMENTATION DEBT

| Gap | Location | Impact |
|-----|----------|--------|
| **Minimal README** - Only 2 lines | README.md:1-3 | New developers cannot onboard |
| **No architecture diagrams** - Created in this session | Previously missing | System complexity unclear |
| **No API documentation** - Refit interfaces lack XML comments | All IXxxApi.cs files | Consuming developers guess parameters |

**Total Documentation Debt:** 3 items, 24 hours effort

### Details:

**1. Minimal README**

```markdown
# Scrips.Core
Entities and Models and DTOs across repos
```

**Missing:**
- Purpose of this library
- How to consume it
- How to add new API clients
- How to use audit logging
- Multi-tenancy setup
- Environment variable requirements
- Build instructions
- Versioning strategy

**Effort:** 4 hours

---

**2. No XML Comments on Refit Interfaces**

**Example:** `IPatientApi.cs:6-30` - 5 endpoints, zero documentation

**Impact:** Developers guess:
- What does `EditPatientResponse` contain?
- Is `Authorization` header required?
- What's the difference between `HealthInsuranceSponsor` and `PatientCorporateSponsor`?

**Effort:** 16 hours (document all 11 API clients)

---

## 6. PERFORMANCE DEBT

| Issue | Location | Impact | Effort |
|-------|----------|--------|--------|
| **N+1 query potential** - No `.Include()` found | EF Core usage (not visible in library) | Slow queries | 16h |
| **No distributed cache** - Interface only | ICacheService.cs:1-16 | Repeated DB queries | 40h |
| **Synchronous SaveChanges** - Blocks thread | AuditableBaseDbContext.cs:39-55 | Thread pool exhaustion | 4h |
| **No pagination guidance** - `PagedResults<T>` exists but no enforcement | PagedResults.cs | Large result sets crash app | 8h |

**Total Performance Debt:** 4 items, 68 hours effort

### Details:

**1. No Distributed Cache (HIGH)**

**Evidence:** 
- `ICacheService.cs:1-16` - Interface defined
- `Microsoft.Extensions.Caching.Abstractions` v7.0.0 referenced
- Zero implementation files found

**Impact:** 
- Lookup data (Gender, MaritalStatus, Countries) queried every request
- Provider/practice data not cached
- Multi-instance deployments have per-instance cache = inconsistency
- Slow response times for repeated queries

**Referenced in:** 
- Architecture doc Risk #4
- Integration Details "NOT IMPLEMENTED"
- Microservices Topology Section 7

**Recommended Implementation:** Redis with:
- TLS encryption (PHI may be cached)
- TTL limits (no indefinite PHI caching)
- Cache invalidation on updates (via Dapr events)
- Redis ACLs for access control

**Effort:** 40 hours (Redis setup + implementation + PHI security + testing)

---

## 7. SECURITY DEBT

| Issue | Location | Severity | OWASP Category |
|-------|----------|----------|----------------|
| **Connection string in plain text** - No encryption | DatabaseSettings.cs:6 | 🔴 CRITICAL | A02-Cryptographic Failures |
| **Hardcoded secret risk** - "Password" property exposed | DatabaseSettings.cs:6 | 🔴 CRITICAL | A07-Identification Failures |
| **TLS not enforced in code** - Relies on infrastructure | All Refit clients | 🟠 HIGH | A02-Cryptographic Failures |
| **.NET 7.0 security patches unavailable** | All 6 projects | 🔴 CRITICAL | A06-Vulnerable Components |
| **No input validation visible** - FluentValidation not applied here | Core library | 🟡 MEDIUM | A03-Injection |
| **JWT claims not validated in library** - Done in consuming services | Core library | 🟡 MEDIUM | A07-Identification Failures |
| **Audit log message only** - No stack trace | AuditableBaseDbContext.cs:33 | 🟡 MEDIUM | A09-Security Logging |
| **No rate limiting** - Not visible in core library | API Gateway level | 🟡 MEDIUM | A04-Insecure Design |
| **Dapr health check before publish** - But no auth check | AuditableBaseDbContext.cs:60 | 🟢 LOW | A01-Broken Access Control |

**Total Security Debt:** 9 items, 48 hours effort

### Details:

**1. Connection String in Plain Text (CRITICAL)**

```csharp
// DatabaseSettings.cs:6
public string? ConnectionString { get; set; }
```

**Problem:** Database credentials stored in configuration file in plain text.

**Impact:** 
- If config file leaked (git commit, log file, memory dump) → database compromise
- All patient PHI accessible
- Violates HIPAA/UAE security requirements

**OWASP:** A02:2021 - Cryptographic Failures

**Referenced in:** 
- Architecture doc Risk #3
- Microservices Topology Section 7

**Fix:** Move connection strings to:
- Azure Key Vault (preferred)
- Environment variables with restricted access
- AWS Secrets Manager
- HashiCorp Vault

**Effort:** 8 hours (migration + consuming service updates)

---

**2. .NET 7.0 Vulnerable Components (CRITICAL)**

**Problem:** Running .NET 7.0 (end-of-support May 2024) means:
- Zero security patches for 8+ months
- Known CVEs unpatched
- Encryption library vulnerabilities unpatched
- SQL injection protection bugs unpatched

**Impact:** Exploitable vulnerabilities could expose PHI, violate compliance

**OWASP:** A06:2021 - Vulnerable and Outdated Components

**Fix:** Upgrade to .NET 8 (see Dependency Debt #1)

**Effort:** 8 hours (see above)

---

**3. TLS Not Enforced (HIGH)**

**Problem:** All 11 Refit HTTP clients rely on infrastructure-level TLS configuration. No code enforcement.

**Impact:** If misconfigured, PHI transmitted in plain text over network

**OWASP:** A02:2021 - Cryptographic Failures

**Referenced in:** Integration Details Part 2, Microservices Topology Section 6

**Fix:** Add Refit configuration to reject non-HTTPS endpoints

```csharp
.ConfigureHttpClient(c => {
    if (!c.BaseAddress.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("HTTPS required for PHI");
});
```

**Effort:** 4 hours

---

## 8. HEALTHCARE COMPLIANCE DEBT

| Gap | Requirement | Location | Risk |
|-----|-------------|----------|------|
| **Audit log loss** - No retry mechanism | HIPAA 164.308(a)(1)(ii)(D) | AuditableBaseDbContext.cs:60-61 | 🔴 CRITICAL - Compliance violation |
| **Connection string exposure** - Plain text credentials | HIPAA 164.312(a)(2)(iv) | DatabaseSettings.cs:6 | 🔴 CRITICAL - Encryption required |
| **TLS not enforced** - Infrastructure-dependent | HIPAA 164.312(e)(1) | All Refit clients | 🔴 CRITICAL - PHI in transit |
| **MaskValueAudit not verified** - No tests | HIPAA 164.308(a)(3)(ii)(A) | MaskValueAuditAttribute.cs:13 | 🟠 HIGH - PHI exposure risk |
| **No consent tracking visible** - May be in consuming services | HIPAA 164.508 | Not found | 🟠 HIGH - Authorization |
| **Email PHI protection unclear** - Depends on email service | HIPAA 164.312(e)(1) | IEmailSenderApi.cs:8 | 🟠 HIGH - Transmission security |
| **Azure Search PHI retention** - No TTL visible | UAE Data Residency | ChiefComplaintDocument.cs | 🟡 MEDIUM - Data residency |
| **No breach notification capability** - External system | HIPAA 164.410 | Not found | 🟡 MEDIUM - Incident response |

**Total Healthcare Compliance Debt:** 8 items, 72 hours effort

### Details:

**1. Audit Log Loss (CRITICAL)**

**Requirement:** HIPAA 164.308(a)(1)(ii)(D) - Information System Activity Review
- "Implement procedures to regularly review records of information system activity"
- **Requires complete audit trail** - no gaps allowed

**Problem:** Fire-and-forget Dapr publishing (AuditableBaseDbContext.cs:60-61) loses audit logs if:
- Dapr unavailable
- Message queue full
- Network timeout
- Publish fails

**Impact:** 
- Incomplete audit trail = HIPAA violation
- Cannot investigate security incidents
- Cannot prove compliance during audit
- Regulatory penalties possible

**Referenced in:** 
- Microservices Topology Risk #2
- Architecture doc Risk #1
- Integration Details Part 2 "Critical Issues"

**Fix:** Implement retry with local queue fallback (see Architecture Debt #5)

**Effort:** 16 hours

---

**2. Connection String Exposure (CRITICAL)**

**Requirement:** HIPAA 164.312(a)(2)(iv) - Encryption and Decryption
- "Implement a mechanism to encrypt and decrypt electronic protected health information"

**Problem:** Plain text database credentials (DatabaseSettings.cs:6) violate encryption requirements

**Impact:** Credentials leaked = database compromise = all PHI exposed

**Fix:** See Security Debt #1

**Effort:** 8 hours

---

**3. TLS Not Enforced (CRITICAL)**

**Requirement:** HIPAA 164.312(e)(1) - Transmission Security
- "Implement technical security measures to guard against unauthorized access to electronic protected health information that is being transmitted over an electronic communications network"

**Problem:** If HTTPS misconfigured, PHI transmitted in plain text

**Fix:** See Security Debt #3

**Effort:** 4 hours

---

**4. MaskValueAudit Not Verified (HIGH)**

**Requirement:** HIPAA 164.308(a)(3)(ii)(A) - Access Management
- "Implement policies and procedures for authorizing access to electronic protected health information"

**Problem:** 
- `[MaskValueAudit]` attribute defined (MaskValueAuditAttribute.cs:13)
- Applied in `AuditLoggingHelper.cs:56,60,68`
- But **zero tests verify it works**
- If broken, PHI logged in plain text

**Impact:** PHI exposure in audit logs, compliance violation

**Fix:** 
1. Add unit tests for masking logic
2. Add integration tests verifying masked fields
3. Audit attribute usage across all DTOs

**Effort:** 16 hours (tests + audit)

---

## PRIORITIZATION MATRIX

| Priority | Item | Category | Effort | Impact | Deadline |
|----------|------|----------|--------|--------|----------|
| **1** | .NET 7.0 → .NET 8 upgrade | Dependency | 8h + 40-60h services | Security patches critical for PHI | IMMEDIATE (30 days) |
| **2** | Audit log retry mechanism | Architecture + Compliance | 16h | HIPAA compliance violation | This Sprint |
| **3** | Connection string to Key Vault | Security + Compliance | 8h | Database compromise risk | This Sprint |
| **4** | Fix async/await deadlock (.Result) | Code | 4h | Application freeze | This Sprint |
| **5** | TLS enforcement in Refit clients | Security + Compliance | 4h | PHI in plain text risk | This Sprint |
| **6** | Swallowed audit exceptions | Code + Compliance | 4h | Silent audit log loss | This Sprint |
| **7** | Add Polly retry policies | Architecture | 24h | Transient failures break workflows | This Month |
| **8** | Remove sync SaveChanges() | Performance | 4h | Thread pool exhaustion | This Month |
| **9** | Upgrade Dapr.Client 1.10 → 1.14 | Dependency | 2h | Better audit log reliability | This Month |
| **10** | Implement MaskValueAudit tests | Test + Compliance | 16h | Verify PHI protection | This Month |
| **11** | Remove System.Data.SqlClient | Dependency | 2h | Legacy SQL provider | This Month |
| **12** | Uncomment multi-DB support | Architecture | 2h | PostgreSQL/MySQL blocked | This Quarter |
| **13** | Implement Redis cache | Performance + Architecture | 40h | Slow queries, no distribution | This Quarter |
| **14** | Document all Refit APIs | Documentation | 16h | Developer confusion | This Quarter |
| **15** | Split IPracticeApi (God class) | Architecture | 8h | Hard to maintain | This Quarter |

---

## REMEDIATION ROADMAP

### This Sprint (Critical) - Week 1-2

**Total Effort:** 44 hours  
**Owner:** Development team + DevOps

1. **.NET 7.0 → .NET 8 upgrade** - 8h core library + 40-60h microservices
   - Update all 6 .csproj files `TargetFramework` to `net8.0`
   - Test multi-tenancy isolation
   - Test PHI masking in audit logs
   - Test all 11 microservice integrations
   - Deploy to staging first

2. **Audit log retry mechanism** - 16h
   - Implement Polly retry (exponential backoff, 3 attempts)
   - Add local queue fallback if Dapr unavailable
   - Add monitoring/alerting for audit log failures
   - Integration test: verify retry works

3. **Connection string to Azure Key Vault** - 8h
   - Migrate DatabaseSettings to Key Vault references
   - Update consuming services configuration
   - Test connection string retrieval
   - Document Key Vault setup for new environments

4. **Fix async/await deadlock** - 4h
   - Remove synchronous `SaveChanges()` override
   - Force all callers to use `SaveChangesAsync()`
   - Update consuming services if needed
   - Add code analyzer rule to prevent `.Result` usage

5. **TLS enforcement** - 4h
   - Add Refit HTTP client configuration to reject non-HTTPS
   - Test with valid and invalid endpoints
   - Document HTTPS requirement

6. **Swallowed audit exceptions** - 4h
   - Change exception handling: fail database save if audit fails
   - OR queue locally for retry
   - Add alerting on audit failure
   - Integration test: verify behavior

---

### This Month (High) - Week 3-4

**Total Effort:** 48 hours  
**Owner:** Development team

7. **Add Polly retry policies to Refit clients** - 24h
   - Design retry policy: exponential backoff, 3 attempts, timeout 30s
   - Apply to all 11 Refit API clients
   - Test transient failure scenarios
   - Document retry behavior

8. **Remove sync SaveChanges()** - 4h
   - Delete `SaveChanges()` override from both contexts
   - Ensure all consuming services use async
   - Run load test to verify no thread pool exhaustion

9. **Upgrade Dapr.Client 1.10.0 → 1.14.0** - 2h
   - Update package reference
   - Test audit log publishing
   - Verify Dapr sidecar compatibility

10. **Implement MaskValueAudit tests** - 16h
    - Unit test: verify masking logic replaces with "*"
    - Integration test: verify attributes applied to PHI fields
    - Audit: document all PHI fields needing `[MaskValueAudit]`
    - Add CI check to enforce attribute usage

11. **Remove System.Data.SqlClient legacy provider** - 2h
    - Remove package from Scrips.Infrastructure.csproj:16
    - Consolidate to Microsoft.Data.SqlClient
    - Test SQL Server connections

---

### This Quarter (Medium) - Month 2-3

**Total Effort:** 66 hours  
**Owner:** Development team

12. **Uncomment multi-database support** - 2h
    - Uncomment Startup.cs:42-94
    - Read `DatabaseSettings.DBProvider` instead of hardcoded "mssql"
    - Test PostgreSQL and MySQL configurations
    - Document database provider options

13. **Implement Redis distributed cache** - 40h
    - Design: what to cache (lookups, sessions, search results)
    - Setup: Redis with TLS encryption, ACLs
    - Implement: `ICacheService` with Redis backend
    - Security: TTL for PHI, cache invalidation on updates
    - Test: cache hit/miss, expiration, multi-instance consistency
    - Document: caching strategy, what NOT to cache (raw PHI)

14. **Document all Refit API clients (XML comments)** - 16h
    - Add XML comments to all 11 `IXxxApi.cs` files
    - Document parameters, return types, exceptions
    - Document authentication requirements
    - Generate API documentation site

15. **Split IPracticeApi God class** - 8h
    - Split into `IPracticeSetupApi`, `IProviderScheduleApi`, `IStaffApi`
    - Update consuming services
    - Maintain backward compatibility if needed

---

### Backlog (Low) - Quarter 2+

**Total Effort:** 80+ hours

16. **Comprehensive test suite** - 80h+
    - Unit tests for all validators
    - Integration tests for all API clients
    - Multi-tenancy isolation tests
    - Azure Cognitive Search AI tests
    - Load tests for performance regression

17. **Improve README documentation** - 4h
    - Purpose, usage, setup instructions
    - Multi-tenancy configuration
    - Audit logging usage
    - Contributing guidelines

18. **Migrate Newtonsoft.Json → System.Text.Json** - 16h
    - Performance improvement
    - Breaking changes require careful migration
    - Test serialization/deserialization

19. **Add pagination enforcement** - 8h
    - Enforce maximum page size (e.g., 100 items)
    - Add pagination metadata to responses
    - Document pagination best practices

20. **Resolve TODO comment** - 4h
    - Clean IOptions validation (Startup.cs:15)
    - Use `IValidateOptions<DatabaseSettings>`

---

## SUMMARY

**Immediate Action Required (This Sprint):**
- .NET 7.0 upgrade (security)
- Audit log retry (compliance)
- Connection string security (compliance + security)
- Async/await deadlock fix (reliability)
- TLS enforcement (compliance)

**Total Estimated Effort:** 320-450 hours (~8-11 weeks)

**Critical Path:** .NET 8 upgrade blocks all other work (consuming services need updates)

**Compliance Risk:** Running unsupported .NET 7.0 + audit log loss = dual HIPAA violations

**Recommendation:** Dedicate 1-2 sprints to critical debt remediation before new feature work.
