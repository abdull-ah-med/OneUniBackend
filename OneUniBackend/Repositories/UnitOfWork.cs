using Microsoft.EntityFrameworkCore.Storage;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Data;

namespace OneUniBackend.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OneUniDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(OneUniDbContext context)
    {
        _context = context;
        
        Users = new UserRepository(_context);
        Universities = new UniversityRepository(_context);
        Applications = new ApplicationRepository(_context);
        StudentProfiles = new StudentProfileRepository(_context);
        Mentors = new MentorRepository(_context);
        Programs = new ProgramRepository(_context);
        Departments = new DepartmentRepository(_context);
        AdmissionCycles = new AdmissionCycleRepository(_context);
        UserRefreshTokens = new UserRefreshTokenRepository(_context);
        UserExternalLoginRepository = new UserExternalLoginRepository(_context);
    }

    public IUserRepository Users { get; }
    public IUniversityRepository Universities { get; }
    public IApplicationRepository Applications { get; }
    public IStudentProfileRepository StudentProfiles { get; }
    public IMentorRepository Mentors { get; }
    public IProgramRepository Programs { get; }
    public IDepartmentRepository Departments { get; }
    public IAdmissionCycleRepository AdmissionCycles { get; }
    public IUserRefreshTokenRepository UserRefreshTokens { get; }
    public IUserExternalLoginRepository UserExternalLoginRepository { get; }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

