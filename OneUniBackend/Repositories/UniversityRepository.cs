using Microsoft.EntityFrameworkCore;
using OneUni.Interfaces.Repositories;
using OneUni.Entities;
using OneUni.Infrastructure.Data;

namespace OneUni.Infrastructure.Repositories;

public class UniversityRepository : GenericRepository<University>, IUniversityRepository
{
    public UniversityRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<University>> GetActiveUniversitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(u => u.IsActive == true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<University>> GetByLocationAsync(string city, string? province = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.City == city && u.IsActive == true);
        
        if (!string.IsNullOrEmpty(province))
        {
            query = query.Where(u => u.Province == province);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<University?> GetWithProgramsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Departments)
            .ThenInclude(d => d.Program)
            .FirstOrDefaultAsync(u => u.UniversityId == id, cancellationToken);
    }
}

