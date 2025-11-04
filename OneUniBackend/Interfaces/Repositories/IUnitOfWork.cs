namespace OneUniBackend.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUniversityRepository Universities { get; }
    IApplicationRepository Applications { get; }
    IStudentProfileRepository StudentProfiles { get; }
    IMentorRepository Mentors { get; }
    IProgramRepository Programs { get; }
    IDepartmentRepository Departments { get; }
    IAdmissionCycleRepository AdmissionCycles { get; }
    IUserRefreshTokenRepository UserRefreshTokens { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

