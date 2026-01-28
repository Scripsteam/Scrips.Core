# Framework Audit

## RUNTIME FRAMEWORK

- **Framework:** .NET 7.0
  - File: Scrips.Core.csproj:4
  - File: Scrips.Core.Application.csproj:4
  - File: Scrips.Infrastructure.csproj:4
  - File: Scrips.BaseDbContext.csproj:4
  - File: Scrips.WebApi.csproj:4
  - File: Scrips.Core.Domain.csproj:4
- **Status:** ⚠️ **END OF SUPPORT** (May 2024)
- **Recommendation:** Migrate to .NET 8 (LTS) immediately

## WEB FRAMEWORK

- **Framework:** ASP.NET Core (implicit via .NET 7.0)
- **API Versioning:** Microsoft.AspNetCore.Mvc.Versioning v5.0.0
  - File: Scrips.Infrastructure.csproj:14
- **HTTP Abstractions:** Microsoft.AspNetCore.Http v2.2.2
  - File: Scrips.Core.Application.csproj:14
  - File: Scrips.BaseDbContext.csproj:11

## DATA ACCESS

- **ORM:** Entity Framework Core v7.0.4
  - File: Scrips.BaseDbContext.csproj:13
- **Database Providers:**
  - SQL Server: Microsoft.Data.SqlClient v5.1.0 (Scrips.BaseDbContext.csproj:12)
  - SQL Server (Legacy): System.Data.SqlClient v4.8.5 (Scrips.Infrastructure.csproj:16)
- **Multi-Tenancy:** Finbuckle.MultiTenant.EntityFrameworkCore v6.10.0
  - File: Scrips.BaseDbContext.csproj:17

## TESTING

- **Unit:** NOT FOUND - No test frameworks in scanned projects
- **Integration:** NOT FOUND
- **Note:** Test projects may exist outside core library

## MAJOR LIBRARIES (Top 20)

| Library | Version | Purpose | File:Line |
|---------|---------|---------|-----------|
| MediatR | 12.0.1 | CQRS pattern, request/response pipeline | Scrips.Core.Application.csproj:13 |
| FluentValidation | 11.5.1 | Request validation | Scrips.Core.Application.csproj:10 |
| Mapster | 7.3.0 | Object-to-object mapping | Scrips.Core.Application.csproj:12 |
| Ardalis.Specification | 6.1.0 | Specification pattern for queries | Scrips.Core.Application.csproj:11 |
| Dapr.Client | 1.10.0 | Distributed app runtime, pub/sub | Scrips.BaseDbContext.csproj:10 |
| Refit | 6.3.2 | Declarative HTTP client | Scrips.Core.csproj:14 |
| Azure.Search.Documents | 11.6.0 | Azure Cognitive Search (AI) | Scrips.Core.csproj:8 |
| Serilog | 2.12.0 | Structured logging | Scrips.Infrastructure.csproj:15, Scrips.BaseDbContext.csproj:16 |
| Newtonsoft.Json | 13.0.3 | JSON serialization | Scrips.Core.csproj:9, Scrips.BaseDbContext.csproj:15 |
| Roslynator.Analyzers | 4.2.0 | Code analysis | All .csproj files |
| Microsoft.Extensions.Caching.Abstractions | 7.0.0 | Caching interface | Scrips.Core.Application.csproj:15 |
| Microsoft.Extensions.Configuration.Binder | 7.0.4 | Configuration binding | Scrips.Core.Application.csproj:16 |
| Microsoft.Extensions.Localization | 7.0.4 | Localization support | Scrips.Core.Application.csproj:17 |
| Microsoft.Extensions.Configuration.UserSecrets | 7.0.0 | User secrets management | Scrips.BaseDbContext.csproj:14 |
| Finbuckle.MultiTenant.EntityFrameworkCore | 6.10.0 | Multi-tenancy data isolation | Scrips.BaseDbContext.csproj:17 |
| Microsoft.AspNetCore.Mvc.Versioning | 5.0.0 | API versioning | Scrips.Infrastructure.csproj:14 |
| Protobuf (gRPC) | N/A | gRPC client definitions (9 .proto files) | Scrips.Core.csproj:18-26 |

