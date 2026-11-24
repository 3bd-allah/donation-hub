using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.RefreshTokens.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.RefreshTokens.Queries.GetByTokenHash;

// TODO: we need to return a proper DTO for this refresh [I don't know business requirements until now]
public sealed record GetByTokenHashQuery(string TokenHash) : IQuery<RequestResponse<RefreshToken>>;

public sealed class GetByTokenHashQueryHandler( 
    IBaseRepository<RefreshToken> repository,
    TimeProvider timeProvider) : IQueryHandler<GetByTokenHashQuery, RequestResponse<RefreshToken>>
{
    public async Task<RequestResponse<RefreshToken>> Handle(GetByTokenHashQuery request, CancellationToken cancellationToken)
    {
        var storedRefreshToken = await repository
            .FirstOrDefaultAsync(rt => rt.TokenHash == request.TokenHash, cancellationToken);

        /// check not revoked or check not expired 
        if (storedRefreshToken is null ||   // find it 
            storedRefreshToken.RevokedAt is not null || // checked not revoked 
            storedRefreshToken.ExpiresAt < timeProvider.GetUtcNow() ) // checked not expired
            return RefreshTokenErrors.RefreshTokenInvalid;

        return storedRefreshToken;

    }
}