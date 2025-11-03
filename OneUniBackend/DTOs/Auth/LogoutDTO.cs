using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.Auth;

public class LogoutDTO
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

