using OneUni.Entities;

namespace OneUni.Interfaces.Repositories;

public interface IUniversityRepository : IGenericRepository<University>
{
    Task<IEnumerable<University>> GetActiveUniversitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<University>> GetByLocationAsync(string city, string? province = null, CancellationToken cancellationToken = default);
    Task<University?> GetWithProgramsAsync(Guid id, CancellationToken cancellationToken = default);
}

