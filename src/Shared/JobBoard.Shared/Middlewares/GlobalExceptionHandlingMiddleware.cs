using System.Net;
using System.Text.Json;
using JobBoard.Shared.ApiResponse; // Твій namespace
using JobBoard.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace JobBoard.Shared.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        int statusCode;
        string messageTitle; 
        string errorDetail = exception.Message;

        switch (exception)
        {
            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                messageTitle = "Resource Not Found";
                break;
            
            case BadRequestException:
            case ArgumentException: 
                statusCode = (int)HttpStatusCode.BadRequest;
                messageTitle = "Bad Request";
                break;

            case ConflictException:
                statusCode = (int)HttpStatusCode.Conflict;
                messageTitle = "Conflict";
                break;

            case UnauthorizedException:
            case Microsoft.IdentityModel.Tokens.SecurityTokenException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                messageTitle = "Unauthorized";
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred.");
                statusCode = (int)HttpStatusCode.InternalServerError;
                messageTitle = "Internal Server Error";
                errorDetail = "An unexpected error occurred. Please try again later.";
                break;
        }

        context.Response.StatusCode = statusCode;
        
        var response = new ApiErrorResponse(messageTitle, errorDetail);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, jsonOptions);

        await context.Response.WriteAsync(json);
    }
}