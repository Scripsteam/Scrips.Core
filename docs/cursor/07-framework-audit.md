Refreshed 2026-11-15 from origin/master @ 6fc2554. Previous version dated 2026-03-04 (see git history / backup).
Verified against origin/master @ 6fc2554, 2026-11-15. Versions below are read from the csproj files. No `dotnet list package --outdated` or vulnerability scan was run (machine load; brief forbids builds). Newer-version and CVE statements are therefore [UNVERIFIED].

# Framework Audit

## Runtime framework [UPDATED]

- All six projects: `<TargetFramework>net8.0</TargetFramework>` at line 4 of each csproj. [CONFIRMED] The old text said `net7.0` and listed a ".NET 7 end of support" risk; that risk is gone (see `05-architecture.md` 7.7, [REMOVED]).
- `ImplicitUsings` and `Nullable` are enabled in Domain, Application, BaseDbContext, Infrastructure and WebApi. `Scrips.Core.csproj` does not enable either. [CONFIRMED: csproj read] Several `Scrips.Core` files use `#nullable enable` or `string?` themselves. [CONFIRMED in samples]
- .NET 8 is a long-term-support release. Microsoft's published end-of-support date for .NET 8 is in November 2026, which is now. [UNVERIFIED this session: the date is from memory of Microsoft's lifecycle page] [NEEDS HUMAN REVIEW: confirm the date and plan the move to .NET 10; consumers must move together because Core is a source dependency]

## Web framework [UPDATED]

- ASP.NET Core via `FrameworkReference Include="Microsoft.AspNetCore.App"` in `Scrips.BaseDbContext` and `Scrips.Core.Application`. [CONFIRMED]
- `Asp.Versioning.Mvc` 8.1.0 in Infrastructure and WebApi. API version default 1.0. [CONFIRMED: `Scrips.Infrastructure/Startup.cs:60-70`]
- MediatR 12.0.1 in Application and WebApi. [CONFIRMED]
- `Scrips.WebApi/VersionedApiController.cs:1` imports `Microsoft.AspNetCore.Components` for `[Route]`. `Microsoft.AspNetCore.Mvc.RouteAttribute` is the usual one; `Microsoft.AspNetCore.Components.RouteAttribute` is the Blazor attribute. [CONFIRMED import; INFERRED that the Blazor attribute is being applied; its effect on MVC routing was not tested] [NEEDS HUMAN REVIEW]

## Data access [UPDATED]

- EF Core 8.0.11 (`Microsoft.EntityFrameworkCore`, `.Relational`), `Microsoft.Data.SqlClient` 5.2.2. [CONFIRMED]
- Finbuckle.MultiTenant.EntityFrameworkCore 6.10.0. [CONFIRMED]
- Ardalis.Specification 8.0.0 (reads only; `IRepository<T>` extends `IRepositoryBase<T>` and has no implementation here). [CONFIRMED]
- Dapper: referenced by name in `IDapperRepository` only; no Dapper package. [CONFIRMED: no Dapper in any csproj]

## Testing [UPDATED]

No test project, no test package, no CI workflow in this repo. [CONFIRMED: `git ls-files`] Consumers carry tests (for example Scrips.Auditing has `UnitTests/Explain/AuditDeliveryTests.cs`). The outbox code has no test in this repo. [CONFIRMED absence]

## Packages by project [UPDATED]

### Scrips.Core [UPDATED]
Azure.Identity 1.17.1, Azure.Search.Documents 11.6.0, Microsoft.Extensions.Primitives 10.0.2, Newtonsoft.Json 13.0.4, System.Diagnostics.DiagnosticSource 10.0.2, System.IO.Pipelines 8.0.0, System.Security.Cryptography.ProtectedData 9.0.6, System.Text.Encodings.Web 8.0.0, System.Text.Json 8.0.5, System.Threading.Channels 8.0.0, Refit 8.0.0. [CONFIRMED: `Scrips.Core.csproj:9-21`]
No `Google.Protobuf`, `Grpc.Tools` or `Grpc.Net.Client` entry although the project lists nine `<Protobuf>` items. How a standalone build gets them is not visible. [CONFIRMED absence in csproj; NOT VERIFIED by build]

### Scrips.Core.Domain [UPDATED]
NewId 4.0.1, Newtonsoft.Json 13.0.4, System.Diagnostics.DiagnosticSource 10.0.2; project reference to Scrips.Core. [CONFIRMED]

