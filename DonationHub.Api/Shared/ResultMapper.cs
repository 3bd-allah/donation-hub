using DonationHub.Application.Shared.ResultPattern;


namespace DonationHub.Api.Shared;

public static class ResultMapper
{
    private static int MapStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => 400,
        ErrorType.NotFound => 404,
        ErrorType.Conflict => 409,
        ErrorType.Unauthorized => 401,
        ErrorType.Forbidden => 403,
        ErrorType.Failure => 500,
        ErrorType.PayloadTooLarge => 413,
        ErrorType.UnprocessableEntity =>  422,
        ErrorType.TooManyRequests => 429,
        _ => 500
    };

    private static int GetFailureStatusCode(IReadOnlyList<Error> errors)
    {
        if (errors.Count == 0)
            throw new InvalidOperationException(
            "A failed result must contain an error.");

        if (errors.Count > 1 &&
            errors.Any(e => e.Type != ErrorType.Validation))
            throw new InvalidOperationException("Only validation errors may be aggregated.");

        return MapStatusCode(errors[0].Type);
    }

    private static EndpointError[] MapErrors(IReadOnlyList<Error> errors)
        => errors.Select(e => new EndpointError(
                    Code: e.Code,
                    Description: e.Description,
                    Type: e.Type.ToString(),
                    Field: e.Field))
                .ToArray();


    public static EndpointResponse Map(
        RequestResponse result,
        string successMessage = "", 
        int successStatusCode = 200)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Success)
            return EndpointResponse.OK(successStatusCode, successMessage);

        return EndpointResponse.Fail(
            statusCode: GetFailureStatusCode(result.Errors),
            msg: "Request failed",
            errors: MapErrors(result.Errors));
    }


    public static EndpointResponse<T> Map<T>(
        RequestResponse<T> result,
        string successMessage = "",
        int successStatusCode = 200)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Success)
            return EndpointResponse<T>.OK(result.Data, successStatusCode, successMessage);

        return EndpointResponse<T>.Fail(
            statusCode: GetFailureStatusCode(result.Errors),
            msg: "Request failed",
            errors: MapErrors(result.Errors));
    }
}
