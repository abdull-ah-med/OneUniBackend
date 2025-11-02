using System.ComponentModel.DataAnnotations;
using OneUni.Enums;

namespace OneUni.DTOs.User;

public class CreateUserDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    public UserRole Role { get; set; }
}

