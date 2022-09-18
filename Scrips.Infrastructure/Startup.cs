using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrips.Infrastructure.Auth;

namespace Scrips.Infrastructure;
public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        return services

            // .AddApiVersioning()
            .AddCurrentUser()

            // .AddBackgroundJobs(config)
            // .AddCaching(config)
            // .AddCorsPolicy(config)
            // .AddExceptionMiddleware()
            // .AddHealthCheck()
            // .AddPOLocalization(config)
            // .AddMailing(config)
            // .AddMediatR(Assembly.GetExecutingAssembly())
            // .AddMultitenancy(config)
            // .AddNotifications(config)
            // .AddOpenApiDocumentation(config)
            // .AddPersistence(config)
            // .AddRequestLogging(config)
            // .AddRouting(options => options.LowercaseUrls = true)
            // .AddServices()
            ;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder) =>
            builder

                // .UseRequestLocalization()
                // .UseStaticFiles()
                // .UseSecurityHeaders(config)
                // .UseFileStorage()
                // .UseExceptionMiddleware()
                // .UseRouting()
                // .UseCorsPolicy()
                // .UseAuthentication()
                .UseCurrentUser()

                // .UseMultiTenancy()
                // .UseAuthorization()
                // .UseRequestLogging(config)
                // .UseHangfireDashboard(config)
                // .UseOpenApiDocumentation(config)
                ;
}