using System.Threading.RateLimiting;
using JobBoard.Shared.Extensions;

namespace JobBoard.Gateway;

public static class DependencyInjection
{
    public static IServiceCollection AddJobBoardGatewayApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging();
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin() 
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
        
        services.AddHealthChecks();
        services.AddJobBoardOpenTelemetry(configuration, "Gateway");
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; 

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ipAddress,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,   
                        Window = TimeSpan.FromSeconds(10),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    });
            });
        });
        return services;
    }
}