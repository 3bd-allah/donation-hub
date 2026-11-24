using DonationHub.Application.Shared.ResultPattern;


namespace DonationHub.Api.Shared;

public static class ResultMapper
{
    private static int MapStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        ErrorType.PayloadTooLarge => StatusCodes.Status413PayloadTooLarge,
        ErrorType.UnprocessableEntity =>  StatusCodes.Status422UnprocessableEntity,
        ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
        _ => StatusCodes.Status500InternalServerError
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


    public static EndpointResponse<TDestination> Map<TSource, TDestination>(
        RequestResponse<TSource> result,
        Func<TSource, TDestination> mapData,
        string successMessage = "",
        int successStatusCode = 200)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(mapData);
        if (result.Success)
        {
            var data = mapData(result.Data);
            return EndpointResponse<TDestination>.OK(data, successStatusCode, successMessage);
        }

        return EndpointResponse<TDestination>.Fail(
            statusCode: GetFailureStatusCode(result.Errors),
            msg: "Request failed",
            errors: MapErrors(result.Errors));
    }
}
