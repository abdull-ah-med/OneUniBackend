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

    public CookieService(IHttpContextAccessor httpContextAccessor, IOptions<JWTSettings> jwtSettings, IWebHostEnvironment environment)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings.Value;
        _environment = environment;
    }
    
    public void SetAuthCookies(string accessToken, string? refreshToken)
    {
        var response = _httpContextAccessor.HttpContext!.Response;
        var isProduction = _environment.IsProduction();
        var cookieDomain = Environment.GetEnvironmentVariable("COOKIE_DOMAIN");
        var csrfToken = Guid.NewGuid().ToString("N");
        
        response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,  // Lax allows cookies on redirects
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });

        if(!string.IsNullOrEmpty(refreshToken))
        {
            response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays),
                Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
            });
        }

        // CSRF double-submit token (non-HttpOnly)
        response.Cookies.Append("XSRF-TOKEN", csrfToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
    }
    
    public void ClearAuthCookies()
    {
        var response = _httpContextAccessor.HttpContext!.Response;
        var isProduction = _environment.IsProduction();
        var cookieDomain = Environment.GetEnvironmentVariable("COOKIE_DOMAIN");
        
        response.Cookies.Delete("access_token", new CookieOptions
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = isProduction,
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
        response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = isProduction,
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
        response.Cookies.Delete("XSRF-TOKEN", new CookieOptions
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = false,
            Secure = isProduction,
            Domain = string.IsNullOrWhiteSpace(cookieDomain) ? null : cookieDomain
        });
    }
}
