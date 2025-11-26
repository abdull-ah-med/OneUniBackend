using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneUniBackend.Configuration;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.DTOs.Common;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Utils;
using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly JWTSettings _jwtSettings;

    public AuthController(IAuthService authService, ILogger<AuthController> logger, IOptions<JWTSettings> jwtSettings)
    {
        _authService = authService;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Sets authentication cookies (access token and refresh token)
    /// </summary>
    private void SetAuthCookies(string accessToken, string refreshToken, DateTime accessTokenExpiry)
    {
        // Access token cookie - sent to all /api endpoints
        Response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api",
            Expires = accessTokenExpiry
        });

        // Refresh token cookie - only sent to /api/auth/refresh endpoint
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth/refresh",
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays)
        });
    }

    /// <summary>
    /// Clears authentication cookies
    /// </summary>
    private void ClearAuthCookies()
    {
        Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api"
        });

        Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth/refresh"
        });
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with access and refresh tokens</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
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
                        ErrorResponseDTO.FromMessage(
                            "Failed to register user. Please try again later.",
                            HttpContext.TraceIdentifier)),
                    _ => BadRequest(ErrorResponseDTO.FromMessage(
                        result.ErrorMessage ?? "Registration failed.",
                        HttpContext.TraceIdentifier))
                };
            }

            // Set authentication cookies
            SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken, result.Data.ExpiresAt);

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
    /// Authenticate user and receive access and refresh tokens
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
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
            SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken, result.Data.ExpiresAt);

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

    /// <summary>
    /// Refresh access token using refresh token from cookie
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access and refresh tokens set as cookies</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        try
        {
            // Read refresh token from cookie
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
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
                ClearAuthCookies();

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
            SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken, result.Data.ExpiresAt);

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

    /// <summary>
    /// Logout user, revoke refresh token and clear cookies
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            // Read refresh token from cookie
            var refreshToken = Request.Cookies["refresh_token"];
            
            // Always clear cookies, even if refresh token is missing
            ClearAuthCookies();

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
            ClearAuthCookies();
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(OneUniBackend.DTOs.User.UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="request">Current and new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDTO request,
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

            var userId = User.GetUserId();
            if (userId == null)
            {
                _logger.LogWarning("User ID not found in token claims");
                return Unauthorized(ErrorResponseDTO.FromMessage(
                    "Invalid authentication token.",
                    HttpContext.TraceIdentifier));
            }

            // Read refresh token from cookie
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(ErrorResponseDTO.FromMessage(
                    "Refresh token not provided.",
                    HttpContext.TraceIdentifier));
            }

            var changePasswordRequest = new ChangePasswordRequestDTO
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
                refreshToken = refreshToken
            };

            var result = await _authService.ChangePasswordAsync(userId.Value, changePasswordRequest, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password change failed for user {UserId}: {ErrorMessage}", userId, result.ErrorMessage);

                return result.ErrorMessage switch
                {
                    "INVALID_REFRESH_TOKEN" => Unauthorized(ErrorResponseDTO.FromMessage(
                        "Invalid refresh token.",
                        HttpContext.TraceIdentifier)),
                    "INVALID_CURRENT_PASSWORD" => BadRequest(ErrorResponseDTO.FromMessage(
                        "Current password is incorrect.",
                        HttpContext.TraceIdentifier)),
                    "PASSWORD_CHANGE_FAILED" => StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ErrorResponseDTO.FromMessage(
                            "Failed to change password. Please try again later.",
                            HttpContext.TraceIdentifier)),
                    _ => BadRequest(ErrorResponseDTO.FromMessage(
                        result.ErrorMessage ?? "Password change failed.",
                        HttpContext.TraceIdentifier))
                };
            }

            // Clear cookies since all refresh tokens are revoked after password change
            ClearAuthCookies();

            _logger.LogInformation("Password changed successfully for user {UserId}", userId);
            return Ok(new { message = "Password changed successfully. Please log in again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password change");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

    /// <summary>
    /// Request password reset email
    /// </summary>
    /// <param name="request">Email address for password reset</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequestDTO request,
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

            var result = await _authService.RequestPasswordResetAsync(request.Email, cancellationToken);

            // Always return success to prevent email enumeration attacks
            _logger.LogInformation("Password reset requested for email: {Email}", request.Email);
            return Ok(new { message = "If the email exists, a password reset link has been sent." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password reset request");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    /// <param name="token">Password reset token</param>
    /// <param name="request">New password details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword(
        [FromQuery][Required] string token,
        [FromBody] ResetPasswordRequestDTO request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(token))
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ErrorResponseDTO.FromErrors(errors, HttpContext.TraceIdentifier));
            }

            var result = await _authService.ResetPasswordAsync(token, request.NewPassword, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password reset failed: {ErrorMessage}", result.ErrorMessage);

                return result.ErrorMessage switch
                {
                    "INVALID_TOKEN" or "TOKEN_EXPIRED" => BadRequest(ErrorResponseDTO.FromMessage(
                        "Invalid or expired reset token.",
                        HttpContext.TraceIdentifier)),
                    _ => StatusCode(
                        StatusCodes.Status500InternalServerError,
                        ErrorResponseDTO.FromMessage(
                            "Failed to reset password. Please try again later.",
                            HttpContext.TraceIdentifier))
                };
            }

            _logger.LogInformation("Password reset successfully");
            return Ok(new { message = "Password has been reset successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password reset");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }

    /// <summary>
    /// Verify email using verification token
    /// </summary>
    /// <param name="token">Email verification token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpGet("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail(
        [FromQuery][Required] string token,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(ErrorResponseDTO.FromMessage(
                    "Verification token is required.",
                    HttpContext.TraceIdentifier));
            }

            var result = await _authService.VerifyEmailAsync(token, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Email verification failed: {ErrorMessage}", result.ErrorMessage);

                return BadRequest(ErrorResponseDTO.FromMessage(
                    "Invalid or expired verification token.",
                    HttpContext.TraceIdentifier));
            }

            _logger.LogInformation("Email verified successfully");
            return Ok(new { message = "Email verified successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during email verification");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ErrorResponseDTO.FromMessage(
                    "An unexpected error occurred. Please try again later.",
                    HttpContext.TraceIdentifier));
        }
    }
}
