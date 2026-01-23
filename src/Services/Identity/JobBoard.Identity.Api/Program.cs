using JobBoard.Shared.Middlewares;

namespace JobBoard.Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddIdentityApi(builder.Configuration);
        
        var app = builder.Build();
        
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.MapHealthChecks("/api/v1/identity/health").AllowAnonymous();
        
        app.Run();
    }
}