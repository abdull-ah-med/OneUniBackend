using OneUniBackend.DTOs.User;

namespace OneUniBackend.DTOs.Auth;

public class AuthResponseDTO<T>
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public T? User { get; set; }
}

