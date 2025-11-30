using System;
using OneUniBackend.Entities;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.DTOs.Auth;
namespace OneUniBackend.Interfaces.Services;

public interface IGoogleOAuthService
{
    public Task<User?> GetUserByGoogleIDAsync(string googleID, CancellationToken cancellationToken = default);
    public Task<bool> SaveNewGoogleUserAsync(Guid userID, string googleID, CancellationToken cancellationToken = default);
    public Task<GoogleUserInfo?> ExchangeCodeforUserInfoAsync(string code, CancellationToken cancellationToken = default);
}
