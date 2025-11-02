using OneUni.Entities;

namespace OneUni.Interfaces.Repositories;

public interface IStudentProfileRepository : IGenericRepository<StudentProfile>
{
    Task<StudentProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

