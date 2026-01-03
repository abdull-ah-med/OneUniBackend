using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OneUniBackend.Extensions;

public static class AuthExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var key = Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!);

        var tokenValidation = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["JwtSettings:Issuer"],

            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        services.AddSingleton(tokenValidation);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidation;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Prefer Authorization header; fall back to cookie for browser flows.
                        if (string.IsNullOrWhiteSpace(context.Token))
                        {
                            context.Token = context.Request.Cookies["access_token"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void AddGoogleJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication()
            .AddJwtBearer("Google", options =>
            {
            options.Authority = "https://accounts.google.com";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://accounts.google.com",

                ValidateAudience = true,
                ValidAudience = config["GoogleAuth:ClientId"],

                ValidateLifetime = true
            };
        });
    }
}
