using JobBoard.Recruitment.Infrastructure;
using JobBoard.Shared.Extensions;
using JobBoard.Shared.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace JobBoard.Recruitment.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSerilogLogging();
        builder.Services.AddRecruitmentApi(builder.Configuration);
        
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
        
        app.MapHealthChecks("/api/v1/recruitment/health").AllowAnonymous();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JobBoardRecruitmentContext>().Database;

            foreach (var migration in db.GetPendingMigrations())
            {
                Console.WriteLine($"Applying {migration}...");
            }
    
            await db.MigrateAsync();
        }
        
        app.Run();
    }
}