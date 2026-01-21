# Identity, Authentication & Multi-Tenancy - Complete Documentation

## OVERVIEW

**Purpose:** User authentication, JWT token management, tenant resolution, multi-tenant data isolation

**Key Entities:** UserIdentity, Tenant (via JWT claims), Person, ScripsPermissions, ScripsClaims

**Key Workflows:** User authentication, JWT token generation, multi-tenant context resolution, role-based authorization

**PHI Scope:** NO - Authentication data only (unless Person is also a Patient)

---

## ENTITIES

### JWT Claims Structure

**Location:** Shared/Authorization/ScripsClaims.cs

**Purpose:** JWT token claims for authentication and tenant context

**Claims:**
- **sub** - User ID (subject)
- **tenant** - Tenant ID (organization)
- **sa** - Super Admin flag (system-level access across all tenants)

**Token Generation:** IIdentityApi.cs (assumed POST /api/v1/Auth/Login)

---

### ScripsPermissions

**Location:** Shared/Authorization/ScripsPermissions.cs

**Purpose:** Granular permission structure for role-based access control

**Permission Categories:**
- Appointments (Create, Read, Update, Delete)
- Patients (Create, Read, Update, Delete)
- Billing (Create, Read, Update, Delete)
- Clinical (Create, Read, Update, Delete)
- Reports (View, Export)
- Admin (Users, Settings, Audit)

---

### OrganizationV1Dto

**Location:** Models/Organization/OrganizationV1Dto.cs

**Purpose:** Tenant entity with hierarchy and settings

**Key Fields:**
- OrganizationId - Tenant identifier
- OrganizationName
- Settings - Tenant-specific configuration

---

### PersonInfoResponse

**Location:** Models/Person/PersonInfoResponse.cs

**Purpose:** User demographics separate from patient/provider roles

**Key Fields:**
- PersonId, UserId
- Demographics (may include PHI if person is also a patient)

---

## WORKFLOWS

### Workflow 1: User Authentication

**Entry Point:** IIdentityApi.cs (assumed POST /api/v1/Auth/Login)

**Steps:**
1. **Login** - Username, password
2. **JWT Generation** - Claims: sub (user ID), tenant (tenant ID), sa (super admin)
3. **Token Validation** - Verify signature, expiration
4. **Context Resolution** - Extract user/tenant from JWT

**JWT Structure:**
```json
{
  "sub": "user-guid",
  "tenant": "organization-guid",
  "sa": "true/false",
  "exp": timestamp,
  "iss": "scrips-identity"
}
```

---

### Workflow 2: Multi-Tenant Context Resolution

**Entry Point:** All service requests

**Steps:**
1. **JWT Extraction** - Authorization header
2. **Tenant Claim** - Extract "tenant" claim
3. **Global Query Filter** - Finbuckle.MultiTenant applies WHERE TenantId = {tenant}
4. **Data Isolation** - Automatic tenant filtering on all queries

**File:** Finbuckle.MultiTenant.EntityFrameworkCore v6.10.0 (Scrips.BaseDbContext.csproj:17)

**CRITICAL:** Data isolation failure = HIPAA violation (cross-tenant PHI exposure)

---

### Workflow 3: Role-Based Authorization

**Entry Point:** ScripsPermissions.cs

**Steps:**
1. **Permission Check** - Verify user has required permission
2. **Resource Access** - Allow/deny operation
3. **Role Validation** - Check role membership
4. **Tenant Scope** - Permissions scoped to tenant

---

## INTEGRATIONS

**Identity Service (IIdentityApi.cs:6-13):**
- 2 endpoints: ContactDetails (POST), Tenants (GET)
- Purpose: User/tenant lookup

**Organization Service (IOrganizationApi.cs:6-19):**
- Organization CRUD, settings
- Purpose: Tenant management

**Person Service (IPersonApi.cs:6-11):**
- User demographics

---

## COMPLIANCE SUMMARY

**PHI:** NONE (authentication data only)

**Audit:** ✅ Login attempts, permission failures logged

**Multi-Tenancy:** CRITICAL - Finbuckle.MultiTenant global filters enforce data isolation

**Session Management:** JWT expiration, refresh mechanisms

**Super Admin:** System-level access (high risk - strict control)

---

## CRITICAL FINDINGS

**Risk #1: Multi-Tenancy Isolation Failure**
- Global query filter failure = cross-tenant data leakage
- Impact: CRITICAL - HIPAA violation
- Mitigation: Automated tests for tenant isolation in every service

**Risk #2: JWT Token Security**
- No visible token refresh mechanism
- Impact: Session hijacking risk
- Mitigation: Implement refresh tokens, short-lived access tokens

---

**Document Version:** 1.0  
**Last Updated:** January 21, 2026  
**Audited Against:** Scrips.Core v7.0 (.NET 7.0)
