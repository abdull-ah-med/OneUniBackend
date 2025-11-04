using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth;

public class RefreshTokenRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

