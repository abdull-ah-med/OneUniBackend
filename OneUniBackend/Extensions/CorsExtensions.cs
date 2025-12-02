using Microsoft.Extensions.DependencyInjection;

namespace OneUniBackend.Extensions;

public static class CorsExtensions
{
    public static void AddConfiguredCors(this IServiceCollection services)
    {
        var allowedOrigins = 
            Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') 
            ?? new[] { "http://localhost:3000" };

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
