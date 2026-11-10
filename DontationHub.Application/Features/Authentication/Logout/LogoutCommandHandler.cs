using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Logout;

public sealed class LogoutCommandHandler(
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // revoke the refresh token 
        var computedHashedToken = tokenService.HashRefreshToken(request.RefreshToken);

        var storedRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(computedHashedToken, cancellationToken);
        if (storedRefreshToken is null)
            return RequestResponse.Fail(AuthCodes.RefreshTokenInvalid);


        RefreshToken.Revoke(storedRefreshToken, timeProvider.GetUtcNow());

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return RequestResponse.Ok(AuthCodes.Success);
    }
}