using System;
using System.Collections.Generic;

namespace OneUniBackend.Entities;

public partial class UserRefreshToken
{
    public Guid TokenId { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool? IsRevoked { get; set; }

    public virtual User User { get; set; } = null!;
}
