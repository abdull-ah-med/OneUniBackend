using System;
using Microsoft.EntityFrameworkCore;
using OneUni.Interfaces;
using OneUni.Interfaces.Repositories;
using OneUni.Entities;
using OneUni.Infrastructure.Data;

namespace OneUni.Infrastructure.Repositories;

public class UserRefreshTokenRepository : GenericRepository<UserRefreshToken>, IUserRefreshTokenRepository
{
    public UserRefreshTokenRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<UserRefreshToken?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(urt => urt.TokenHash == refreshTokenHash, cancellationToken);
    }
    public async Task<IEnumerable<UserRefreshToken>> GetUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(urt => urt.UserId == userId).ToListAsync(cancellationToken);
    }
    public async Task<bool> RevokeRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        var token = await _dbSet.FirstOrDefaultAsync(urt => urt.TokenHash == refreshTokenHash, cancellationToken);
        if (token == null)
        {
            return false;
        }
        token.IsRevoked = true;
        _dbSet.Update(token);
        return true;

    }
    public async Task<bool> RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbSet.Where(urt => urt.UserId == userId).ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
        _dbSet.UpdateRange(tokens);
        return true;

    }
    public async Task<bool> RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expiredTokens = await _dbSet.Where(urt => urt.ExpiresAt <= now).ToListAsync(cancellationToken);
        _dbSet.RemoveRange(expiredTokens);
        return true;

    }
}
