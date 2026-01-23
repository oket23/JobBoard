using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JobBoard.Shared.ApiResponse;
using Microsoft.Extensions.Logging; // Додай цей using

namespace JobBoard.Shared.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly ILogger<FluentValidationFilter> _logger;

    public FluentValidationFilter(ILogger<FluentValidationFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage);
            
            var errorString = string.Join("; ", errors);
            
            _logger.LogWarning("Validation failed for {Path}. Errors: {ErrorString}", 
                context.HttpContext.Request.Path, 
                errorString);
            
            var response = new ApiErrorResponse("Validation Failed", errorString);
            
            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}