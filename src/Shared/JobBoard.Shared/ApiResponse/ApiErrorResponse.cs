using System.Text.Json.Serialization;

namespace JobBoard.Shared.ApiResponse;

public class ApiErrorResponse
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; set; } = false; 

    [JsonPropertyName("message")]
    public string Message { get; set; } 

    [JsonPropertyName("error")]
    public string Error { get; set; }
    
    public ApiErrorResponse(string message, string error)
    {
        Message = message;
        Error =  error;
    }
}