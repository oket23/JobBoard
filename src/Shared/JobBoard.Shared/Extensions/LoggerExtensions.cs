using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Hosting;       
using Serilog;

namespace JobBoard.Shared.Extensions;

public static class LoggerExtensions
{
    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) => 
            ConfigureSerilog(loggerConfiguration, context.Configuration, builder.Environment.ApplicationName));
    }
    
    public static void AddSerilogLogging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) => 
            ConfigureSerilog(loggerConfiguration, builder.Configuration, builder.Environment.ApplicationName));
    }
    
    private static void ConfigureSerilog(LoggerConfiguration loggerConfiguration, IConfiguration configuration, string appName)
    { 
        loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", appName)
            .WriteTo.Console();

        var seqUrl = configuration["Serilog:SeqServerUrl"];
        if (!string.IsNullOrEmpty(seqUrl))
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
        }
    }
}