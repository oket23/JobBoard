using System.Text.Json.Serialization;

namespace JobBoard.Shared.ApiResponse;

public class ApiSuccessResponse<T>
{
    [JsonPropertyName("success")] public bool IsSuccess { get; set; } = true;

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("data")] public T Data { get; set; }

    public ApiSuccessResponse(T data, string message = "")
    {
        Data = data;
        Message = message;
    }
}

public class ApiSuccessResponse
{
    [JsonPropertyName("success")] 
    public bool IsSuccess { get; set; } = true;

    [JsonPropertyName("message")] 
    public string Message { get; set; }

    public ApiSuccessResponse(string message = "Success")
    {
        Message = message;
    }
}