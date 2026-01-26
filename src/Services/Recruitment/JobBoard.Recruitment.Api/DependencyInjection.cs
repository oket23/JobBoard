using JobBoard.Shared.Extensions;

namespace JobBoard.Recruitment.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddRecruitmentApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!, name: "postgres_check",
                tags: new[] { "db" })
            .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis_check", tags: new[] { "cache" })
            .AddRabbitMQ(configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq_check",
                tags: new[] { "broker" });

        services.AddJobBoardOpenTelemetry(configuration, "Recruitment-Service");

        services.AddEndpointsApiExplorer();
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
}