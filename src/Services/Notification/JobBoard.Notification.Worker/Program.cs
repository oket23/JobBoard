using JobBoard.Shared.Extensions;
using MassTransit;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Hosting;

namespace JobBoard.Notification.Worker;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMQ");
        
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                // Вказуємо хост через рядок підключення
                cfg.Host(rabbitConnectionString);

                // Тут пізніше ти будеш реєструвати Consumer-ів (обробників подій)
                // Наприклад:
                // cfg.ReceiveEndpoint("notification-queue", e => 
                // {
                //     e.ConfigureConsumer<NotificationConsumer>(context);
                // });
            });
        });

        builder.Services.AddJobBoardOpenTelemetry(builder.Configuration, "Notification");

        var host = builder.Build();
        
        Console.WriteLine($"Notification Worker connecting to RabbitMQ at: {rabbitConnectionString}");
        
        await host.RunAsync();
    }
}