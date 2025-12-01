using System;
using System.ComponentModel.DataAnnotations;
using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Auth;

public class GoogleUserInfo
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string GoogleUserId { get; set; }
    [Required]
    public string UserEmail { get; set; }
    [Required]
    public bool isEmailVerified { get; set; }
    public UserRole? Role { get; set; }

}
