using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.Auth;

public class RefreshTokenRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

