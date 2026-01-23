using JobBoard.Shared.Extensions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog; 

namespace JobBoard.Notification.Worker;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.AddSerilogLogging();
        
        var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMQ");
        
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitConnectionString);
                
                // MassTransit автоматично пише логи в Serilog/Seq, 
                // тобі не треба налаштовувати RequestLogging.
                cfg.ConfigureEndpoints(context);
            });
        });

        // OpenTelemetry теж можна залишити, якщо там немає Web-специфічних речей
        builder.Services.AddJobBoardOpenTelemetry(builder.Configuration, "Notification");

        var host = builder.Build();
        

        Console.WriteLine($"Notification Worker connecting to RabbitMQ at: {rabbitConnectionString}");
        
        try 
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Worker crashed!");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}