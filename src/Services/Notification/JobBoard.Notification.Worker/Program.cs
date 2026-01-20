using JobBoard.Shared.Extensions;
using Microsoft.Extensions.Hosting;

namespace JobBoard.Notification.Worker;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Тут пізніше буде підключення MassTransit
        // builder.Services.AddMassTransit(...) 
        
        builder.Services.AddJobBoardOpenTelemetry(builder.Configuration, "Notification");

        var host = builder.Build();

        Console.WriteLine("Notification Worker Started! 🚀 Waiting for messages...");

        // Цей рядок тримає програму запущеною вічно
        
        await host.RunAsync();
    }
}