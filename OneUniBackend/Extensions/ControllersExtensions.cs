using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace OneUniBackend.Extensions;

public static class ControllersExtensions
{
    public static void AddConfiguredControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation failed"
                    };
                    problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                    return new ObjectResult(problemDetails)
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });
    }
}
