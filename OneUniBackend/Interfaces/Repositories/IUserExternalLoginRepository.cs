using System;
using OneUniBackend.Entities;
using OneUniBackend.Repositories;
using System.Threading;
namespace OneUniBackend.Interfaces.Repositories;

public interface IUserExternalLoginRepository : IGenericRepository<UserLogin>
{
    Task<User?> FindUserByProviderAndKeyAsync(string provider, string providerKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserLogin>> GetLoginsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string provider, string providerKey, CancellationToken cancellationToken = default);

}

