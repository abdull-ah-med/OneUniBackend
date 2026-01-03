using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneUniBackend.Configuration;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.DTOs.Common;
using OneUniBackend.DTOs.User;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Utils;
using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.Auth.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly JWTSettings _jwtSettings;
    private readonly ICookieService _cookieService;

    public AuthController(IAuthService authService, ILogger<AuthController> logger, IOptions<JWTSettings> jwtSettings, ICookieService cookieService)
    {
        _authService = authService;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
        _cookieService = cookieService;
    }

    
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] SignUpRequestDTO request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ErrorResponseDTO.FromErrors(errors, HttpContext.TraceIdentifier));
            }

            var result = await _authService.RegisterAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User registration failed: {ErrorMessage}", result.ErrorMessage);

                return result.ErrorMessage switch
                {
                    "USER_ALREADY_EXISTS" => Conflict(ErrorResponseDTO.FromMessage(
                        "A user with this email already exists.",
                        HttpContext.TraceIdentifier)),
                    "USER_REGISTRATION_FAILED" => StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ErrorResponseDTO.FromMessage(result.ErrorMessage,
                            HttpContext.TraceIdentifier)),
                    _ => BadRequest(ErrorResponseDTO.FromMessage(
                        result.ErrorMessage ?? "Registration failed.",
                        HttpContext.TraceIdentifier))
                };
            }

            // Set authentication cookies
            _cookieService.SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken);

            _logger.LogInformation("User registered successfully: {Email}", request.Email);

            // Return response without tokens (they're in cookies)
            return CreatedAtAction(nameof(GetCurrentUser), new { }, new
            {
                expiresAt = result.Data.ExpiresAt,
                user = result.Data.User
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user registration");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

    /// <summary>
   

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDTO request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ErrorResponseDTO.FromErrors(errors, HttpContext.TraceIdentifier));
            }

            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);

                return result.ErrorMessage switch
                {
                    "INVALID_CREDENTIALS" => Unauthorized(ErrorResponseDTO.FromMessage(
                        "Invalid email or password.",
                        HttpContext.TraceIdentifier)),
                    "TOKEN_GENERATION_FAILED" or "REFRESH_TOKEN_SAVE_FAILED" => StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ErrorResponseDTO.FromMessage(
                            "Authentication failed. Please try again later.",
                            HttpContext.TraceIdentifier)),
                    _ => BadRequest(ErrorResponseDTO.FromMessage(
                        result.ErrorMessage ?? "Login failed.",
                        HttpContext.TraceIdentifier))
                };
            }

            // Set authentication cookies
            _cookieService.SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            // Return response without tokens (they're in cookies)
            return Ok(new
            {
                expiresAt = result.Data.ExpiresAt,
                user = result.Data.User
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

   
    [HttpPost("refresh")]
    
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        try
        {
            // Read refresh token from cookie
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _cookieService.ClearAuthCookies();
                return Unauthorized(ErrorResponseDTO.FromMessage(
                    "Refresh token not provided.",
                    HttpContext.TraceIdentifier));
            }

            var request = new RefreshTokenRequestDTO { RefreshToken = refreshToken };
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Token refresh failed: {ErrorMessage}", result.ErrorMessage);

                // Clear invalid cookies
                _cookieService.ClearAuthCookies();

                return result.ErrorMessage switch
                {
                    "INVALID_REFRESH_TOKEN" => Unauthorized(ErrorResponseDTO.FromMessage(
                        "Invalid or expired refresh token.",
                        HttpContext.TraceIdentifier)),
                    "REFRESH_TOKEN_SAVE_FAILED" or "TOKEN_REFRESH_FAILED" => StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ErrorResponseDTO.FromMessage(
                            "Token refresh failed. Please try again later.",
                            HttpContext.TraceIdentifier)),
                    _ => BadRequest(ErrorResponseDTO.FromMessage(
                        result.ErrorMessage ?? "Token refresh failed.",
                        HttpContext.TraceIdentifier))
                };
            }

            // Set new authentication cookies
            _cookieService.SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken);

            _logger.LogInformation("Token refreshed successfully");

            // Return response without tokens (they're in cookies)
            return Ok(new
            {
                expiresAt = result.Data.ExpiresAt,
                user = result.Data.User
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token refresh");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            // Read refresh token from cookie
            var refreshToken = Request.Cookies["refresh_token"];

            // Always clear cookies, even if refresh token is missing
            _cookieService.ClearAuthCookies();

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var request = new LogoutDTO { RefreshToken = refreshToken };
                var result = await _authService.LogoutAsync(request, cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Logout failed: {ErrorMessage}", result.ErrorMessage);
                    // Still return success since cookies are cleared
                }
            }

            _logger.LogInformation("User logged out successfully");
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout");
            // Still clear cookies on error
            _cookieService.ClearAuthCookies();
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

    
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                {
                    _logger.LogWarning("User ID not found in token claims");
                    return Unauthorized(ErrorResponseDTO.FromMessage(
                        "Invalid authentication token.",
                        HttpContext.TraceIdentifier));
                }

                var result = await _authService.GetCurrentUserAsync(userId.Value, cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to get current user: {ErrorMessage}", result.ErrorMessage);

                    return result.ErrorMessage switch
                    {
                        "USER_NOT_FOUND" => NotFound(ErrorResponseDTO.FromMessage(
                            "User not found.",
                            HttpContext.TraceIdentifier)),
                        _ => BadRequest(ErrorResponseDTO.FromMessage(
                        result.ErrorMessage ?? "Failed to retrieve user information.",
                        HttpContext.TraceIdentifier))
                };
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving current user");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }
}
