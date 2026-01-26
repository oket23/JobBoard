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

    // ОПТИМІЗАЦІЯ: Створюємо налаштування один раз (Singleton), а не на кожен запит
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = false // Для економії трафіку на проді
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
        // Не переписуємо відповідь, якщо вона вже почала відправлятися
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("The response has already started, the exception middleware will not be executed.");
            return;
        }

        (int statusCode, string title, string detail, LogLevel logLevel) = MapException(exception);

        // Логуємо з правильним рівнем (4xx - Warning, 5xx - Error)
        // Додаємо TraceId для легкого пошуку в логах
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        
        if (logLevel == LogLevel.Error)
        {
            _logger.LogError(exception, "Error occurred via Request {TraceId}: {Message}", traceId, exception.Message);
            
            // SECURITY: На проді не показуємо внутрішні деталі 500-ї помилки
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
        
        // Використовуємо оптимізований серіалізатор
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private (int StatusCode, string Title, string Detail, LogLevel LogLevel) MapException(Exception exception)
    {
        return exception switch
        {
            // 404 Not Found
            NotFoundException ex => 
                (StatusCodes.Status404NotFound, "Resource Not Found", ex.Message, LogLevel.Warning),

            // 400 Bad Request (Custom Logic)
            BadRequestException ex => 
                (StatusCodes.Status400BadRequest, "Bad Request", ex.Message, LogLevel.Warning),
            
            // 400 Validation Errors (FluentValidation manual throw)
            ValidationException ex => 
                (StatusCodes.Status400BadRequest, "Validation Failed", FormatValidationErrors(ex), LogLevel.Warning),

            // 400 System JSON Errors (Bad Format, Empty Body, Date Parsing)
            BadHttpRequestException ex => 
                (StatusCodes.Status400BadRequest, "Invalid Request", GetInnerExceptionMessage(ex), LogLevel.Warning),

            // 400 Argument Exceptions
            ArgumentException ex => 
                (StatusCodes.Status400BadRequest, "Invalid Argument", ex.Message, LogLevel.Warning),

            // 409 Conflict
            ConflictException ex => 
                (StatusCodes.Status409Conflict, "Conflict", ex.Message, LogLevel.Warning),

            // 401 Unauthorized
            UnauthorizedException ex => 
                (StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message, LogLevel.Warning),
            
            // 401 Token Issues
            Microsoft.IdentityModel.Tokens.SecurityTokenException ex => 
                (StatusCodes.Status401Unauthorized, "Authentication Failed", ex.Message, LogLevel.Warning),

            // 403 Forbidden
            UnauthorizedAccessException => 
                (StatusCodes.Status403Forbidden, "Forbidden", "Access denied.", LogLevel.Warning),

            // 500 Internal Server Error (Default)
            _ => 
                (StatusCodes.Status500InternalServerError, "Internal Server Error", exception.Message, LogLevel.Error)
        };
    }

    // Допоміжний метод для витягування суті помилки JSON
    private static string GetInnerExceptionMessage(Exception ex)
    {
        if (ex.InnerException != null)
        {
            // Це поверне текст типу "The JSON value could not be converted to System.DateTime..."
            return ex.InnerException.Message;
        }
        return "The request body is missing or invalid JSON.";
    }

    // Якщо FluentValidation кидає помилку вручну, форматуємо її гарно
    private static string FormatValidationErrors(ValidationException ex)
    {
        var errors = ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
        return string.Join("; ", errors);
    }
}