using System;
using System.ComponentModel.DataAnnotations;
using OneUniBackend.Enums;

namespace OneUniBackend.DTOs.Auth;

public record CompleteGoogleSignUpRequestDTO
(
    [Required]
    string GoogleUserId,
    [Required]
    string UserName,
    [Required]
     string UserEmail,
    [Required]
    UserRole Role
);
