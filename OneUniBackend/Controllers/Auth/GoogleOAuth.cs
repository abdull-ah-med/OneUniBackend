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

        public GoogleOAuth(ICookieService cookieService, IGoogleOAuthService googleOAuthService, IOptions<JWTSettings> jwtSettings, IAuthService authService, ILogger<GoogleOAuth> logger)
        {
            _googleOAuthService = googleOAuthService;
            _jwtSettings = jwtSettings.Value;
            _authService = authService;
            _logger = logger;
            _cookieService = cookieService;
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
                    return BadRequest(ErrorResponseDTO.FromErrors(errors, HttpContext.TraceIdentifier));
                }
                GoogleUserInfo? googleUserObject = await _googleOAuthService.ExchangeCodeforUserInfoAsync(code, cancellationToken);
                if (googleUserObject == null)
                {
                    return StatusCode(
                                    StatusCodes.Status400BadRequest,
                                    ErrorResponseDTO.FromMessage(
                                        "Invalid Google OAuth code.",
                                        HttpContext.TraceIdentifier));
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
                            "INVALID_SIGNUP_REQUEST" => Unauthorized(ErrorResponseDTO.FromMessage(
                                "Invalid credentials.",
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
                    _cookieService.SetAuthCookies(Result.Data!.AccessToken, Result.Data!.RefreshToken);
                    _logger.LogInformation("Token refreshed successfully");

                    // Return response without tokens (they're in cookies)
                    return Ok(new
                    {
                        expiresAt = Result.Data.ExpiresAt,
                        user = Result.Data.User
                    });

                }
                // Temporary signup flow
                else if (existingUser == null)
                {
                    Result<AuthResponseDTO<GoogleUserInfo>> Result = await _authService.TempGoogleSignUpAsync(googleUserObject, cancellationToken);
                    if (!Result.IsSuccess)
                    {
                        _logger.LogWarning("Temporary signup failed for Google User ID: {Google User ID}, {Email}", googleUserObject.GoogleUserId, googleUserObject.UserEmail);

                        return Result.ErrorMessage switch
                        {
                            "USER_ALREADY_EXISTS" => Unauthorized(ErrorResponseDTO.FromMessage(
                                "User with matching credentials already exists.",
                                HttpContext.TraceIdentifier)),
                            "TOKEN_GENERATION_FAILED" or "REFRESH_TOKEN_SAVE_FAILED" => StatusCode(
                                StatusCodes.Status500InternalServerError,
                                ErrorResponseDTO.FromMessage(
                                    "Authentication failed. Please try again later.",
                                    HttpContext.TraceIdentifier)),
                            _ => BadRequest(ErrorResponseDTO.FromMessage(
                                Result.ErrorMessage ?? "Google Signin failed.",
                                HttpContext.TraceIdentifier))
                        };
                    }
                    _cookieService.SetAuthCookies(Result.Data!.AccessToken, null);
                    _logger.LogInformation("Temporary Google signup successful for Google User ID: {Google User ID}, {Email}", googleUserObject.GoogleUserId, googleUserObject.UserEmail);
                    return StatusCode(StatusCodes.Status307TemporaryRedirect, new
                    {
                        expiresAt = Result.Data.ExpiresAt,
                        user = Result.Data.User
                    });
                }
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ErrorResponseDTO.FromMessage(
                        "An unexpected error occurred. Please try again later.",
                        HttpContext.TraceIdentifier));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google OAuth callback. TraceId: {TraceId}", HttpContext.TraceIdentifier);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ErrorResponseDTO.FromMessage(
                        "An unexpected error occurred. Please try again later.",
                        HttpContext.TraceIdentifier));
            }
        }
        [ProducesResponseType(typeof(AuthResponseDTO<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDTO<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompleteGoogleSignUp([FromBody] CompleteGoogleSignUpRequestDTO googleSignUpRequest, CancellationToken cancellationToken)
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
                GoogleUserInfo? googleUserObject = await _googleOAuthService.ExchangeCodeforUserInfoAsync(googleSignUpRequest.GoogleUserId, cancellationToken);
                if (googleUserObject == null)
                {
                    return StatusCode(
                                    StatusCodes.Status400BadRequest,
                                    ErrorResponseDTO.FromMessage(
                                        "Invalid Google OAuth code.",
                                        HttpContext.TraceIdentifier));
                }
                // Read temporary access token from cookie BEFORE clearing it
                var temporaryAccessToken = Request.Cookies["access_token"];
                if (string.IsNullOrEmpty(temporaryAccessToken))
                {
                    return Unauthorized(ErrorResponseDTO.FromMessage(
                        "Temporary access token not found. Please start the signup process again.",
                        HttpContext.TraceIdentifier));
                }

                // Check if User with Google ID already exists
                User? existingUser = await _googleOAuthService.GetUserByGoogleIDAsync(googleUserObject.GoogleUserId, cancellationToken);
                if (existingUser != null)
                {
                    _cookieService.ClearAuthCookies();
                    return StatusCode(StatusCodes.Status409Conflict, ErrorResponseDTO.FromMessage("User with similar credentials exists.", HttpContext.TraceIdentifier));
                }
                
                Result<AuthResponseDTO<UserDTO>> Result = await _authService.CompleteGoogleSignupAsync(googleSignUpRequest, temporaryAccessToken, cancellationToken);
                
                if (!Result.IsSuccess)
                {
                    // Clear temporary cookie on failure
                    _cookieService.ClearAuthCookies();
                    _logger.LogWarning("Google signup completion failed for email: {Google User ID}, {Email}", googleUserObject.GoogleUserId, googleUserObject.UserEmail);

                    return Result.ErrorMessage switch
                    {
                        "INVALID_TEMPORARY_TOKEN" or "TOKEN_EXPIRED" => Unauthorized(ErrorResponseDTO.FromMessage(
                            "Invalid or expired temporary token. Please start the signup process again.",
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
                return CreatedAtAction(nameof(Result.Data.User), new { }, new
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
    }
}
