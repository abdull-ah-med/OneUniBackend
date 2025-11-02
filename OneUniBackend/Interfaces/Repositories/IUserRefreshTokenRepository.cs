using System;
using OneUni.Entities;

namespace OneUni.Interfaces.Repositories;

public interface IUserRefreshTokenRepository : IGenericRepository<UserRefreshToken>
{
    Task<UserRefreshToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRefreshToken>> GetUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken = default);


}
