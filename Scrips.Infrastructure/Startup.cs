using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrips.Infrastructure.Auth;
using Scrips.Infrastructure.Persistence;

namespace Scrips.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        return services
                .AddApiVersioning()
                .AddCurrentUser()

                // .AddBackgroundJobs(config)
                // .AddCaching(config)
                // .AddCorsPolicy(config)
                // .AddExceptionMiddleware()
                // .AddHealthCheck()
                .AddLocalization()

                // .AddMailing(config)
                .AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()); })

                // .AddMultitenancy(config)
                // .AddNotifications(config)
                // .AddOpenApiDocumentation(config)
                .AddPersistence(config)

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

    private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
}