using JobBoard.Shared.Extensions;

namespace JobBoard.Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddHealthChecks();
        builder.Services.AddJobBoardOpenTelemetry(builder.Configuration, "Identity-Service");
        
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