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
        var connectionString =
            $"Host={Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"};" +
            $"Port={Environment.GetEnvironmentVariable("DB_PORT") ?? "5432"};" +
            $"Database={Environment.GetEnvironmentVariable("DB_NAME") ?? "oneuni"};" +
            $"Username={Environment.GetEnvironmentVariable("DB_USER") ?? "postgres"};" +
            $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres"}";

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        // Map enums
        EnumMappings.Map(dataSourceBuilder);

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<OneUniDbContext>(options =>
            options.UseNpgsql(dataSource, o => EnumMappings.MapEnumsforEF(o))
                   .UseSnakeCaseNamingConvention());
    }
}
