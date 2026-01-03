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

    public CsrfProtectionMiddleware(RequestDelegate next, ILogger<CsrfProtectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (UnsafeMethods.Contains(context.Request.Method.ToUpperInvariant()))
        {
            var cookieToken = context.Request.Cookies["XSRF-TOKEN"];
            var headerToken = context.Request.Headers["X-CSRF-TOKEN"].FirstOrDefault();

            if (string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(headerToken) || !string.Equals(cookieToken, headerToken, StringComparison.Ordinal))
            {
                _logger.LogWarning("CSRF validation failed");
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

