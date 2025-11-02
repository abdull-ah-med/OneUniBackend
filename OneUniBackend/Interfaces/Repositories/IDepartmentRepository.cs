using OneUni.Entities;

namespace OneUni.Interfaces.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<IEnumerable<Department>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default);
}

