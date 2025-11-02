using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.Auth;

public class RevokeTokenRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

