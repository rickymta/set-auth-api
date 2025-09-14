using Newtonsoft.Json;

namespace Set.Auth.Api.Models.Response;

public class ResponseResult<T>
    where T : class
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("data")]
    public T? Data { get; set; }

    [JsonProperty("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    public ResponseResult() { }

    public ResponseResult(T data, string message = "Success")
    {
        Success = true;
        Data = data;
        Message = message;
    }

    public ResponseResult(string message, Dictionary<string, string[]>? errors = null)
    {
        Success = false;
        Message = message;
        Errors = errors;
    }

    public static ResponseResult<T> SuccessResponse(T data, string message = "Success")
    {
        return new ResponseResult<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ResponseResult<T> FailureResponse(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ResponseResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class ResponseResult
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    // Constructor mặc định
    public ResponseResult() { }

    // Constructor tiện ích
    public ResponseResult(bool success, string message, Dictionary<string, string[]>? errors = null)
    {
        Success = success;
        Message = message;
        Errors = errors;
    }

    // Phương thức tạo response thành công
    public static ResponseResult SuccessResponse(string message = "Success")
    {
        return new ResponseResult
        {
            Success = true,
            Message = message
        };
    }

    // Phương thức tạo response thất bại
    public static ResponseResult FailureResponse(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ResponseResult
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
