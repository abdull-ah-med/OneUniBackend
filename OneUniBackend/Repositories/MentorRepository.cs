using Microsoft.EntityFrameworkCore;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Entities;
using OneUniBackend.Enums;
using OneUniBackend.Data;

namespace OneUniBackend.Repositories;

public class MentorRepository : GenericRepository<Mentor>, IMentorRepository
{
    public MentorRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Mentor>> GetActiveVerifiedMentorsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.IsActive == true && m.VerificationStatus == VerificationStatus.Verified)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Mentor>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.UniversityId == universityId && m.IsActive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<Mentor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.University)
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
    }
}

