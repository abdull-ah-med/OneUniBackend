using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth;

/// <summary>
/// DTO for changing password (refresh token is read from cookie)
/// </summary>
public record ChangePasswordDTO
{
    [Required]
    [MinLength(6)]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = null!;
}

