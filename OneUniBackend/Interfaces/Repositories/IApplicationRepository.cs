using OneUniBackend.Entities;
using OneUniBackend.Enums;
using ApplicationEntity = OneUniBackend.Entities.Application;

namespace OneUniBackend.Interfaces.Repositories;

public interface IApplicationRepository : IGenericRepository<ApplicationEntity>
{
    Task<IEnumerable<ApplicationEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationEntity>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationEntity>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default);
    Task<ApplicationEntity?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}

