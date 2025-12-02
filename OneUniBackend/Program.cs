using OneUniBackend.Configuration;
using OneUniBackend.Data;
using OneUniBackend.Extensions;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Repositories;
using OneUniBackend.Services;
using DotNetEnv;

Env.Load();

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
builder.Services.AddConfiguredCors();

// 9) Swagger (enable later if needed)
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ✔ HTTPS
app.UseHttpsRedirection();

// ✔ CORS
app.UseCors("AllowCredentials");

// ✔ Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ✔ Test DB connection
await app.TestDatabaseConnection();

app.Run();
