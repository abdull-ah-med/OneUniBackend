using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OneUniBackend.Extensions;

public static class ConfigurationExtensions
{
    public static void ConfigureEnvironmentVariables(this IServiceCollection services, IConfiguration config)
    {
        // JWT Settings
        config["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET") 
            ?? throw new InvalidOperationException("JWT_SECRET environment variable is not configured");

        config["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "OneUni";
        config["JwtSettings:AccessTokenExpiresInMinutes"] = Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES") ?? "60";
        config["JwtSettings:RefreshTokenExpiresInDays"] = Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRES_IN_DAYS") ?? "7";

        // Google OAuth Settings
        config["GoogleAuth:ClientId"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
            ?? throw new InvalidOperationException("GOOGLE_CLIENT_ID environment variable is not configured");

        config["GoogleAuth:ClientSecret"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")
            ?? throw new InvalidOperationException("GOOGLE_CLIENT_SECRET environment variable is not configured");

        config["GoogleAuth:RedirectUri"] = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI")
            ?? throw new InvalidOperationException("GOOGLE_REDIRECT_URI environment variable is not configured");

        // Frontend URLs
        config["FrontendUrl"] = Environment.GetEnvironmentVariable("FRONTEND_URL");
        config["FrontendBaseUrl"] = Environment.GetEnvironmentVariable("FRONTEND_BASE_URL")
            ?? throw new InvalidOperationException("FRONTEND_BASE_URL environment variable is not configured");

        // CORS Settings
        config["AllowedOrigins"] = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")
            ?? throw new InvalidOperationException("ALLOWED_ORIGINS environment variable is not configured");

        // Cookie Settings
        config["CookieDomain"] = Environment.GetEnvironmentVariable("COOKIE_DOMAIN");
        config["CookieSameSite"] = Environment.GetEnvironmentVariable("COOKIE_SAMESITE") ?? "Lax";
        config["CookieSecure"] = Environment.GetEnvironmentVariable("COOKIE_SECURE") ?? "auto";

        // Database Settings
        config["Database:Host"] = Environment.GetEnvironmentVariable("DB_HOST")
            ?? throw new InvalidOperationException("DB_HOST environment variable is not configured");
        config["Database:Port"] = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        config["Database:Name"] = Environment.GetEnvironmentVariable("DB_NAME")
            ?? throw new InvalidOperationException("DB_NAME environment variable is not configured");
        config["Database:User"] = Environment.GetEnvironmentVariable("DB_USER")
            ?? throw new InvalidOperationException("DB_USER environment variable is not configured");
        config["Database:Password"] = Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException("DB_PASSWORD environment variable is not configured");
    }
}
