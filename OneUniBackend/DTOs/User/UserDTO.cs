using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.User;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }
}

