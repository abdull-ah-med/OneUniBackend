using OneUniBackend.Entities;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.Common;

namespace OneUniBackend.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Task<Result<string>> SaveRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<Result<User?>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken = default);
    string GenerateTemporaryAccessToken(GoogleUserInfo googleUserInfo);
    string HashRefreshToken(string refreshToken);

}

