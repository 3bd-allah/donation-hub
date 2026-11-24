namespace DonationHub.Application.Shared.ResultPattern;

public class RequestResponse : IRequestResponse<RequestResponse>
{
    public bool Success { get; }
    public IReadOnlyList<Error> Errors { get; }

    protected RequestResponse(bool success, IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var items = errors.ToArray();
        // exceptions thrown here for unexpected behaviors for example:
        // if the success is false => should be at least one error, if not throw exception
        if (items.Any(e => e is null))
            throw new ArgumentException("Errors cannot contain null.", nameof(errors));

        if (success && items.Length > 0)
            throw new InvalidOperationException("A successful result cannot contain errors.");

        if (!success && items.Length == 0)
            throw new InvalidOperationException("A failed result must contain at least one error.");


        if (items.Length > 1 &&
            items.Any(e => e.Type != ErrorType.Validation))
            throw new InvalidOperationException("Multiple errors must be all validation.");

        Success = success;
        Errors = items.AsReadOnly();
    }

    public static RequestResponse Ok()
        => new(true, []);

    public static RequestResponse Fail(params Error[] errors)
        => new(false, errors);

    public static implicit operator RequestResponse (Error[] errors) => Fail(errors);
    public static implicit operator RequestResponse(Error error) => Fail(error);
}

public class RequestResponse<T> : RequestResponse, IRequestResponse<RequestResponse<T>>
{
    private readonly T? _data;

    public T Data => Success ? _data!
        : throw new InvalidOperationException("A failed result does not contain data.");

    private RequestResponse(bool success, T? data, IEnumerable<Error> errors) : base(success, errors)
    {
        _data = data; 
    }

    public static RequestResponse<T> Ok(T data)
        => new(true, data, []);

    public new static RequestResponse<T> Fail(params Error[] errors)
        => new(false, default, errors);

    public static implicit operator RequestResponse<T>(Error[] errors) => Fail(errors);
    public static implicit operator RequestResponse<T>(Error error) => Fail(error);
    public static implicit operator RequestResponse<T>(T data) => Ok(data);

}