## PRODUCTION DEPENDENCIES

### Scrips.Core Project (Core Models & API Clients)

| Package | Version | Purpose |
|---------|---------|---------|
| Azure.Search.Documents | 11.6.0 | AI-powered chief complaint search with vector embeddings |
| Newtonsoft.Json | 13.0.3 | JSON serialization for API responses |
| Refit | 6.3.2 | Type-safe HTTP API clients (11 services) |
| Protobuf Definitions | N/A | 9 gRPC service contracts compiled as clients |
| Roslynator.Analyzers | 4.2.0 | Static code analysis |

### Scrips.Core.Application Project (Application Layer)

| Package | Version | Purpose |
|---------|---------|---------|
| FluentValidation | 11.5.1 | Request DTO validation |
| Ardalis.Specification | 6.1.0 | Specification pattern for complex queries |
| Mapster | 7.3.0 | High-performance object mapping |
| MediatR | 12.0.1 | CQRS mediator pattern |
| Microsoft.AspNetCore.Http | 2.2.2 | HTTP context abstractions |
| Microsoft.Extensions.Caching.Abstractions | 7.0.0 | Cache service interface (not implemented) |
| Microsoft.Extensions.Configuration.Binder | 7.0.4 | Configuration binding |
| Microsoft.Extensions.Localization | 7.0.4 | Multi-language support |

### Scrips.BaseDbContext Project (Audit Infrastructure)

| Package | Version | Purpose |
|---------|---------|---------|
| Dapr.Client | 1.10.0 | Pub/sub for audit log distribution |
| Microsoft.AspNetCore.Http.Abstractions | 2.2.0 | HTTP context for audit logging |
| Microsoft.Data.SqlClient | 5.1.0 | SQL Server database provider |
| Microsoft.EntityFrameworkCore | 7.0.4 | ORM for database access |
| Microsoft.Extensions.Configuration.UserSecrets | 7.0.0 | Secure configuration storage |
| Newtonsoft.Json | 13.0.3 | JSON serialization for audit logs |
| Serilog | 2.12.0 | Structured logging |
| Finbuckle.MultiTenant.EntityFrameworkCore | 6.10.0 | Multi-tenant data isolation with global filters |

### Scrips.Infrastructure Project (Infrastructure Services)

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNetCore.Mvc.Versioning | 5.0.0 | API endpoint versioning |
| Serilog | 2.12.0 | Structured logging |
| System.Data.SqlClient | 4.8.5 | Legacy SQL Server provider (should use Microsoft.Data.SqlClient) |

### Scrips.WebApi Project (API Base Controllers)

| Package | Version | Purpose |
|---------|---------|---------|
| (No packages - minimal base) | N/A | Base controllers with MediatR integration |

### Scrips.Core.Domain Project (Domain Contracts)

| Package | Version | Purpose |
|---------|---------|---------|
| (No packages - pure domain) | N/A | Domain entity contracts and events |

## UPGRADE READINESS

### 🔴 CRITICAL Updates Needed

**1. .NET 7.0 → .NET 8 (LTS)**
- **Current:** .NET 7.0 (All 6 projects - .csproj:4)
- **Status:** End of support May 2024 - NO SECURITY PATCHES
- **Target:** .NET 8 (LTS support until November 2026)
- **Impact:** HIGH - Security vulnerability, no bug fixes
- **Effort:** Medium (4-8 hours) - Most libraries compatible
- **Blockers:** None identified - all packages have .NET 8 versions
- **Healthcare Risk:** Security patches critical for PHI protection

**2. System.Data.SqlClient → Microsoft.Data.SqlClient**
- **Current:** System.Data.SqlClient v4.8.5 (Scrips.Infrastructure.csproj:16)
- **Status:** LEGACY - superseded by Microsoft.Data.SqlClient
- **Target:** Microsoft.Data.SqlClient v5.1.0+ (already used in Scrips.BaseDbContext)
- **Impact:** MEDIUM - Legacy provider lacks new SQL Server features
- **Effort:** Low (1-2 hours) - Drop-in replacement
- **Note:** Scrips.BaseDbContext already uses modern provider - consolidate

