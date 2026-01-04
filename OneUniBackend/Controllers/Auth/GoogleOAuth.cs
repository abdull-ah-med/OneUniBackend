using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneUniBackend.Configuration;
using OneUniBackend.Interfaces.Services;
using Microsoft.Extensions.Options;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.DTOs.Common;
using Google.Apis.Auth;
using OneUniBackend.Common;
using OneUniBackend.Entities;
using OneUniBackend.Services;
using OneUniBackend.DTOs.User;
using Microsoft.Extensions.Configuration;
using OneUniBackend.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
namespace OneUniBackend.Controllers.Auth
{
    [Route("api/google-oauth")]
    [ApiController]
    public class GoogleOAuth : ControllerBase
    {
        private readonly IGoogleOAuthService _googleOAuthService;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<GoogleOAuth> _logger;
        private readonly IAuthService _authService;
        private readonly ICookieService _cookieService;
        private readonly IConfiguration _configuration;
        private static readonly HashSet<string> AllowedFrontendPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/login/callback",
            "/signup/callback",
            "/auth/error"
        };

        public GoogleOAuth(ICookieService cookieService, IGoogleOAuthService googleOAuthService, IOptions<JWTSettings> jwtSettings, IAuthService authService, ILogger<GoogleOAuth> logger, IConfiguration configuration)
        {
            _googleOAuthService = googleOAuthService;
            _jwtSettings = jwtSettings.Value;
            _authService = authService;
            _logger = logger;
            _cookieService = cookieService;
            _configuration = configuration;
        }
        [HttpGet("callback")]
        [ProducesResponseType(typeof(AuthResponseDTO<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDTO<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(AuthResponseDTO<GoogleUserInfo>), StatusCodes.Status307TemporaryRedirect)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleOAuthCallback([FromQuery] string code, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return RespondError(StatusCodes.Status400BadRequest, ErrorResponseDTO.FromErrors(errors, HttpContext.TraceIdentifier), "invalid_request");
                }
                GoogleUserInfo? googleUserObject = await _googleOAuthService.ExchangeCodeforUserInfoAsync(code, cancellationToken);
                if (googleUserObject == null)
                {
                    return RespondError(StatusCodes.Status400BadRequest,
                        ErrorResponseDTO.FromMessage(
                            "Invalid Google OAuth code.",
                            HttpContext.TraceIdentifier),
                        "invalid_code");
                }
                // Check if User with Google ID already exists
                User? existingUser = await _googleOAuthService.GetUserByGoogleIDAsync(googleUserObject.GoogleUserId, cancellationToken);
                // If User exists, log them in
                if (existingUser != null)
                {
                    Result<AuthResponseDTO<UserDTO>> Result = await _authService.GoogleLoginAsync(googleUserObject, cancellationToken);
                    if (!Result.IsSuccess)
                    {
                        _logger.LogWarning("Login failed for email: {Google User ID}, {Email}", googleUserObject.GoogleUserId, googleUserObject.UserEmail);

                        return Result.ErrorMessage switch
                        {
                            "INVALID_SIGNUP_REQUEST" => RespondError(
                                StatusCodes.Status400BadRequest,
                                ErrorResponseDTO.FromMessage(
                                    "Invalid credentials.",
                                    HttpContext.TraceIdentifier),
                                "invalid_credentials"),
                            "TOKEN_GENERATION_FAILED" or "REFRESH_TOKEN_SAVE_FAILED" => RespondError(
                                StatusCodes.Status500InternalServerError,
                                ErrorResponseDTO.FromMessage(
                                    "Authentication failed. Please try again later.",
                                    HttpContext.TraceIdentifier),
                                "auth_failed"),
                            _ => RespondError(
                                StatusCodes.Status400BadRequest,
                                ErrorResponseDTO.FromMessage(
                                    Result.ErrorMessage ?? "Google SignUp failed.",
                                    HttpContext.TraceIdentifier),
                                "google_login_failed")
                        };
                    }
                    _cookieService.SetAuthCookies(Result.Data!.AccessToken, Result.Data!.RefreshToken);
                    _logger.LogInformation("Google login successful for: {Email}", googleUserObject.UserEmail);

                    return RedirectToFrontend("/login/callback");
                }
                // Temporary signup flow
                else if (existingUser == null)
                {
                    Result<AuthResponseDTO<GoogleUserInfo>> Result = await _authService.TempGoogleSignUpAsync(code, googleUserObject, cancellationToken);
                    if (!Result.IsSuccess)
                    {
                        _logger.LogWarning("Temporary signup failed for Google User ID: {Google User ID}, {Email}", googleUserObject.GoogleUserId, googleUserObject.UserEmail);

                        return Result.ErrorMessage switch
                        {
                            "USER_ALREADY_EXISTS" => RespondError(
                                StatusCodes.Status409Conflict,
                                ErrorResponseDTO.FromMessage(
                                    "User with matching credentials already exists.",
                                    HttpContext.TraceIdentifier),
                                "user_exists"),
                            "TOKEN_GENERATION_FAILED" or "REFRESH_TOKEN_SAVE_FAILED" => RespondError(
                                StatusCodes.Status500InternalServerError,
                                ErrorResponseDTO.FromMessage(
                                    "Authentication failed. Please try again later.",
                                    HttpContext.TraceIdentifier),
                                "auth_failed"),
                            _ => RespondError(
                                StatusCodes.Status400BadRequest,
                                ErrorResponseDTO.FromMessage(
                                    Result.ErrorMessage ?? "Google Signin failed.",
                                    HttpContext.TraceIdentifier),
                                "google_signup_failed")
                        };
                    }
                    _cookieService.SetAuthCookies(Result.Data!.AccessToken, null);
                    _logger.LogInformation("Temporary Google signup successful for Google User ID: {Google User ID}, {Email}", googleUserObject.GoogleUserId, googleUserObject.UserEmail);
                    
                    return RedirectToFrontend("/signup/callback");
                }
                return RespondError(
                    StatusCodes.Status500InternalServerError,
                    ErrorResponseDTO.FromMessage(
                        "An unexpected error occurred. Please try again later.",
                        HttpContext.TraceIdentifier),
                    "server_error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google OAuth callback. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                return RespondError(
                    StatusCodes.Status500InternalServerError,
                    ErrorResponseDTO.FromMessage(
                        "An unexpected error occurred. Please try again later.",
                        HttpContext.TraceIdentifier),
                    "server_error");
            }
        }
        [HttpPost("complete-signup")]
                public async Task<IActionResult> CompleteGoogleSignUp([FromBody] UserRoleDTO userRole, CancellationToken cancellationToken)
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
                var temporaryAccessToken = Request.Cookies["access_token"];
                if (string.IsNullOrEmpty(temporaryAccessToken))
                {
                    return Unauthorized(ErrorResponseDTO.FromMessage(
                        "Temporary access token not found. Please start the signup process again.",
                        HttpContext.TraceIdentifier));
                }
              
