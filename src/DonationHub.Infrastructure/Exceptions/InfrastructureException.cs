using DonationHub.Domain.Exceptions;
using DonationHub.Domain.Primitives;

namespace DonationHub.Infrastructure.Exceptions;

internal class InfrastructureException(string title) : BaseException(title)
{
    public override string Code { get; init; } = string.Empty;
    public override ExceptionType ExceptionType { get; init; }
}
