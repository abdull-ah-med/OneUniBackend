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

    public CookieService(IHttpContextAccessor httpContextAccessor, IOptions<JWTSettings> jwtSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings.Value;
    }
    public void SetAuthCookies(string accessToken, string? refreshToken)
    {
        var response = _httpContextAccessor.HttpContext!.Response;
        response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api",
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes)
        });

        if(!string.IsNullOrEmpty(refreshToken))
        {
            response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Secure = true,
                Path = "/api/auth/refresh",
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays)
            });
        }
    }
    public void ClearAuthCookies()
    {
        var response = _httpContextAccessor.HttpContext!.Response;
        response.Cookies.Delete("access_token", new CookieOptions
        {
            Path = "/api",
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            Secure = true
        });
        response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/api/auth/refresh",
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            Secure = true
        });
    }
}