**3. Microsoft.AspNetCore.Http v2.2.2**
- **Current:** 2.2.2 (Scrips.Core.Application.csproj:14, Scrips.BaseDbContext.csproj:11)
- **Status:** OLD - .NET Core 2.2 package (2019)
- **Target:** Use framework-provided version (implicit with .NET 8)
- **Impact:** MEDIUM - May have security vulnerabilities
- **Effort:** Low (1 hour) - Remove explicit reference after .NET 8 upgrade

### 🟡 RECOMMENDED Updates

**4. MediatR 12.0.1 → 12.4.0 (Latest)**
- **Current:** 12.0.1 (Scrips.Core.Application.csproj:13)
- **Latest:** 12.4.0 (January 2024)
- **Impact:** LOW - Minor improvements, bug fixes
- **Effort:** Low (1 hour) - Backward compatible

**5. FluentValidation 11.5.1 → 11.9.2 (Latest)**
- **Current:** 11.5.1 (Scrips.Core.Application.csproj:10)
- **Latest:** 11.9.2 (May 2024)
- **Impact:** LOW - Bug fixes, validation improvements
- **Effort:** Low (1 hour) - Backward compatible

**6. Refit 6.3.2 → 7.0.0 (Latest)**
- **Current:** 6.3.2 (Scrips.Core.csproj:14)
- **Latest:** 7.0.0 (May 2024)
- **Impact:** MEDIUM - Breaking changes in v7, new features
- **Effort:** Medium (2-4 hours) - Review breaking changes
- **Note:** Stay on 6.x or plan migration carefully

**7. Azure.Search.Documents 11.6.0 (Current)**
- **Status:** CURRENT - Latest stable version
- **No update needed**

**8. Dapr.Client 1.10.0 → 1.14.0 (Latest)**
- **Current:** 1.10.0 (Scrips.BaseDbContext.csproj:10)
- **Latest:** 1.14.0 (January 2024)
- **Impact:** MEDIUM - New features, reliability improvements
- **Effort:** Low (1-2 hours) - Backward compatible
- **Healthcare Impact:** Better retry mechanisms for audit logs

**9. Newtonsoft.Json 13.0.3 (Current)**
- **Status:** CURRENT - Latest version
- **Note:** Consider migrating to System.Text.Json for performance
- **Effort if migrated:** HIGH (8+ hours) - Breaking changes

**10. Serilog 2.12.0 → 3.1.1 (Latest)**
- **Current:** 2.12.0 (Scrips.Infrastructure.csproj:15, Scrips.BaseDbContext.csproj:16)
- **Latest:** 3.1.1 (January 2024)
- **Impact:** LOW - Bug fixes, performance improvements
- **Effort:** Low (1 hour) - Backward compatible

### 🟢 UP TO DATE

- Roslynator.Analyzers: 4.2.0 (Current)
- Microsoft.Extensions.* (7.0.x): Current for .NET 7, will update with .NET 8
- Microsoft.Data.SqlClient: 5.1.0 (Recent, stable)
- Mapster: 7.3.0 (Current)
- Ardalis.Specification: 6.1.0 (Current)

## HEALTHCARE IMPACT ANALYSIS

### Patient Safety

**Critical (Immediate Action Required):**
- **.NET 7.0 End-of-Support:** Security vulnerabilities in runtime could expose PHI
  - Impact: Database connections, encryption libraries, authentication
  - Risk: Unpatched security flaws exploitable by attackers
  - Action: Upgrade to .NET 8 within 30 days

**High (Upgrade Recommended):**
- **Legacy SQL Client:** Older SQL Server provider lacks modern security features
  - Impact: Database connections may be less secure
  - Risk: Missing connection encryption improvements
  - Action: Remove System.Data.SqlClient, use Microsoft.Data.SqlClient

