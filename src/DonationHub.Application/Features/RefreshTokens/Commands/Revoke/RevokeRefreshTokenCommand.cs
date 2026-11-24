using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.RefreshTokens.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.RefreshTokens.Commands.Revoke;

public sealed record RevokeRefreshTokenCommand(string TokenHash) : ICommand<RequestResponse>;

public sealed class RevokeRefreshTokenCommandHandler( 
    IBaseRepository<RefreshToken> repository,
    TimeProvider timeProvider) : ICommandHandler<RevokeRefreshTokenCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedRefreshToken = await repository
            .FirstOrDefaultAsync(rt => rt.TokenHash == request.TokenHash && rt.RevokedAt == null, cancellationToken);

        if (storedRefreshToken is null)
            return RefreshTokenErrors.RefreshTokenInvalid;

        storedRefreshToken.Revoke(timeProvider.GetUtcNow());

        return RequestResponse.Ok();
    }
}