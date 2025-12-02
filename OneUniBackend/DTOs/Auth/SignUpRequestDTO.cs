using System.ComponentModel.DataAnnotations;
using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Auth;

public class SignUpRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    [MaxLength(255)]
    public string? FullName { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;


    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;

    [Required]
    public UserRole Role { get; set; }
}

