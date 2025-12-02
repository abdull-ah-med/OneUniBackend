using Microsoft.Extensions.DependencyInjection;
using OneUniBackend.Data;

namespace OneUniBackend.Extensions;

public static class DatabaseTestExtensions
{
    public static async Task TestDatabaseConnection(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OneUniDbContext>();

            await db.Database.CanConnectAsync();

            Console.WriteLine("✅ Database connection successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Database connection failed: {ex.Message}");
        }
    }
}
