using JobBoard.Shared.Extensions;
using JobBoard.Shared.Middlewares;
using Serilog;

namespace JobBoard.Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSerilogLogging();
        builder.Services.AddJobBoardGatewayApi(builder.Configuration);
        
        var app = builder.Build();
        
        app.UseSerilogRequestLogging();
        
        //app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/identity/swagger/v1/swagger.json", "Identity API (Auth & Users) v1");
                x.SwaggerEndpoint("/swagger/recruitment/swagger/v1/swagger.json", "Recruitment API (Jobs & Apps) v1");
            });
        }
        
        app.MapHealthChecks("/health");
        
        app.UseCors("AllowAll");
        
        app.UseRateLimiter();
        
        app.MapReverseProxy();
        
        app.Run();
    }
}