using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrips.Core.Application.Common.Persistence;
using Scrips.Infrastructure.Persistence.ConnectionString;
using Serilog;

namespace Scrips.Infrastructure.Persistence;

internal static class Startup
{
    private static readonly ILogger _logger = Log.ForContext(typeof(Startup));

    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        // TODO: there must be a cleaner way to do IOptions validation...
        var databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        string? rootConnectionString = databaseSettings.ConnectionString;
        if (string.IsNullOrEmpty(rootConnectionString))
        {
            throw new InvalidOperationException("DB ConnectionString is not configured.");
        }

        const string dbProvider = "mssql";

        _logger.Information($"Current DB Provider : {dbProvider}");

        return services

            // .Configure<DatabaseSettings>(config.GetSection(nameof(DatabaseSettings)))
            // .AddDbContext<ApplicationDbContext>(m => m.UseDatabase(dbProvider, rootConnectionString))
            // .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
            // .AddTransient<ApplicationDbInitializer>()
            // .AddTransient<ApplicationDbSeeder>()
            // .AddServices(typeof(ICustomSeeder), ServiceLifetime.Transient)
            // .AddTransient<CustomSeederRunner>()
            .AddTransient<IConnectionStringSecurer, ConnectionStringSecurer>()
            .AddTransient<IConnectionStringValidator, ConnectionStringValidator>();

            // .AddRepositories();
    }

/*
    internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                return builder.UseNpgsql(connectionString, e =>
                     e.MigrationsAssembly("Migrators.PostgreSQL"));

            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(connectionString, e =>
                     e.MigrationsAssembly("Migrators.MSSQL"));

            case DbProviderKeys.MySql:
                return builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), e =>
                     e.MigrationsAssembly("Migrators.MySQL")
                      .SchemaBehavior(MySqlSchemaBehavior.Ignore));

            case DbProviderKeys.Oracle:
                return builder.UseOracle(connectionString, e =>
                     e.MigrationsAssembly("Migrators.Oracle"));

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Add Repositories
        services.AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>));

        foreach (var aggregateRootType in
            typeof(IAggregateRoot).Assembly.GetExportedTypes()
                .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                .ToList())
        {
            // Add ReadRepositories.
            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));

            // Decorate the repositories with EventAddingRepositoryDecorators and expose them as IRepositoryWithEvents.
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), sp =>
                Activator.CreateInstance(
                    typeof(EventAddingRepositoryDecorator<>).MakeGenericType(aggregateRootType),
                    sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)))
                ?? throw new InvalidOperationException($"Couldn't create EventAddingRepositoryDecorator for aggregateRootType {aggregateRootType.Name}"));
        }

        return services;
    }
    */
}