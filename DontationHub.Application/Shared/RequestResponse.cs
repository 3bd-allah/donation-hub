namespace DonationHub.Application.Shared;

public class RequestResponse
{
    public int? Code { get; set; }
    public bool Success{ get; init; }
    public Dictionary<string, string[]>? Errors { get; init; } 

    public static RequestResponse Ok(int? code= null) =>
        new()
        {
            Code = code,
            Success = true,
        };
    public static RequestResponse Fail(int? code=null, Dictionary<string, string[]>? errors = null ) =>
        new()
        {
            Code = code,
            Success = false,
            Errors= errors
        };
}

public class RequestResponse<T> : RequestResponse
{
    public T? Data { get; init; }
    public static RequestResponse<T> Ok(int? code, T data) =>
        new()
        {
            Code = code,
            Success = true,
            Data = data
        };
    public static new RequestResponse<T> Fail(int? code=null ,Dictionary<string, string[]>? errors = null) =>
        new()
        {
            Code = code,
            Success = false,
            Errors = errors
        };
}