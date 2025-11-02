using Microsoft.EntityFrameworkCore;
using OneUni.Interfaces.Repositories;
using OneUni.Entities;
using OneUni.Enums;
using OneUni.Infrastructure.Data;
using ApplicationEntity = OneUni.Entities.Application;

namespace OneUni.Infrastructure.Repositories;

public class ApplicationRepository : GenericRepository<ApplicationEntity>, IApplicationRepository
{
    public ApplicationRepository(OneUniDbContext context) : base(context)
    {
        
    }

    public async Task<IEnumerable<ApplicationEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.University)
            .Include(a => a.Program)
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApplicationEntity>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.User)
            .Include(a => a.Program)
            .Where(a => a.UniversityId == universityId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApplicationEntity>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.University)
            .Include(a => a.Program)
            .Include(a => a.User)
            .Where(a => a.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationEntity?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.University)
            .Include(a => a.Program)
            .Include(a => a.User)
            .Include(a => a.Cycle)
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.ApplicationId == id, cancellationToken);
    }
}

