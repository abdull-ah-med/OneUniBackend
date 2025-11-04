using OneUniBackend.Entities;

namespace OneUniBackend.Interfaces.Repositories;

public interface IStudentProfileRepository : IGenericRepository<StudentProfile>
{
    Task<StudentProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

