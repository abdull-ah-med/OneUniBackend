using System;
using System.ComponentModel.DataAnnotations;
using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Auth;

public class CompleteGoogleSignUpRequestDTO
{
    [Required]
    public string GoogleUserId { get; set; }
    [Required]
    public string UserName { get; set; } = "";
    [Required]
    public string UserEmail { get; set; } = ""; 
    [Required]
    public string Code { get; set; } = "";
}   