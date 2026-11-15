Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. The old file (305 lines) was replaced by this shorter file.

# Section 6: Identity and Tenancy

## 1. Scope [UPDATED]

This repo defines the claim vocabulary, a user-context helper, tenancy constants, a small role and permission list, and the Identity wire contracts. Authentication, token issue, role storage and tenant resolution are NOT here. The owning service is Scrips.Identity. [CONFIRMED: no JWT validation, no `AddAuthentication`, no tenant strategy in this repo; `git grep -n "AddAuthentication\|AddMultiTenant"` returns nothing]

## 2. Claims and helpers [UPDATED]

| Item | Value | Evidence |
|---|---|---|
| Claim names (`ScripsClaims`) | `tenant`, `fullName`, `permission`, `image_url`, `ipAddress`, `exp`, `orgRoles`, `orgId` | `Shared/Authorization/ScripsClaims.cs:5-12` |
| Tenant name constant | `MultitenancyConstants.TenantIdName = "tenant"` | `Shared/Multitenancy/MultitenancyConstants.cs:5` |
| Roles | `Admin`, `Basic`; `IsDefault(name)` | `Shared/Authorization/ScripsRoles.cs` |
| Permission name format | `Permissions.{Resource}.{Action}` | `Shared/Authorization/ScripsPermissions.cs:37` |
| Permissions defined | 3, all root: View, Create, Update on resource `Tenants`. `ScripsAction` also defines Search and Delete, which no permission uses. | `ScripsPermissions.cs:5-26` |
| `ClaimsPrincipal` extensions | `GetEmail` (ClaimTypes.Email), `GetTenant`, `GetFullName`, `GetFirstName`, `GetSurname`, `GetPhoneNumber`, `GetUserId` (claim `sub`), `GetImageUrl`, `GetExpiration` | `ClaimsPrincipalExtensions.cs:7-33` |

The claims `orgId` and `orgRoles` exist as constants, but this repo has no helper that reads them. [CONFIRMED by `git grep -n "OrganizationId\b\|OrganizationRoles" -- Scrips.Core/Shared`] The proto comments in `PatientManagement.proto` say the organization comes from "the token's orgId claim" or the `organizationid` metadata header. [CONFIRMED: `PatientManagement.proto:92, 1266-1267`]

## 3. CurrentUser [UPDATED]

- `ICurrentUser` (`Scrips.Core.Application/Common/Interfaces/ICurrentUser.cs`): `Name`, `GetUserId`, `GetUserEmail`, `GetTenant`, `IsAuthenticated`, `IsInRole`, `GetUserClaims`, `IsSSA`.
- `CurrentUser` (`Scrips.Infrastructure/Auth/CurrentUser.cs`): `IsSSA()` is true when the user has claim `sa` equal to `1` (`:34`). `SetCurrentUser` and `SetCurrentUserId` throw `Exception("Method reserved for in-scope initialization")` on a second call (`:39-60`). `GetUserId()` returns `Guid.Parse(...)` of the `sub` claim when authenticated and throws `FormatException` if `sub` is not a GUID. [CONFIRMED: `:15-18`] (The audit helper handles a non-GUID `sub` safely; `CurrentUser` does not.)
- `CurrentUserMiddleware` calls `SetCurrentUser(context.User)` on every request. [CONFIRMED: `CurrentUserMiddleware.cs:12-17`]
- Registration: `AddCurrentUser()` registers `CurrentUserMiddleware`, `ICurrentUser` and `ICurrentUserInitializer` as scoped. [CONFIRMED: `Auth/Startup.cs:11-15`]
- The middleware order that `UseInfrastructure()` applies is only `UseCurrentUser()`. Authentication, routing, authorization and multi-tenancy are commented out. [CONFIRMED: `Scrips.Infrastructure/Startup.cs:40-58`] `CurrentUser` therefore sees an authenticated principal only if the consumer ran `UseAuthentication()` earlier. [INFERRED]

## 4. Multi-tenancy [UPDATED]

- `AuditableMultiTenantBaseDbContext` extends Finbuckle `MultiTenantDbContext` and takes an `ITenantInfo` in its constructor. It stores `tenantInfo?.Id` and writes it to outbox rows. [CONFIRMED: `AuditableMultiTenantBaseDbContext.cs:12-33, 49`]
- Finbuckle applies a query filter only to entities that the consumer marks as multi-tenant (`[MultiTenant]` or `IMultiTenant`). No entity in this repo is marked. [CONFIRMED: `git grep -n "MultiTenant" -- Scrips.Core.Domain` returns nothing] The old text implied isolation comes "for free"; it holds only for consumer entities that opt in. [INFERRED from Finbuckle's design; consumer entities not checked]
- The audit helper reads the tenant from claim `tenant` of `HttpContext.User`, not from `ITenantInfo` (`AuditLoggingHelper.cs:21`). The outbox row's `TenantId` comes from `ITenantInfo`. These two sources can differ in a background job with no `HttpContext`; then the per-entity tenant in the payload is null. [CONFIRMED by reading both; the null case is INFERRED]
- Slot-search requests carry an explicit `OrganizationId` (`Models/Scheduling/SlotsRequest.cs:51`). [CONFIRMED]

## 5. gRPC `IdentityManagement` [UPDATED]

21 rpcs, same list as March. [CONFIRMED: rpc-name diff against 4d8e816 is empty] Message changes: `TenantMasterId` was added to the `GetTenantRoles` row (commit 47cd711, PROD-2080). [CONFIRMED by commit title]

`SignUpUser`, `LoginByCode`, `GetUserAccesCodeByEmail` (sic), `LogOutUserByUserId`, `CreateRolesPermissionForOrganization` and device-token rpcs are part of the contract. `GetUserDetails...` responses carry a `passwordResetToken` field (`IdentityManagement.proto:199`). [CONFIRMED: field exists; whether it is ever filled is not verified] [NEEDS HUMAN REVIEW]

Refit `IIdentityApi` has 2 routes: `POST /api/v1/Users/ContactDetails` and `GET /api/Tenants/{tenantId}`. Both take an `Authorization` header value as a plain string parameter. [CONFIRMED: `IIdentityApi.cs:8-12`]

Models: `TenantDto` (Id, Name, AdminEmail, IsActive, ValidUpto), `EmiratesIdCardData` (id number, names, dates, nationality, passport number, photo and signature bytes), `ContactDto`, `GetUserDetailsRequest`. [CONFIRMED] `EmiratesIdCardData` holds government-ID data and biometric images. [NEEDS HUMAN REVIEW for handling rules]

## 6. Gaps [NEW]

- No `IMultiTenant` entity, tenant resolution strategy or authorization policy in this repo; consumers carry them. [CONFIRMED]
- The `Scrips.Core.Domain` project has no tenant field on `BaseEntity` or `AuditableEntity`. [CONFIRMED: `BaseEntity.cs`, `AuditableEntity.cs`]
