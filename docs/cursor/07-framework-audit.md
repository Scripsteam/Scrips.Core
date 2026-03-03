# Framework Audit

## RUNTIME FRAMEWORK

- **Framework:** .NET 8.0 (LTS)
  - File: Scrips.Core.csproj:4
  - File: Scrips.Core.Application.csproj:4
  - File: Scrips.Infrastructure.csproj:4
  - File: Scrips.BaseDbContext.csproj:4
  - File: Scrips.WebApi.csproj:4
  - File: Scrips.Core.Domain.csproj:4
- **Status:** ✅ **CURRENT** - LTS support until November 2026
- **Note:** **FIXED** (2026-03) -- Upgraded from .NET 7.0 (which was EOL since May 2024)

## WEB FRAMEWORK

- **Framework:** ASP.NET Core (implicit via .NET 8.0)
- **API Versioning:** Asp.Versioning.Mvc v8.1.0
  - File: Scrips.Infrastructure.csproj:14, Scrips.WebApi.csproj:11

## DATA ACCESS

- **ORM:** Entity Framework Core v8.0.11
  - File: Scrips.BaseDbContext.csproj:19-20
- **Database Providers:**
  - SQL Server: Microsoft.Data.SqlClient v5.2.2 (Scrips.BaseDbContext.csproj:18, Scrips.Infrastructure.csproj:17)
- **Multi-Tenancy:** Finbuckle.MultiTenant.EntityFrameworkCore v6.10.0
  - File: Scrips.BaseDbContext.csproj:28

## TESTING

- **Unit:** NOT FOUND - No test frameworks in scanned projects
- **Integration:** NOT FOUND
- **Note:** Test projects may exist outside core library

## MAJOR LIBRARIES (Top 20)

| Library | Version | Purpose | File:Line |
|---------|---------|---------|-----------|
| MediatR | 12.0.1 | CQRS pattern, request/response pipeline | Scrips.Core.Application.csproj:17 |
| FluentValidation | 11.10.0 | Request validation | Scrips.Core.Application.csproj:14 |
| Mapster | 7.4.0 | Object-to-object mapping | Scrips.Core.Application.csproj:16 |
| Ardalis.Specification | 8.0.0 | Specification pattern for queries | Scrips.Core.Application.csproj:15 |
| Dapr.Client | 1.14.0 | Distributed app runtime, pub/sub | Scrips.BaseDbContext.csproj:15 |
| Refit | 8.0.0 | Declarative HTTP client | Scrips.Core.csproj:18 |
| Azure.Search.Documents | 11.6.0 | Azure Cognitive Search (AI) | Scrips.Core.csproj:9 |
| Serilog | 4.0.2 | Structured logging | Scrips.BaseDbContext.csproj:27 |
| Newtonsoft.Json | 13.0.4 | JSON serialization | Scrips.Core.csproj:12, Scrips.BaseDbContext.csproj:26 |
| Google.Protobuf | 3.33.5 | gRPC protocol buffers | Scrips.BaseDbContext.csproj:16 |
| Microsoft.Extensions.Caching.Abstractions | 8.0.0 | Caching interface | Scrips.Core.Application.csproj:18 |
| Microsoft.Extensions.Configuration.Binder | 8.0.2 | Configuration binding | Scrips.Core.Application.csproj:19 |
| Microsoft.Extensions.Localization | 8.0.10 | Localization support | Scrips.Core.Application.csproj:20 |
| Microsoft.Extensions.Configuration.UserSecrets | 8.0.1 | User secrets management | Scrips.BaseDbContext.csproj:21 |
| Finbuckle.MultiTenant.EntityFrameworkCore | 6.10.0 | Multi-tenancy data isolation | Scrips.BaseDbContext.csproj:28 |
| Asp.Versioning.Mvc | 8.1.0 | API versioning | Scrips.Infrastructure.csproj:14, Scrips.WebApi.csproj:11 |
| Protobuf (gRPC) | 3.33.5 | gRPC client definitions (9 .proto files) | Scrips.Core.csproj:22-30 |
| Microsoft.IdentityModel.JsonWebTokens | 7.7.1 | JWT token handling | Scrips.BaseDbContext.csproj:23 |
| Azure.Identity | 1.17.1 | Azure managed identity | Scrips.BaseDbContext.csproj:14, Scrips.Core.csproj:8 |

## PRODUCTION DEPENDENCIES

### Scrips.Core Project (Core Models & API Clients)

