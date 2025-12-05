using System;
using System.ComponentModel.DataAnnotations;
namespace OneUniBackend.DTOs.Auth;
using OneUniBackend.Enums;

public class UserRoleDTO
{
    [Required]
    public UserRole Role { get; set; }
}
