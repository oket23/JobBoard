using FluentValidation;
using JobBoard.Shared.ApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; 

namespace JobBoard.Shared.Filters;

public class FluentValidationEndpointFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        
        if (validator is null) return await next(context);
        
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        
        if (argument is null) return await next(context);
        
        var validationResult = await validator.ValidateAsync(argument);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage);
            
            var errorString = string.Join("; ", errors);
            
            var logger = context.HttpContext.RequestServices
                .GetService<ILogger<FluentValidationEndpointFilter<T>>>();

            logger?.LogWarning("Validation failed for {Path}. Errors: {ErrorString}", 
                context.HttpContext.Request.Path, 
                errorString);

            return Results.BadRequest(new ApiErrorResponse("Validation Failed", errorString));
        }

        return await next(context);
    }
}