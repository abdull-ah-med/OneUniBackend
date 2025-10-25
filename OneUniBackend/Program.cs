using Microsoft.EntityFrameworkCore;
using OneUniBackend.Models;
using OneUniBackend.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
// 1. Get the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Create a data source builder
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

dataSourceBuilder.MapEnum<OneUniBackend.Models.UserRole>("user_role");
dataSourceBuilder.MapEnum<OneUniBackend.Models.GenderType>("gender_type");
dataSourceBuilder.MapEnum<OneUniBackend.Models.EducationType>("education_type");
dataSourceBuilder.MapEnum<OneUniBackend.Models.TestType>("test_type");
dataSourceBuilder.MapEnum<OneUniBackend.Models.ApplicationStatus>("application_status");
dataSourceBuilder.MapEnum<OneUniBackend.Models.SessionType>("session_type");
dataSourceBuilder.MapEnum<OneUniBackend.Models.SessionStatus>("session_status");
dataSourceBuilder.MapEnum<OneUniBackend.Models.DocumentType>("document_type");
dataSourceBuilder.MapEnum<OneUniBackend.Models.VerificationStatus>("verification_status");
dataSourceBuilder.MapEnum<OneUniBackend.Models.GuardianRelation>("guardian_relation");
dataSourceBuilder.MapEnum<OneUniBackend.Models.IdDocumentType>("id_document_type");

// 4. Build the data source
var dataSource = dataSourceBuilder.Build();

// 5. Register your DbContext, but now tell it to use thnew data source
builder.Services.AddDbContext<OneUniContext>(options =>
    options.UseNpgsql(dataSource) // <-- Use the pre-configured data source
           .UseSnakeCaseNamingConvention()
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
    var dbContext = scope.ServiceProvider.GetRequiredService<OneUniContext>();
    await dbContext.Database.CanConnectAsync();
    Console.WriteLine("✅ Database connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
}

app.Run();