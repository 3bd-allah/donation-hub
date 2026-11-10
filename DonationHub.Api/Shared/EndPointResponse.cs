namespace DonationHub.Api.Shared;

public class EndPointResponse
{
    public int? StatusCode { get; init; }
    public bool Success { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }

    public static EndPointResponse Ok(int? statusCode) =>
        new()
        {
            StatusCode = statusCode,
            Success = true,
        };
    public static EndPointResponse Fail(int? statusCode, Dictionary<string, string[]>? errors = null) =>
        new()
        {
            StatusCode = statusCode,
            Success = false,
            Errors = errors
        };
}


public class EndPointResponse<T> : EndPointResponse
{
    public T? Data { get; init; }
    public static EndPointResponse<T> Ok(int? statusCode, T data) =>
        new()
        {
            StatusCode = statusCode,
            Success = true,
            Data = data
        };
    public static new EndPointResponse<T> Fail(int? statusCode, Dictionary<string, string[]>? errors = null) =>
        new()
        {
            StatusCode = statusCode,
            Success = false,
            Errors = errors
        };
}