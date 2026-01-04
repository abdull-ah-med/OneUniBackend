using System;
using System.Collections.Generic;

namespace OneUniBackend.Entities;

public partial class UserLogin
{
    public string Loginprovider { get; set; } = null!;

    public string Providerkey { get; set; } = null!;

    public string? Providerdisplayname { get; set; }

    public Guid Userid { get; set; }

    public virtual User User { get; set; } = null!;
}