| Package | Version | Purpose |
|---------|---------|---------|
| Azure.Identity | 1.17.1 | Azure managed identity authentication |
| Azure.Search.Documents | 11.6.0 | AI-powered chief complaint search with vector embeddings |
| Newtonsoft.Json | 13.0.4 | JSON serialization for API responses |
| Refit | 8.0.0 | Type-safe HTTP API clients (11 services) |
| Protobuf Definitions | 3.33.5 | 9 gRPC service contracts compiled as clients |

### Scrips.Core.Application Project (Application Layer)

| Package | Version | Purpose |
|---------|---------|---------|
| FluentValidation | 11.10.0 | Request DTO validation |
| Ardalis.Specification | 8.0.0 | Specification pattern for complex queries |
| Mapster | 7.4.0 | High-performance object mapping |
| MediatR | 12.0.1 | CQRS mediator pattern |
| Microsoft.Extensions.Caching.Abstractions | 8.0.0 | Cache service interface (not implemented) |
| Microsoft.Extensions.Configuration.Binder | 8.0.2 | Configuration binding |
| Microsoft.Extensions.Localization | 8.0.10 | Multi-language support |
| Newtonsoft.Json | 13.0.4 | JSON serialization |

### Scrips.BaseDbContext Project (Audit Infrastructure)

| Package | Version | Purpose |
|---------|---------|---------|
| Azure.Identity | 1.17.1 | Azure managed identity authentication |
| Dapr.Client | 1.14.0 | Pub/sub for audit log distribution |
| Google.Protobuf | 3.33.5 | Protocol buffer support |
| Microsoft.Data.SqlClient | 5.2.2 | SQL Server database provider |
| Microsoft.EntityFrameworkCore | 8.0.11 | ORM for database access |
| Microsoft.EntityFrameworkCore.Relational | 8.0.11 | Relational database support |
| Microsoft.Extensions.Configuration.UserSecrets | 8.0.1 | Secure configuration storage |
| Microsoft.IdentityModel.JsonWebTokens | 7.7.1 | JWT token handling |
| Microsoft.IdentityModel.Tokens | 7.7.1 | Token validation |
| System.IdentityModel.Tokens.Jwt | 7.7.1 | JWT token support |
| Newtonsoft.Json | 13.0.4 | JSON serialization for audit logs |
| Serilog | 4.0.2 | Structured logging |
| Finbuckle.MultiTenant.EntityFrameworkCore | 6.10.0 | Multi-tenant data isolation with global filters |

### Scrips.Infrastructure Project (Infrastructure Services)

| Package | Version | Purpose |
|---------|---------|---------|
| Asp.Versioning.Mvc | 8.1.0 | API endpoint versioning |
| Newtonsoft.Json | 13.0.4 | JSON serialization |
| Serilog | 4.0.2 | Structured logging |
| Microsoft.Data.SqlClient | 5.2.2 | SQL Server database provider |

### Scrips.WebApi Project (API Base Controllers)

| Package | Version | Purpose |
|---------|---------|---------|
| MediatR | 12.0.1 | CQRS mediator pattern |
| Asp.Versioning.Mvc | 8.1.0 | API versioning |
| Newtonsoft.Json | 13.0.4 | JSON serialization |

### Scrips.Core.Domain Project (Domain Contracts)

| Package | Version | Purpose |
|---------|---------|---------|
| NewId | 4.0.1 | ID generation |
| Newtonsoft.Json | 13.0.4 | JSON serialization |

## UPGRADE READINESS

### ✅ CRITICAL Updates -- COMPLETED (2026-03)

**1. .NET 7.0 → .NET 8.0 (LTS)** -- **FIXED**
- **Previous:** .NET 7.0 (All 6 projects)
- **Current:** .NET 8.0 LTS (All 6 projects - .csproj:4)
- **Status:** ✅ Upgraded to .NET 8.0 LTS (support until November 2026)

**2. System.Data.SqlClient → Microsoft.Data.SqlClient** -- **FIXED**
- **Previous:** System.Data.SqlClient v4.8.5 (legacy)
- **Current:** Microsoft.Data.SqlClient v5.2.2 consolidated across all projects
- **Status:** ✅ Legacy provider removed

**3. Microsoft.AspNetCore.Http v2.2.2** -- **FIXED**
- **Previous:** Explicit v2.2.2 references
- **Current:** Using framework-provided FrameworkReference with .NET 8.0
- **Status:** ✅ Old explicit references removed

### ✅ PREVIOUSLY RECOMMENDED Updates -- COMPLETED (2026-03)

