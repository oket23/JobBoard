using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
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
                    .AddSource(serviceName)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("MassTransit") 
                    .AddNpgsql() 
                    .AddRedisInstrumentation(options => 
                    {
                        options.SetVerboseDatabaseStatements = true;
                    }) 
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(jaegerEndpoint);
                    });
            });

        return services;
    }
}