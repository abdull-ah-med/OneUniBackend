using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth;

public class LogoutDTO
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

