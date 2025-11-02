using OneUni.Entities;
using OneUni.Common;

namespace OneUni.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Task<Result<string>> SaveRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<User?>> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    string HashRefreshToken(string refreshToken);
}

