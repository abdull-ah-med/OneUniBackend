using System;

namespace OneUniBackend.Models
{
    public class UserRefreshToken
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }

        public User User { get; set; } // navigation property
    }
}
