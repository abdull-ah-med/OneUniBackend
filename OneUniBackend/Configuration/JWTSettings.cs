using System;

namespace OneUniBackend.Configuration;

public class JWTSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int AccessTokenExpiresInMinutes { get; set; }
    public int RefreshTokenExpiresInDays { get; set; }
}
