using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OneUni.Entities;

[Table("user_refresh_tokens")]
[Index("UserId", Name = "idx_user_refresh_tokens_user_id")]
public partial class UserRefreshToken
{
    [Key]
    [Column("token_id")]
    public Guid TokenId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("token_hash")]
    [StringLength(255)]
    public string TokenHash { get; set; } = null!;

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("expires_at", TypeName = "timestamp without time zone")]
    public DateTime ExpiresAt { get; set; }

    [Column("is_revoked")]
    public bool? IsRevoked { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserRefreshTokens")]
    public virtual User User { get; set; } = null!;
}
