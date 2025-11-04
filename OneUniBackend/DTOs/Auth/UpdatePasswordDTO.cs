using System;
using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth;

public class ChangePasswordRequestDTO
{
    [Required]
    [MinLength(6)]
    public string CurrentPassword { get; set; } = null!;
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = null!;
    [Required]
    public string refreshToken { get; set; } = null!;
    
}
