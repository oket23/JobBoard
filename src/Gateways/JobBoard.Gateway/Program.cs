using JobBoard.Shared.Extensions;

namespace JobBoard.Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
        
        builder.Services.AddJobBoardOpenTelemetry(builder.Configuration, "Gateway");
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        // розібратися з авторизацією як її робити і тд
        //app.UseAuthorization();

        app.MapReverseProxy();
        
        app.Run();
    }
}