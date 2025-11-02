using System;
using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.Auth;

public class ChangePasswordRequestDTO
{
    [Required]
    [MinLength(6)]
    public string CurrentPassword { get; set; } = null!;
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = null!;
}
