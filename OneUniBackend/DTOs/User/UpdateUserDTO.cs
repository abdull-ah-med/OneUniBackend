using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.User;

public class UpdateUserDTO
{
    [EmailAddress]
    public string? Email { get; set; }
    
    public bool? IsActive { get; set; }
}

