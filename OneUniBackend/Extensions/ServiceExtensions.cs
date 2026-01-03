using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneUniBackend.Configuration;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Services;
using Microsoft.Extensions.Options;

namespace OneUniBackend.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JWTSettings>(config.GetSection("JwtSettings"));
        services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

        services.AddHttpContextAccessor();

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();

        services.AddSingleton<Cloudinary>(sp =>
        {
            var cloudinarySettings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
            var account = new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret);
            return new Cloudinary(account);
        });

        services.AddScoped<IStorageService, CloudinaryStorageService>();
        services.AddScoped<IOneProfileService, OneProfileService>();
    }
}
