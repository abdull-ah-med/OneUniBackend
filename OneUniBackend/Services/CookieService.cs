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
        
        response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,  // Lax allows cookies on redirects
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes)
        });

        if(!string.IsNullOrEmpty(refreshToken))
        {
            response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays)
            });
        }
    }
    
    public void ClearAuthCookies()
    {
        var response = _httpContextAccessor.HttpContext!.Response;
        var isProduction = _environment.IsProduction();
        
        response.Cookies.Delete("access_token", new CookieOptions
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = isProduction
        });
        response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = isProduction
        });
    }
}
