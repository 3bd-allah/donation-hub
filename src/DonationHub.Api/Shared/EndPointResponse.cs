namespace DonationHub.Api.Shared;

public class EndpointResponse
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public IReadOnlyList<EndpointError>? Errors { get; init; }
    public string? Message { get; init; } = "";
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;


    public static EndpointResponse OK(int statusCode = 200, string msg = "")
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = msg
        };
    public static EndpointResponse Fail(int statusCode, string msg, IReadOnlyList<EndpointError> errors)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = msg,
            Errors = errors
        };
}


public class EndpointResponse<T> : EndpointResponse
{
    public T? Data { get; init; }
    public static EndpointResponse<T> OK(T data, int statusCode = 200, string msg = "")
        => new()
        {
            Success = true,
            Data = data,
            StatusCode = statusCode,
            Message = msg
        };
    public static new EndpointResponse<T> Fail(int statusCode, string msg, IReadOnlyList<EndpointError> errors)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = msg,
            Errors = errors
        };
}