**4. MediatR** -- Pinned at 12.0.1
- **Current:** 12.0.1
- **Note:** Pinned at 12.0.1 because 12.4.1 breaks IRequestPostProcessor auto-registration on .NET 8. Do NOT upgrade.

**5. FluentValidation** -- **FIXED**
- **Previous:** 11.5.1
- **Current:** 11.10.0
- **Note:** FluentValidation.AspNetCore (deprecated) replaced with FluentValidation + FluentValidation.DependencyInjectionExtensions

**6. Refit** -- **FIXED**
- **Previous:** 6.3.2
- **Current:** 8.0.0
- **Status:** ✅ Upgraded to v8.0.0

**7. Azure.Search.Documents 11.6.0**
- **Status:** ✅ CURRENT - Latest stable version

**8. Dapr.Client** -- **FIXED**
- **Previous:** 1.10.0
- **Current:** 1.14.0
- **Status:** ✅ Upgraded

**9. Newtonsoft.Json** -- **FIXED**
- **Previous:** 13.0.3
- **Current:** 13.0.4
- **Status:** ✅ Updated

**10. Serilog** -- **FIXED**
- **Previous:** 2.12.0
- **Current:** 4.0.2
- **Status:** ✅ Upgraded

### 🟢 UP TO DATE

- Microsoft.Extensions.* (8.0.x): Current for .NET 8.0
- Microsoft.Data.SqlClient: 5.2.2 (Current)
- Mapster: 7.4.0 (Current)
- Ardalis.Specification: 8.0.0 (Current)
- Finbuckle.MultiTenant: 6.10.0 (Pinned - 7.x has breaking changes)
- Google.Protobuf: 3.33.5 (Current)
- Microsoft.IdentityModel.*: 7.7.1 (Pinned - 8.x causes 401 auth failures)
- Asp.Versioning.Mvc: 8.1.0 (Current)

## HEALTHCARE IMPACT ANALYSIS

### Patient Safety

**FIXED (2026-03):**
- **.NET 8.0 LTS:** ✅ Upgraded from .NET 7.0 -- security patches now available through November 2026
- **Legacy SQL Client:** ✅ System.Data.SqlClient removed, consolidated to Microsoft.Data.SqlClient v5.2.2
- **Dapr.Client:** ✅ Upgraded to 1.14.0 for better reliability

**Remaining concerns:**
- **Audit log fire-and-forget pattern:** Still present -- Dapr unavailability can lose audit logs
- **Connection string security:** Still in plain text configuration (not yet moved to Key Vault)

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

2. **Audit Logging:** MaskValueAudit attribute with EF Core 8.0.11
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
| Critical Updates | ✅ Complete | 0 | RESOLVED |
| Up to Date | ✅ Current | 20+ | LOW |
| Pinned (intentional) | ✅ Stable | 3 (MediatR, Finbuckle, IdentityModel) | LOW |
| Missing (Cache) | ⚠️ Not Impl | 1 | MEDIUM |

**Overall Health:** ✅ **GOOD**
- All critical updates completed (2026-03)
- .NET 8.0 LTS, all packages current or intentionally pinned
- Remaining concern: Cache implementation still missing

## UPGRADE ROADMAP

**Phase 1-2 -- COMPLETED (2026-03):**
1. ✅ Upgraded .NET 7.0 → .NET 8.0 LTS (all 6 projects)
2. ✅ Removed System.Data.SqlClient, consolidated to Microsoft.Data.SqlClient v5.2.2
3. ✅ Removed explicit Microsoft.AspNetCore.Http references (using FrameworkReference)
4. ✅ Updated Dapr.Client 1.10.0 → 1.14.0
5. ✅ Updated FluentValidation 11.5.1 → 11.10.0
6. ✅ Updated Serilog 2.12.0 → 4.0.2
7. ✅ Updated Refit 6.3.2 → 8.0.0
8. ✅ Updated Ardalis.Specification 6.1.0 → 8.0.0
9. ✅ Updated Mapster 7.3.0 → 7.4.0
10. ✅ Added Microsoft.IdentityModel.* packages pinned at 7.7.1
11. ✅ Replaced Microsoft.AspNetCore.Mvc.Versioning with Asp.Versioning.Mvc 8.1.0

**Phase 3 (Remaining - Next Quarter):**
12. Implement cache (Redis) with encryption
13. Add Polly retry policies to Refit clients
14. Implement integration tests for multi-tenancy
15. Add E2E tests for critical healthcare workflows

**Estimated Remaining Effort:** 80-120 hours
**Priority:** MEDIUM - Operational improvements
