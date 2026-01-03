using Microsoft.EntityFrameworkCore;
using OneUniBackend.Data;
using OneUniBackend.Entities;
using OneUniBackend.Interfaces.Repositories;

namespace OneUniBackend.Repositories;

public class EducationalRecordRepository : GenericRepository<EducationalRecord>, IEducationalRecordRepository
{
    public EducationalRecordRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<EducationalRecord>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<EducationalRecord?> GetByIdForUserAsync(Guid recordId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            r => r.RecordId == recordId && r.UserId == userId,
            cancellationToken);
    }
}

