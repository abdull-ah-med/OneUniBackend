using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OneUniBackend.Extensions;

public static class CorsExtensions
{
    public static void AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOriginsConfig = configuration["AllowedOrigins"]
            ?? throw new InvalidOperationException("AllowedOrigins is not configured. Ensure ALLOWED_ORIGINS environment variable is set.");
        
        var allowedOrigins = allowedOriginsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowCredentials", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });
    }
}
