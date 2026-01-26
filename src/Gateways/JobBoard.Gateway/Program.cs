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
            app.UseSwaggerUI();
        }
        
        app.MapHealthChecks("/health");
        
        app.UseCors("AllowAll");
        
        app.UseRateLimiter();
        
        app.MapReverseProxy();
        
        app.Run();
    }
}