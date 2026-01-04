using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OneUniBackend.Data;

namespace OneUniBackend.Extensions;

public static class DatabaseExtensions
{
    public static void AddConfiguredDatabase(this IServiceCollection services, IConfiguration config)
    {
        var dbHost = config["Database:Host"] 
            ?? throw new InvalidOperationException("Database:Host is not configured");
        var dbPort = config["Database:Port"] ?? "5432";
        var dbName = config["Database:Name"] 
            ?? throw new InvalidOperationException("Database:Name is not configured");
        var dbUser = config["Database:User"] 
            ?? throw new InvalidOperationException("Database:User is not configured");
        var dbPassword = config["Database:Password"] 
            ?? throw new InvalidOperationException("Database:Password is not configured");

        var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        // Map enums
        EnumMappings.Map(dataSourceBuilder);

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<OneUniDbContext>(options =>
            options.UseNpgsql(dataSource, o => EnumMappings.MapEnumsforEF(o))
                   .UseSnakeCaseNamingConvention());
    }
}
