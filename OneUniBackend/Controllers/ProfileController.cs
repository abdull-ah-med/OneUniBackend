using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using OneUniBackend.DTOs.Common;
using OneUniBackend.DTOs.Profile;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Utils;

namespace OneUniBackend.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IOneProfileService _profileService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IOneProfileService profileService, ILogger<ProfileController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(ErrorResponseDTO.FromMessage("Invalid authentication token.", HttpContext.TraceIdentifier));
        }

        var profile = await _profileService.GetStudentProfileAsync(userId.Value, cancellationToken);
        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpsertProfile([FromBody] OneProfileUpsertRequestDto request, CancellationToken cancellationToken)
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
        if (userId is null)
        {
            return Unauthorized(ErrorResponseDTO.FromMessage("Invalid authentication token.", HttpContext.TraceIdentifier));
        }

        try
        {
            var (profile, uploads) = await _profileService.UpsertStudentProfileAsync(userId.Value, request, cancellationToken);
            return Ok(new { profile, uploads });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error while upserting profile for user {UserId}", userId);
            return BadRequest(ErrorResponseDTO.FromMessage(ex.Message, HttpContext.TraceIdentifier));
        }
    }
}

