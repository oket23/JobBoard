using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace JobBoard.Shared.Extensions;

public static class LoggerExtensions
{
    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) => 
            ConfigureSerilog(loggerConfiguration, context.Configuration, builder.Environment.ApplicationName, context.HostingEnvironment.EnvironmentName));
    }
    
    public static void AddSerilogLogging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) => 
            ConfigureSerilog(loggerConfiguration, builder.Configuration, builder.Environment.ApplicationName, builder.Environment.EnvironmentName));
    }
    
    private static void ConfigureSerilog(LoggerConfiguration loggerConfiguration, IConfiguration configuration, string appName, string environment)
    { 
        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName)
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithExceptionDetails() 
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .WriteTo.Async(wt => wt.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code
            ));
        
        var seqUrl = configuration["Serilog:SeqServerUrl"];
        
        if (!string.IsNullOrWhiteSpace(seqUrl) && Uri.TryCreate(seqUrl, UriKind.Absolute, out _))
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
        }
        else
        {
            Console.WriteLine($"[Serilog] Seq logging disabled or invalid URL: '{seqUrl}'");
        }
    }
}