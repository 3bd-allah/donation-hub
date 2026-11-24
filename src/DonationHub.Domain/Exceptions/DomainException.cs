using DonationHub.Domain.Primitives;

namespace DonationHub.Domain.Exceptions;


public class DomainException(string code, string title, ExceptionType exceptionType) : BaseException(title)
{
    public override string Code { get; init; } = code;
    public override ExceptionType ExceptionType { get; init; } = exceptionType;
}

public enum ExceptionType
{
    Validation,
    NotFound,
    Conflict,
    UnprocessableEntity,
    Failure,
}