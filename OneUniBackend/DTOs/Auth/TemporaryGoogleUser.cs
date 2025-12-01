using System;
using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth;

public class TemporaryGoogleUser
{
    [Required]
    public string GoogleUserId { get; set; }

}
