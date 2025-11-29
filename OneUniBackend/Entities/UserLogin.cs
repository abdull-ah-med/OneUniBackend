using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OneUniBackend.Entities;

[PrimaryKey("Loginprovider", "Providerkey")]
[Table("user_logins")]
[Index("Userid", Name = "ix_user_logins_userid")]
public partial class UserLogin
{
    [Column("loginprovider")]
    [StringLength(128)]
    public string Loginprovider { get; set; } = null!;

    [Column("providerkey")]
    [StringLength(128)]
    public string Providerkey { get; set; } = null!;

    [Column("providerdisplayname")]
    public string? Providerdisplayname { get; set; }

    [Column("userid")]
    public Guid UserId { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("UserLogins")]
    public virtual User User { get; set; } = null!;
}
