using Microsoft.EntityFrameworkCore;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Entities;
using OneUniBackend.Data;

namespace OneUniBackend.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Department>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Programs)
            .Where(d => d.UniversityId == universityId && d.IsActive == true)
            .ToListAsync(cancellationToken);
    }
}

