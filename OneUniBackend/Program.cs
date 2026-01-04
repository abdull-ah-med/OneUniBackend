using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OneUniBackend.Configuration;
using OneUniBackend.Data;
using OneUniBackend.Extensions;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Repositories;
using OneUniBackend.Services;
using DotNetEnv;

// Only load .env file in development (not in Azure/production)
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production" 
    && File.Exists(".env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// 1) Bind configuration
builder.Services.ConfigureEnvironmentVariables(builder.Configuration);

// 2) Add controllers + JSON options
builder.Services.AddConfiguredControllers();

// 3) Add database
builder.Services.AddConfiguredDatabase(builder.Configuration);

// 4) Add repositories
builder.Services.AddRepositories();

// 5) Add services (PasswordService, TokenService, AuthService, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

// 6) Add JWT authentication with cookie extraction
builder.Services.AddJwtAuthentication(builder.Configuration);

// 7) Add Google auth
builder.Services.AddGoogleJwtAuthentication(builder.Configuration);

// 8) Add CORS
builder.Services.AddConfiguredCors(builder.Configuration);

// 9) Swagger (enable later if needed)
builder.Services.AddEndpointsApiExplorer();

// 10) Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<OneUniDbContext>("database");

var app = builder.Build();

// ✔ Correlation + global exception handling early in pipeline
app.UseMiddleware<OneUniBackend.Middleware.CorrelationIdMiddleware>();
app.UseMiddleware<OneUniBackend.Middleware.ExceptionHandlingMiddleware>();

// ✔ HTTPS
app.UseHttpsRedirection();

// ✔ CORS
app.UseCors("AllowCredentials");

// ✔ Auth
app.UseAuthentication();

// ✔ CSRF (double-submit for cookie auth)
app.UseMiddleware<OneUniBackend.Middleware.CsrfProtectionMiddleware>();

app.UseAuthorization();

app.MapControllers();

// ✔ Health check endpoint
app.MapHealthChecks("/health");

// ✔ Test DB connection
await app.TestDatabaseConnection();

app.Run();
