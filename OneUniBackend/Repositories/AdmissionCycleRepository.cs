using Microsoft.EntityFrameworkCore;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Entities;
using OneUniBackend.Data;

namespace OneUniBackend.Repositories;

public class AdmissionCycleRepository : GenericRepository<AdmissionCycle>, IAdmissionCycleRepository
{
    public AdmissionCycleRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AdmissionCycle>> GetActiveByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ac => ac.UniversityId == universityId && ac.IsActive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<AdmissionCycle?> GetCurrentCycleAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _dbSet
            .Where(ac => ac.UniversityId == universityId 
                      && ac.IsActive == true 
                      && ac.ApplicationStartDate <= today 
                      && ac.ApplicationEndDate >= today)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

