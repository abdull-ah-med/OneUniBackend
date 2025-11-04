using OneUniBackend.Entities;

namespace OneUniBackend.Interfaces.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<IEnumerable<Department>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default);
}

