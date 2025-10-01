using Microsoft.EntityFrameworkCore;
using Npgsql;
using OneUniBackend.Models;

// CRITICAL: Register PostgreSQL enum mappings FIRST before anything else
NpgsqlConnection.GlobalTypeMapper.MapEnum<UserRole>("user_role");
NpgsqlConnection.GlobalTypeMapper.MapEnum<GenderType>("gender_type");
NpgsqlConnection.GlobalTypeMapper.MapEnum<EducationType>("education_type");
NpgsqlConnection.GlobalTypeMapper.MapEnum<TestType>("test_type");
NpgsqlConnection.GlobalTypeMapper.MapEnum<ApplicationStatus>("application_status");
NpgsqlConnection.GlobalTypeMapper.MapEnum<SessionType>("session_type");
NpgsqlConnection.GlobalTypeMapper.MapEnum<SessionStatus>("session_status");
NpgsqlConnection.GlobalTypeMapper.MapEnum<DocumentType>("document_type");
NpgsqlConnection.GlobalTypeMapper.MapEnum<VerificationStatus>("verification_status");
NpgsqlConnection.GlobalTypeMapper.MapEnum<GuardianRelation>("guardian_relation");
NpgsqlConnection.GlobalTypeMapper.MapEnum<IdDocumentType>("id_document_type");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<OneuniContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            // Map enums in Npgsql options as well
            npgsqlOptions.MapEnum<UserRole>("user_role");
            npgsqlOptions.MapEnum<GenderType>("gender_type");
            npgsqlOptions.MapEnum<EducationType>("education_type");
            npgsqlOptions.MapEnum<TestType>("test_type");
            npgsqlOptions.MapEnum<ApplicationStatus>("application_status");
            npgsqlOptions.MapEnum<SessionType>("session_type");
            npgsqlOptions.MapEnum<SessionStatus>("session_status");
            npgsqlOptions.MapEnum<DocumentType>("document_type");
            npgsqlOptions.MapEnum<VerificationStatus>("verification_status");
            npgsqlOptions.MapEnum<GuardianRelation>("guardian_relation");
            npgsqlOptions.MapEnum<IdDocumentType>("id_document_type");
        })
    .UseSnakeCaseNamingConvention() // Automatically converts C# PascalCase to PostgreSQL snake_case
);


// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Test database connection on startup
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OneuniContext>();
    await dbContext.Database.CanConnectAsync();
    Console.WriteLine("✅ Database connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection failed: {ex.Message}");
}

app.Run();