using OneUni.Entities;

namespace OneUni.Interfaces.Repositories;

public interface IAdmissionCycleRepository : IGenericRepository<AdmissionCycle>
{
    Task<IEnumerable<AdmissionCycle>> GetActiveByUniversityIdAsync(Guid universityId, CancellationToken cancellationToken = default);
    Task<AdmissionCycle?> GetCurrentCycleAsync(Guid universityId, CancellationToken cancellationToken = default);
}

