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
using OneUniBackend.Infrastructure.Services;
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
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status201Created)]
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
                if(existingUser != null)
                {
                    var Result = await _authService.GoogleLoginAsync(googleUserObject, cancellationToken);
                }
                else if()
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
    }
}
