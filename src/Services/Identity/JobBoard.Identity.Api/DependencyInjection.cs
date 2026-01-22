using JobBoard.Identity.Application.Services.Auth;
using JobBoard.Identity.Application.Services.Auth.Jwt;
using JobBoard.Identity.Application.Services.Users;
using JobBoard.Identity.Domain.Abstractions;
using JobBoard.Identity.Domain.Abstractions.Repositories;
using JobBoard.Identity.Domain.Abstractions.Services;
using JobBoard.Identity.Domain.DTOs;
using JobBoard.Identity.Infrastructure;
using JobBoard.Identity.Infrastructure.Repositories;
using JobBoard.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<JobBoardIdentityContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<JobBoardIdentityContext>());
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        services.AddScoped<IAuthService, AuthService>();
        
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!, name: "postgres_check", tags: new[] { "db" })
            .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis_check", tags: new[] { "cache" })
            .AddRabbitMQ(configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq_check", tags: new[] { "broker" });
        
        services.AddJobBoardOpenTelemetry(configuration, "Identity-Service");
        
        services.AddEndpointsApiExplorer();
        services.AddControllers();
        services.AddSwaggerGen();
        
        services.AddJobBoardAuthentication(configuration);
        
        return services;
    }
}