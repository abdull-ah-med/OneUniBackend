using System;

namespace OneUniBackend.Interfaces.Services;

public interface ICookieService
{
    void SetAuthCookies(string accessToken, string? refreshToken);
    void ClearAuthCookies();
}
