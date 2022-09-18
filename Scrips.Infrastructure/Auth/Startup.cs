using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scrips.Core.Application.Common.Interfaces;

namespace Scrips.Infrastructure.Auth;
internal static class Startup
{
    internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
        app.UseMiddleware<CurrentUserMiddleware>();


    internal static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
}
