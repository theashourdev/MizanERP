namespace MizanERP.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };

    public static ApiResponse<T> Fail(string message, string error) =>
        new() { Success = false, Message = message, Errors = new() { error } };
}

// Non-generic version for operations that return no data
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string message = "Success") =>
        new() { Success = true, Message = message };

    public new static ApiResponse Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };

    public new static ApiResponse Fail(string message, string error) =>
        new() { Success = false, Message = message, Errors = new() { error } };
}