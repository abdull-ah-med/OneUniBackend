using System;
using OneUniBackend.Configuration;
using OneUniBackend.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace OneUniBackend.Services;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JWTSettings _jwtSettings;
    private readonly IWebHostEnvironment _environment;
    private readonly string? _cookieDomain;
    private readonly SameSiteMode _sameSiteMode;
    private readonly string _secureMode;

    public CookieService(IHttpContextAccessor httpContextAccessor, IOptions<JWTSettings> jwtSettings, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings.Value;
        _environment = environment;
        _cookieDomain = configuration["CookieDomain"];
        _secureMode = configuration["CookieSecure"] ?? "auto";
        
        // Parse SameSite mode from config
        var sameSiteConfig = configuration["CookieSameSite"] ?? "Lax";
        _sameSiteMode = sameSiteConfig.ToLowerInvariant() switch
        {
            "none" => SameSiteMode.None,
            "strict" => SameSiteMode.Strict,
            "lax" => SameSiteMode.Lax,
            _ => SameSiteMode.Lax
        };
    }

    private bool IsSecure()
    {
        return _secureMode.ToLowerInvariant() switch
        {
            "true" => true,
            "false" => false,
            _ => _environment.IsProduction() // "auto" - use production check
        };
    }
    
    public void SetAuthCookies(string accessToken, string? refreshToken)
    {
        var response = _httpContextAccessor.HttpContext!.Response;
        var isSecure = IsSecure();
        var cookieDomain = _cookieDomain;
        var csrfToken = Guid.NewGuid().ToString("N");
        
        response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = _sameSiteMode,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });

        if(!string.IsNullOrEmpty(refreshToken))
        {
            response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isSecure,
                SameSite = _sameSiteMode,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays),
                Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
            });
        }

        // CSRF double-submit token (non-HttpOnly)
        response.Cookies.Append("XSRF-TOKEN", csrfToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = isSecure,
            SameSite = _sameSiteMode,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
    }
    
    public void ClearAuthCookies()
    {
        var response = _httpContextAccessor.HttpContext!.Response;
        var isSecure = IsSecure();
        var cookieDomain = _cookieDomain;
        
        response.Cookies.Delete("access_token", new CookieOptions
        {
            Path = "/",
            SameSite = _sameSiteMode,
            HttpOnly = true,
            Secure = isSecure,
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
        response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/",
            SameSite = _sameSiteMode,
            HttpOnly = true,
            Secure = isSecure,
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
        response.Cookies.Delete("XSRF-TOKEN", new CookieOptions
        {
            Path = "/",
            SameSite = _sameSiteMode,
            HttpOnly = false,
            Secure = isSecure,
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
    }
}
