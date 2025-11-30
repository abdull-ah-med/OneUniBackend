using System.ComponentModel.DataAnnotations;

namespace OneUniBackend.DTOs.Auth
{
    public class GoogleLoginRequestDTO
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }
}
