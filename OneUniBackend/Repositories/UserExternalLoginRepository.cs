using System;
using OneUniBackend.Entities;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace OneUniBackend.Repositories;

public class UserExternalLoginRepository : GenericRepository<UserLogin>, IUserExternalLoginRepository
{
    public UserExternalLoginRepository(OneUniDbContext context) : base(context)
    {
    }
   public async Task<User?> FindUserByProviderAndKeyAsync(string provider, string providerKey, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ul => ul.Loginprovider == provider && ul.Providerkey == providerKey)
            .Select(ul => ul.User)
            .FirstOrDefaultAsync(cancellationToken);
    }
     public async Task<IEnumerable<UserLogin>> GetLoginsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ul => ul.UserId == userId)
            .ToListAsync(cancellationToken);
    }
    public async Task<bool> ExistsAsync(string provider, string providerKey, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(ul => ul.Loginprovider == provider && ul.Providerkey == providerKey, cancellationToken);
    }
    
}
