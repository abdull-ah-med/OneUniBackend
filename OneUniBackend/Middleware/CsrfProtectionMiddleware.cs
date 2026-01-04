using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OneUniBackend.Middleware;

public class CsrfProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CsrfProtectionMiddleware> _logger;
    private static readonly string[] UnsafeMethods = ["POST", "PUT", "PATCH", "DELETE"];
    
    // Endpoints exempt from CSRF validation (they have their own token validation)
    private static readonly string[] CsrfExemptPaths = 
    [
        "/api/google-oauth/complete-signup",  // Validates temporary JWT token
        "/api/auth/register",                  // Public registration
        "/api/auth/login",                     // Public login
        "/api/auth/refresh"                    // Uses refresh token from cookie
    ];

    public CsrfProtectionMiddleware(RequestDelegate next, ILogger<CsrfProtectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        var isExempt = CsrfExemptPaths.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase));

        if (!isExempt && UnsafeMethods.Contains(context.Request.Method.ToUpperInvariant()))
        {
            var cookieToken = context.Request.Cookies["XSRF-TOKEN"];
            var headerToken = context.Request.Headers["X-CSRF-TOKEN"].FirstOrDefault();

            if (string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(headerToken) || !string.Equals(cookieToken, headerToken, StringComparison.Ordinal))
            {
                _logger.LogWarning("CSRF validation failed for path: {Path}", path);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
                    title = "CSRF validation failed",
                    status = StatusCodes.Status400BadRequest,
                    traceId = context.TraceIdentifier
                });
                return;
            }
        }

        await _next(context);
    }
}

