using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Primitives;

public abstract class BaseException(string title) : Exception(title)
{
    public abstract string Code { get; init; }
    public abstract ExceptionType ExceptionType { get; init; }
}