### Scrips.Core.Application [UPDATED]
FluentValidation 11.10.0, Ardalis.Specification 8.0.0, Mapster 7.4.0, MediatR 12.0.1, Microsoft.Extensions.Caching.Abstractions 8.0.0, Microsoft.Extensions.Configuration.Binder 10.0.2, Microsoft.Extensions.Localization 8.0.10, Newtonsoft.Json 13.0.4, System.Diagnostics.DiagnosticSource 10.0.2; references Domain and Core. [CONFIRMED]

### Scrips.BaseDbContext [UPDATED]
Azure.Identity 1.17.1, Dapr.Client 1.17.0, Google.Protobuf 3.33.5, Microsoft.CSharp 4.7.0, Microsoft.Data.SqlClient 5.2.2, EF Core and Relational 8.0.11, 13 `Microsoft.Extensions.*` packages at 10.0.2 (Configuration, Configuration.Abstractions, Configuration.Binder, DependencyInjection, DependencyInjection.Abstractions, Diagnostics, Diagnostics.Abstractions, Http, Logging, Logging.Abstractions, Options, Options.ConfigurationExtensions, Primitives) plus `Configuration.UserSecrets` 8.0.1, Microsoft.IdentityModel.JsonWebTokens, Microsoft.IdentityModel.Tokens and System.IdentityModel.Tokens.Jwt at 7.7.1, Newtonsoft.Json 13.0.4, Serilog 4.0.2, Finbuckle 6.10.0, System.Diagnostics.DiagnosticSource 10.0.2, System.IO.Pipelines 8.0.0, System.Memory 4.5.5, System.Runtime 4.3.1, System.Security.Cryptography.ProtectedData 9.0.6, System.Text.Json 8.0.5, System.Threading.Channels 8.0.0; `NoWarn NU1605`; references Core. [CONFIRMED: `Scrips.BaseDbContext.csproj:1-56`]

### Scrips.Infrastructure [UPDATED]
Asp.Versioning.Mvc 8.1.0, Newtonsoft.Json 13.0.4, Serilog 4.0.2, Microsoft.Data.SqlClient 5.2.2, System.Diagnostics.DiagnosticSource 10.0.2; references Application. [CONFIRMED]

### Scrips.WebApi [UPDATED]
MediatR 12.0.1, Asp.Versioning.Mvc 8.1.0, Newtonsoft.Json 13.0.4, System.Diagnostics.DiagnosticSource 10.0.2; references Infrastructure. [CONFIRMED]

## Upgrade readiness [UPDATED]

### Completed since March [UPDATED]
- Dapr.Client 1.14.0 to 1.17.0 (commit 852a221, 2026-03-09), `Microsoft.Extensions.*` pinned at 10.0.2 (99ad97d), three System packages bumped to 10.0.2 (d0dbf45) and then reverted to 8.x (598a2f4), `Configuration.UserSecrets` held at 8.0.1 (c22643c). [CONFIRMED by `git log` on the csproj and `git diff 4d8e816 origin/master -- '*.csproj'`]
- `NoWarn NU1605` added in Core and BaseDbContext. [CONFIRMED]

### Constraints a maintainer must know [NEW]
- The 10.0.2 and 8.x mixes are deliberate. Comments say `System.IO.Pipelines`, `System.Text.Json` and `System.Threading.Channels` must stay on 8.x because 10.x breaks Kestrel `PipeWriter.Advance()` in gRPC. (`Scrips.BaseDbContext.csproj:45`; `Scrips.Core.csproj:5`) [CONFIRMED comments; not reproduced]
- EF Core 8.0.11 sits next to `Microsoft.Extensions.*` 10.0.2. Raising EF Core is a separate step. [INFERRED]
- Every consumer builds Core from source, so a package bump here changes every service's build. [CONFIRMED mechanism: submodule]

### Not assessed [NEW]
Security advisories for each package; newer versions; license review. [UNVERIFIED]

## Healthcare impact [UPDATED]
Changing the audit classes changes the audit trail of every service that inherits them. Any change to `AuditLoggingHelper`, to the outbox, or to the `SaveAudit` wire format needs a consumer review (Scrips.Auditing at minimum). [INFERRED from the design; consumer review not performed]
