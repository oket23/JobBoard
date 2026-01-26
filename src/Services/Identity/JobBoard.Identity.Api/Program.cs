using JobBoard.Identity.Api.Endpoints;
using JobBoard.Identity.Infrastructure;
using JobBoard.Shared.Extensions;
using JobBoard.Shared.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace JobBoard.Identity.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSerilogLogging();
        builder.Services.AddIdentityApi(builder.Configuration);
        
        var app = builder.Build();
        
        app.UseSerilogRequestLogging();
        
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        app.UseAuthEndpoints();
        
        app.MapHealthChecks("/api/v1/identity/health").AllowAnonymous();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JobBoardIdentityContext>().Database;

            foreach (var migration in db.GetPendingMigrations())
            {
                Console.WriteLine($"Applying {migration}...");
            }
    
            await db.MigrateAsync();
        }
        
        app.Run();
    }
}