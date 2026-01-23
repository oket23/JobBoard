using JobBoard.Identity.Api.Endpoints;
using JobBoard.Shared.Extensions;
using JobBoard.Shared.Middlewares;
using Serilog;

namespace JobBoard.Identity.Api;

public class Program
{
    public static void Main(string[] args)
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
        
        app.UseAuthEndpoints();
        app.MapControllers();
        
        app.MapHealthChecks("/api/v1/identity/health").AllowAnonymous();
        
        app.Run();
    }
}