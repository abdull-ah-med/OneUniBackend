using System.ComponentModel.DataAnnotations;

namespace OneUni.DTOs.User;

public class UpdateUserDTO
{
    [EmailAddress]
    public string? Email { get; set; }
    
    public bool? IsActive { get; set; }
}

