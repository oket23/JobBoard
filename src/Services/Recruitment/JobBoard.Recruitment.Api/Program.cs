using JobBoard.Shared.Extensions;

namespace JobBoard.Recruitment.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddHealthChecks()
            .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgres_check", tags: new[] { "db" })
            .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis_check", tags: new[] { "cache" })
            .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq_check", tags: new[] { "broker" });
        
        builder.Services.AddJobBoardOpenTelemetry(builder.Configuration, "Recruitment-Service");
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();
        
        app.MapHealthChecks("/health");
        
        app.Run();
    }
}