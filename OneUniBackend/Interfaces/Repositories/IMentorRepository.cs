using OneUni.Entities;

namespace OneUni.Interfaces.Repositories;

public interface IMentorRepository : IGenericRepository<Mentor>
{
    Task<IEnumerable<Mentor>> GetActiveVerifiedMentorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Mentor>> GetByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default);
    Task<Mentor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

