using JobBoard.Shared.Extensions;
using JobBoard.Shared.Middlewares;
using Serilog;

namespace JobBoard.Recruitment.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSerilogLogging();
        builder.Services.AddRecruitmentApi(builder.Configuration);
        
        var app = builder.Build();
        
        app.UseSerilogRequestLogging();
        
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        //app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();
        
        app.MapHealthChecks("/api/v1/recruitment/health").AllowAnonymous();
        
        app.Run();
    }
}