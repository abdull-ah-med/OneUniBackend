using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace OneUniBackend.Extensions;

public static class ControllersExtensions
{
    public static void AddConfiguredControllers(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}
