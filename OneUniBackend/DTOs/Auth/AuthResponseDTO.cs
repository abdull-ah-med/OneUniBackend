using OneUni.DTOs.User;

namespace OneUni.DTOs.Auth;

public class AuthResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserDTO User { get; set; } = null!;
}

