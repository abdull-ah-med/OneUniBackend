using Microsoft.EntityFrameworkCore;
using OneUni.Interfaces.Repositories;
using OneUni.Entities;
using OneUni.Infrastructure.Data;

namespace OneUni.Infrastructure.Repositories;

public class StudentProfileRepository : GenericRepository<StudentProfile>, IStudentProfileRepository
{
    public StudentProfileRepository(OneUniDbContext context) : base(context)
    {
    }

    public async Task<StudentProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sp => sp.User)
            .FirstOrDefaultAsync(sp => sp.UserId == userId, cancellationToken);
    }
}