                // Read temporary access token from cookie BEFORE clearing it

                // Check if User with Google ID already exists
                
                    
                Result<AuthResponseDTO<UserDTO>> Result = await _authService.CompleteGoogleSignupAsync(userRole, temporaryAccessToken, cancellationToken);
                
                if (!Result.IsSuccess)
                {
                    // Clear temporary cookie on failure
                    _cookieService.ClearAuthCookies();
                    _logger.LogWarning("Google signup completion failed for email: {Google User ID}", Result.Data);

                    return Result.ErrorMessage switch
                    {
                        "INVALID_TEMPORARY_TOKEN" or "TOKEN_EXPIRED" => Unauthorized(ErrorResponseDTO.FromMessage(
                            "Invalid or expired temporary token. Please start the signup process again.",
                            HttpContext.TraceIdentifier)),
                        "INVALID_GOOGLE_USER_INFO" => BadRequest(ErrorResponseDTO.FromMessage(
                            "Google user information mismatch.",
                            HttpContext.TraceIdentifier)),
                        "TOKEN_MISMATCH" => Unauthorized(ErrorResponseDTO.FromMessage(
                            "Token mismatch. Please start the signup process again.",
                            HttpContext.TraceIdentifier)),
                        "USER_ALREADY_EXISTS" => Conflict(ErrorResponseDTO.FromMessage(
                            "User already exists.",
                            HttpContext.TraceIdentifier)),
                        "TOKEN_GENERATION_FAILED" or "REFRESH_TOKEN_SAVE_FAILED" => StatusCode(
                            StatusCodes.Status500InternalServerError,
                            ErrorResponseDTO.FromMessage(
                                "Authentication failed. Please try again later.",
                                HttpContext.TraceIdentifier)),
                        _ => BadRequest(ErrorResponseDTO.FromMessage(
                            Result.ErrorMessage ?? "Google SignUp failed.",
                            HttpContext.TraceIdentifier))
                    };
                }
                
                // Clear temporary cookie and set new auth cookies on success
                _cookieService.ClearAuthCookies();
                _cookieService.SetAuthCookies(Result.Data!.AccessToken, Result.Data!.RefreshToken);
                _logger.LogInformation("User registered successfully: {Email}", Result.Data.User!.Email);

                // Return response without tokens (they're in cookies)
                return StatusCode(StatusCodes.Status201Created, new
                {
                    expiresAt = Result.Data.ExpiresAt,
                    user = Result.Data.User
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during completing Google signup. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ErrorResponseDTO.FromMessage(
                        "An unexpected error occurred. Please try again later.",
                        HttpContext.TraceIdentifier));
            }
        }

        private IActionResult RedirectToFrontend(string path, string? errorCode = null, int statusCode = StatusCodes.Status303SeeOther)
        {
            var targetPath = AllowedFrontendPaths.Contains(path) ? path : "/auth/error";
            var configuredBase = _configuration["FrontendBaseUrl"]
                ?? throw new InvalidOperationException("FrontendBaseUrl is not configured. Ensure FRONTEND_BASE_URL environment variable is set.");
            
            if (!Uri.TryCreate(configuredBase, UriKind.Absolute, out var baseUri))
            {
                throw new InvalidOperationException($"FrontendBaseUrl '{configuredBase}' is not a valid absolute URI.");
            }

            var uriBuilder = new UriBuilder(baseUri)
            {
                Path = targetPath
            };

            if (!string.IsNullOrWhiteSpace(errorCode))
            {
                uriBuilder.Query = $"error={Uri.EscapeDataString(errorCode)}";
            }

            Response.StatusCode = statusCode;
            Response.Headers.Location = uriBuilder.Uri.ToString();
            return new EmptyResult();
        }

        private IActionResult RespondError(int statusCode, ErrorResponseDTO payload, string errorCode)
        {
            if (PrefersJson())
            {
                return StatusCode(statusCode, payload);
            }

            return RedirectToFrontend("/auth/error", errorCode);
        }

        private bool PrefersJson()
        {
            var acceptHeader = Request.Headers[HeaderNames.Accept];
            return acceptHeader.Any(value =>
                value != null && value.IndexOf("application/json", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}