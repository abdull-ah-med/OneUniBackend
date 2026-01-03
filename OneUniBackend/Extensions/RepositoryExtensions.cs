using Microsoft.Extensions.DependencyInjection;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Repositories;

namespace OneUniBackend.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
        services.AddScoped<IUniversityRepository, UniversityRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
        services.AddScoped<IMentorRepository, MentorRepository>();
        services.AddScoped<IProgramRepository, ProgramRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IAdmissionCycleRepository, AdmissionCycleRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IEducationalRecordRepository, EducationalRecordRepository>();
    }
}
