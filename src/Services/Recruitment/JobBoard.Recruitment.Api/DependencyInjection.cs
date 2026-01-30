using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using JobBoard.Recruitment.Application.Services.Jobs;
using JobBoard.Recruitment.Application.Validators.Applications;
using JobBoard.Recruitment.Domain.Abstractions;
using JobBoard.Recruitment.Infrastructure;
using JobBoard.Shared.Caching;
using JobBoard.Shared.Extensions;
using JobBoard.Shared.Filters;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace JobBoard.Recruitment.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddRecruitmentApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<JobBoardRecruitmentContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

        services.AddSingleton<ICacheService, RedisCacheService>();
        
        services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<JobBoardRecruitmentContext>());
        
        services.Scan(scan => scan
            .FromAssemblies(
                typeof(IUnitOfWork).Assembly,          
                typeof(JobBoardRecruitmentContext).Assembly, 
                typeof(JobsService).Assembly           
            )
            .AddClasses(classes => classes.Where(type => 
                type.Name.EndsWith("Service") || 
                type.Name.EndsWith("Repository") ||
                type.Name.EndsWith("Generator")))
            .AsMatchingInterface() 
            .WithScopedLifetime());
        
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!, name: "postgres_check", tags: new[] { "db" })
            .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis_check", tags: new[] { "cache" })
            .AddRabbitMQ(configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq_check", tags: new[] { "broker" });

        services.AddJobBoardOpenTelemetry(configuration, "Recruitment-Service");

        services.AddEndpointsApiExplorer();
        
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateApplicationValidator>();
        
        services.AddControllers(options => 
            {
                options.Filters.Add<FluentValidationFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMQ"));
            });
        });
        
        services.AddSwaggerGen();
        services.AddJobBoardAuthentication(configuration);

        return services;
    }
}