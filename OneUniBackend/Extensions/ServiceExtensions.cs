using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneUniBackend.Configuration;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Services;

namespace OneUniBackend.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JWTSettings>(config.GetSection("JwtSettings"));

        services.AddHttpContextAccessor();

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();

    }
}
