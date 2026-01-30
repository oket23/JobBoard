using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using JobBoard.Shared.ApiResponse;
using JobBoard.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting; // Для перевірки середовища
using Microsoft.Extensions.Logging;

namespace JobBoard.Shared.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;


    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = false 
    };

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("The response has already started, the exception middleware will not be executed.");
            return;
        }

        (int statusCode, string title, string detail, LogLevel logLevel) = MapException(exception);
        
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        
        if (logLevel == LogLevel.Error)
        {
            _logger.LogError(exception, "Error occurred via Request {TraceId}: {Message}", traceId, exception.Message);
            
            if (!_env.IsDevelopment())
            {
                detail = $"An unexpected error occurred. Trace ID: {traceId}";
            }
        }
        else
        {
            _logger.LogWarning("Client Error via Request {TraceId}: {Message}", traceId, exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ApiErrorResponse(title, detail);
        
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private (int StatusCode, string Title, string Detail, LogLevel LogLevel) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException ex => 
                (StatusCodes.Status404NotFound, "Resource Not Found", ex.Message, LogLevel.Warning),
            
            BadRequestException ex => 
                (StatusCodes.Status400BadRequest, "Bad Request", ex.Message, LogLevel.Warning),
            
            ValidationException ex => 
                (StatusCodes.Status400BadRequest, "Validation Failed", FormatValidationErrors(ex), LogLevel.Warning),
            
            BadHttpRequestException ex => 
                (StatusCodes.Status400BadRequest, "Invalid Request", GetInnerExceptionMessage(ex), LogLevel.Warning),
            
            ArgumentException ex => 
                (StatusCodes.Status400BadRequest, "Invalid Argument", ex.Message, LogLevel.Warning),
            
            ConflictException ex => 
                (StatusCodes.Status409Conflict, "Conflict", ex.Message, LogLevel.Warning),
            
            UnauthorizedException ex => 
                (StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message, LogLevel.Warning),
            
            Microsoft.IdentityModel.Tokens.SecurityTokenException ex => 
                (StatusCodes.Status401Unauthorized, "Authentication Failed", ex.Message, LogLevel.Warning),
            
            UnauthorizedAccessException => 
                (StatusCodes.Status403Forbidden, "Forbidden", "Access denied.", LogLevel.Warning),
            
            _ => 
                (StatusCodes.Status500InternalServerError, "Internal Server Error", exception.Message, LogLevel.Error)
        };
    }
    
    private static string GetInnerExceptionMessage(Exception ex)
    {
        if (ex.InnerException != null)
        {
            return ex.InnerException.Message;
        }
        return "The request body is missing or invalid JSON.";
    }
    
    private static string FormatValidationErrors(ValidationException ex)
    {
        var errors = ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
        return string.Join("; ", errors);
    }
}