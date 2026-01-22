using JobBoard.Shared.Extensions;

namespace JobBoard.Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddJobBoardGatewayApi(builder.Configuration);

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();

        app.MapHealthChecks("/health");
        
        app.UseCors("AllowAll");
        
        app.UseRateLimiter();
        
        app.MapReverseProxy();
        
        app.Run();
    }
}