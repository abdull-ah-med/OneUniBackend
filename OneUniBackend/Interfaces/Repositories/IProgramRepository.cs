using OneUniBackend.Entities;
using ProgramEntity = OneUniBackend.Entities.Program;

namespace OneUniBackend.Interfaces.Repositories;

public interface IProgramRepository : IGenericRepository<ProgramEntity>
{
    Task<IEnumerable<ProgramEntity>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProgramEntity>> GetActiveProgramsAsync(CancellationToken cancellationToken = default);
}

