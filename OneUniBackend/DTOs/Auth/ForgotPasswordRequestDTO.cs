using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth;

public class ForgotPasswordRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

