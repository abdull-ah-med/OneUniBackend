using Microsoft.EntityFrameworkCore;
using OneUniBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add DbContext with PostgreSQL and enum mapping
var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection")
);

// Register all PostgreSQL enum types
dataSourceBuilder.MapEnum<UserRole>("user_role");
dataSourceBuilder.MapEnum<GenderType>("gender_type");
dataSourceBuilder.MapEnum<EducationType>("education_type");
dataSourceBuilder.MapEnum<TestType>("test_type");
dataSourceBuilder.MapEnum<ApplicationStatus>("application_status");
dataSourceBuilder.MapEnum<SessionType>("session_type");
dataSourceBuilder.MapEnum<SessionStatus>("session_status");
dataSourceBuilder.MapEnum<DocumentType>("document_type");
dataSourceBuilder.MapEnum<VerificationStatus>("verification_status");
dataSourceBuilder.MapEnum<GuardianRelation>("guardian_relation");
dataSourceBuilder.MapEnum<IdDocumentType>("id_document_type");

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<OneuniContext>(options =>
    options.UseNpgsql(dataSource)
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