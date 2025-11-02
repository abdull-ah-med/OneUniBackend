using Microsoft.EntityFrameworkCore;
using OneUni.Interfaces.Repositories;
using OneUni.Entities;
using OneUni.Infrastructure.Data;
using ProgramEntity = OneUni.Entities.Program;
namespace OneUni.Infrastructure.Repositories;

public class ProgramRepository : GenericRepository<ProgramEntity>, IProgramRepository
{
    public ProgramRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProgramEntity>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.DepartmentId == departmentId && p.IsActive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProgramEntity>> GetActiveProgramsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Department)
            .ThenInclude(d => d.University)
            .Where(p => p.IsActive == true)
            .ToListAsync(cancellationToken);
    }
}

