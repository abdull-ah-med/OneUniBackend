using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OneUniBackend.Extensions;

public static class ConfigurationExtensions
{
    public static void ConfigureEnvironmentVariables(this IServiceCollection services, IConfiguration config)
    {
        config["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET") 
            ?? throw new InvalidOperationException("JWT_SECRET not configured");

        config["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "OneUni";
        config["JwtSettings:AccessTokenExpiresInMinutes"] = Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES") ?? "60";
        config["JwtSettings:RefreshTokenExpiresInDays"] = Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRES_IN_DAYS") ?? "7";

        config["GoogleAuth:ClientId"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
            ?? "TEST_GOOGLE_CLIENT_ID";

        config["GoogleAuth:ClientSecret"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")
            ?? "TEST_GOOGLE_CLIENT_SECRET";

        config["GoogleAuth:RedirectUri"] = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI")
            ?? "http://localhost:5000/api/auth/google/callback";
    }
}
