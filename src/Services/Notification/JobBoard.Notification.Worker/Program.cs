using JobBoard.Notification.Worker.Consumers;
using JobBoard.Notification.Worker.Interfaces;
using JobBoard.Notification.Worker.Services;
using JobBoard.Shared.Extensions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        
        builder.Services.AddScoped<IEmailService, EmailService>();
        
        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisteredConsumer>();
            x.AddConsumer<UserLoggedInConsumer>();           
            x.AddConsumer<ApplicationCreatedConsumer>();  
            x.AddConsumer<ApplicationStatusChangedConsumer>(); 

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitConnectionString);
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
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