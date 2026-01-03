using OneUniBackend.Entities;

namespace OneUniBackend.Interfaces.Repositories;

public interface IEducationalRecordRepository : IGenericRepository<EducationalRecord>
{
    Task<IReadOnlyList<EducationalRecord>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<EducationalRecord?> GetByIdForUserAsync(Guid recordId, Guid userId, CancellationToken cancellationToken = default);
}

