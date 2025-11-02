using OneUni.Entities;
using ProgramEntity = OneUni.Entities.Program;

namespace OneUni.Interfaces.Repositories;

public interface IProgramRepository : IGenericRepository<ProgramEntity>
{
    Task<IEnumerable<ProgramEntity>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProgramEntity>> GetActiveProgramsAsync(CancellationToken cancellationToken = default);
}

