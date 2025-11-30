using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneUniBackend.Configuration;
using OneUniBackend.Interfaces.Services;
using Microsoft.Extensions.Options;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.DTOs.Common;
using Google.Apis.Auth;
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
                var
            }
        }
    }
}
