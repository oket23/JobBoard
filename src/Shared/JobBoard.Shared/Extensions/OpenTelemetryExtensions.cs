using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace JobBoard.Shared.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddJobBoardOpenTelemetry(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        var jaegerEndpoint = configuration["Jaeger:Endpoint"] ?? "http://jaeger:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddMassTransitInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(jaegerEndpoint);
                    });
            });

        return services;
    }
}