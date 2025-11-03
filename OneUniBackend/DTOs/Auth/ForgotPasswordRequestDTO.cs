using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.Auth;

public class ForgotPasswordRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

