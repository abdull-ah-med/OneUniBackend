using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth;

public class LoginRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}

