using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JobBoard.Shared.ApiResponse;

namespace JobBoard.Shared.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage);
            
            var errorString = string.Join("; ", errors);
            
            var response = new ApiErrorResponse("Validation Failed", errorString);
            
            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}