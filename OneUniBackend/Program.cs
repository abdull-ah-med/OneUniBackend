using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using OneUni.Interfaces.Repositories;
using OneUni.Interfaces.Services;
using OneUni.Infrastructure.Data;
using OneUni.Infrastructure.Repositories;
using OneUni.Infrastructure.Services;
using DotNetEnv;
using OneUni.Configuration;

// Load .env file
DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Bind JWT settings from environment variables to configuration
builder.Configuration["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY not configured");
builder.Configuration["JwtSettings:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "OneUni";
builder.Configuration["JwtSettings:AccessTokenExpiresInMinutes"] = Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES") ?? "60";
builder.Configuration["JwtSettings:RefreshTokenExpiresInDays"] = Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRES_IN_DAYS") ?? "7";

// Add services to the container
builder.Services.AddControllers();

// Database configuration from environment variables
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "oneuni";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

// Map PostgreSQL enums
dataSourceBuilder.MapEnum<OneUni.Enums.UserRole>("user_role");
dataSourceBuilder.MapEnum<OneUni.Enums.GenderType>("gender_type");
dataSourceBuilder.MapEnum<OneUni.Enums.EducationType>("education_type");
dataSourceBuilder.MapEnum<OneUni.Enums.TestType>("test_type");
dataSourceBuilder.MapEnum<OneUni.Enums.ApplicationStatus>("application_status");
dataSourceBuilder.MapEnum<OneUni.Enums.SessionType>("session_type");
dataSourceBuilder.MapEnum<OneUni.Enums.SessionStatus>("session_status");
dataSourceBuilder.MapEnum<OneUni.Enums.DocumentType>("document_type");
dataSourceBuilder.MapEnum<OneUni.Enums.VerificationStatus>("verification_status");
dataSourceBuilder.MapEnum<OneUni.Enums.GuardianRelation>("guardian_relation");
dataSourceBuilder.MapEnum<OneUni.Enums.IdDocumentType>("id_document_type");

var dataSource = dataSourceBuilder.Build();

// Register DbContext
builder.Services.AddDbContext<OneUniDbContext>(options =>
    options.UseNpgsql(dataSource)
           .UseSnakeCaseNamingConvention());

// Register repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
builder.Services.AddScoped<IMentorRepository, MentorRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IAdmissionCycleRepository, AdmissionCycleRepository>();

// Register services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings")); // Add this
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "OneUni API",
//         Version = "v1",
//         Description = "OneUni University Admission Management System API"
//     });

//     // Add JWT authentication to Swagger
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
//         Name = "Authorization",
//         In = ParameterLocation.Header,
//         Type = SecuritySchemeType.ApiKey,
//         Scheme = "Bearer"
//     });

//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             Array.Empty<string>()
//         }
//     });
// });

var app = builder.Build();

// Configure the HTTP request pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "OneUni API V1");
//         c.RoutePrefix = string.Empty; // Swagger at root
//     });
// }

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Test database connection on startup
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OneUniDbContext>();
    await dbContext.Database.CanConnectAsync();
    Console.WriteLine("✅ Database connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection failed: {ex.Message}");
}

app.Run();
