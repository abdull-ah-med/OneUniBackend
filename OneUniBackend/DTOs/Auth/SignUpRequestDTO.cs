using System.ComponentModel.DataAnnotations;
using OneUni.Enums;

namespace OneUni.DTOs.Auth;

public class SignUpRequestDTO
{
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;

    [Required]
    public UserRole Role { get; set; }
}

