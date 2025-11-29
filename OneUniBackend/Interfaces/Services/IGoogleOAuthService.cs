using System;
using OneUniBackend.Entities;
using OneUniBackend.Interfaces.Repositories;

namespace OneUniBackend.Interfaces.Services;

public interface IGoogleOAuthService
{
    public Task<User?> GetUserByGoogleIDAsync(string googleID, CancellationToken cancellationToken = default);
    public Task<bool> SaveNewGoogleUserAsync(UserLogin userLogin, string googleID, string googleSecret, CancellationToken cancellationToken = default);
}
