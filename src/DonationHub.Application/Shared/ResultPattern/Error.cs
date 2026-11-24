namespace DonationHub.Application.Shared.ResultPattern;

public sealed record Error (
    string Code,
    string Description,
    ErrorType Type,
    string? Field = null)
{
    public static Error Validation(string code, string description, string? field = null)
        => new(code, description, ErrorType.Validation, field);

    public static Error NotFound(string code, string description)
        => new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description, string? field = null)
        => new(code, description, ErrorType.Conflict, field);

    public static Error Unauthorized(string code, string description)
        => new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description)
        => new(code, description, ErrorType.Forbidden);

    public static Error Unexpected(string code, string description)
        => new(code, description, ErrorType.Failure);
}