**Medium (Monitor):**
- **Dapr.Client 1.10.0:** Older version may lack reliability improvements
  - Impact: Audit log delivery (fire-and-forget pattern already risky)
  - Risk: Increased audit log loss risk
  - Action: Update to 1.14.0 for better retry mechanisms

### Downtime Required

**Planned Maintenance Window Required:**

**Duration:** 2-4 hours for .NET 8 upgrade
**Timing:** Off-peak hours (weekend or night)
**Steps:**
1. Deploy to staging environment first
2. Run full integration test suite
3. Validate multi-tenant data isolation
4. Test PHI masking in audit logs
5. Verify Azure Cognitive Search integration
6. Deploy to production
7. Monitor audit logs for 24 hours

**Zero-Downtime Possible:**
- Blue-green deployment if using microservices
- Rolling update if containerized (Kubernetes/Docker)
- Requires load balancer and health checks

### Compliance Impact

**HIPAA/UAE Compliance Requirements:**

1. **Security Updates:** Running unsupported .NET 7.0 violates security best practices
   - Audit finding: "System not receiving security patches"
   - Recommendation: Document upgrade plan for compliance officers

2. **Audit Trail Integrity:** Dapr upgrade improves audit log reliability
   - Current: Fire-and-forget pattern (Risk #1 in architecture doc)
   - Improved: Better retry with Dapr 1.14.0

3. **Data Encryption:** Modern SQL client has better TLS/encryption support
   - Impact: PHI in transit protection
   - Requirement: TLS 1.2+ for all database connections

### Testing Requirements

**Critical Test Areas:**

1. **Multi-Tenancy:** Finbuckle.MultiTenant with .NET 8
   - Test: Tenant isolation after upgrade
   - Risk: Cross-tenant data leakage if broken

2. **Audit Logging:** MaskValueAudit attribute with EF Core 7
   - Test: PHI masking still works
   - Risk: PHI exposure in audit logs

3. **API Compatibility:** Refit HTTP clients after upgrade
   - Test: All 11 service integrations
   - Risk: Broken microservice communication

4. **Azure Search:** AI vector search after upgrade
   - Test: Chief complaint search functionality
   - Risk: Clinical decision support failure

5. **JWT Authentication:** Claims extraction after upgrade
   - Test: Multi-tenant context resolution
   - Risk: Authorization bypass

## DEPENDENCY HEALTH SUMMARY

| Category | Status | Count | Risk Level |
|----------|--------|-------|------------|
| Critical Updates | ⚠️ Needed | 3 | HIGH |
| Recommended Updates | 🟡 Suggested | 7 | MEDIUM |
| Up to Date | ✅ Current | 5+ | LOW |
| Deprecated | 🔴 Legacy | 1 | HIGH |
| Missing (Cache) | ⚠️ Not Impl | 1 | MEDIUM |

**Overall Health:** 🟡 **NEEDS ATTENTION**
- Primary concern: .NET 7.0 end-of-support
- Secondary: Legacy SQL client
- Recommendation: Schedule upgrade sprint immediately

## UPGRADE ROADMAP

**Phase 1 (Immediate - Week 1):**
1. Upgrade .NET 7.0 → .NET 8 (all 6 projects)
2. Remove System.Data.SqlClient, consolidate to Microsoft.Data.SqlClient
3. Remove explicit Microsoft.AspNetCore.Http references
4. Test in staging environment

**Phase 2 (Short-term - Week 2-3):**
5. Update Dapr.Client 1.10.0 → 1.14.0
6. Update FluentValidation 11.5.1 → 11.9.2
7. Update Serilog 2.12.0 → 3.1.1
8. Full regression testing

**Phase 3 (Medium-term - Month 2):**
9. Evaluate Refit 6.3.2 → 7.0.0 upgrade (breaking changes)
10. Implement cache (Redis) with encryption
11. Add Polly retry policies to Refit clients

**Phase 4 (Long-term - Quarter 2):**
12. Consider System.Text.Json migration (performance)
13. Implement integration tests for multi-tenancy
14. Add E2E tests for critical healthcare workflows

**Estimated Total Effort:** 40-60 hours
**Priority:** HIGH - Security and compliance driven
