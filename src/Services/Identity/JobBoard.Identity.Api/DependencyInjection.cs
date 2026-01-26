using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using JobBoard.Identity.Application.Services.Users;
using JobBoard.Identity.Application.Validators;
using JobBoard.Identity.Domain.Abstractions;
using JobBoard.Identity.Domain.DTOs;
using JobBoard.Identity.Infrastructure;
using JobBoard.Shared.Extensions;
using JobBoard.Shared.Filters;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<JobBoardIdentityContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<JobBoardIdentityContext>());
        
        services.Scan(scan => scan
            .FromAssemblies(
                typeof(IUnitOfWork).Assembly,          
                typeof(JobBoardIdentityContext).Assembly, 
                typeof(UserService).Assembly           
            )
            .AddClasses(classes => classes.Where(type => 
                type.Name.EndsWith("Service") || 
                type.Name.EndsWith("Repository") ||
                type.Name.EndsWith("Generator")))
            .AsMatchingInterface() 
            .WithScopedLifetime());
        
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!, name: "postgres_check", tags: new[] { "db" })
            .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis_check", tags: new[] { "cache" })
            .AddRabbitMQ(configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq_check", tags: new[] { "broker" });
        
        services.AddJobBoardOpenTelemetry(configuration, "Identity-Service");
        services.AddEndpointsApiExplorer();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
        
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
        
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        
        services.AddSwaggerGen();
        services.AddJobBoardAuthentication(configuration);
        
        return services;
    }
}