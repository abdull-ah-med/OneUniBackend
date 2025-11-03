using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.Auth;

public class ResetPasswordRequestDTO
{
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = null!;

    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = null!;
}

