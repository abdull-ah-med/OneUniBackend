using Microsoft.EntityFrameworkCore;
using OneUni.Interfaces.Repositories;
using OneUni.Entities;
using OneUni.Infrastructure.Data;

namespace OneUni.Infrastructure.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Department>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Program)
            .Where(d => d.UniversityId == universityId && d.IsActive == true)
            .ToListAsync(cancellationToken);
    }
}

