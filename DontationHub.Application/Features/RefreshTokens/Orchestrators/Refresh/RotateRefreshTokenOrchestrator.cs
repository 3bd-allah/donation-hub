using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Features.RefreshTokens.Commands.CreateRefreshToken;
using DonationHub.Application.Features.RefreshTokens.Queries.GetByTokenHash;
using DonationHub.Application.Features.Users.Queries.GetById;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.RefreshTokens.Orchestrators.Refresh;

public sealed record RotateRefreshTokenOrchestrator(string RefreshToken) :
    ITransaction<RequestResponse<RotateRefreshTokenResponseDto>>;

public sealed class RotateRefreshTokenHandler(
    IMediator mediator,
    ITokenService tokenService,
    TimeProvider timeProvider) 
    : ITransactionHandler<RotateRefreshTokenOrchestrator, RequestResponse<RotateRefreshTokenResponseDto>>
{
    public async Task<RequestResponse<RotateRefreshTokenResponseDto>> Handle(
        RotateRefreshTokenOrchestrator request, 
        CancellationToken cancellationToken)
    {
        /// receive the incoming token  + hash it 
        /// tokenService.Hash() => hashedToken
        var computedToken = tokenService.HashRefreshToken(request.RefreshToken);

        /// find it & check not revoked & check not expired
        var result  = await mediator.Send(new GetByTokenHashQuery(computedToken), cancellationToken);
        if (!result.Success)
            return result.Errors.ToArray();

        var oldRefreshToken = result.Data;

        var utcNow = timeProvider.GetUtcNow();

        /// check its user is active 
        // get the user from refresh token (userId)
        Guid userId = oldRefreshToken.UserId;

        /// check the user is active 
        var userResult = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
        if (!userResult.Success)
            return userResult.Errors.ToArray();
        var user = userResult.Data;
        if (!user.IsActive)
            return AuthErrors.AccountDeactivated;

        /// create new one (access and refresh)
        /// to create new access token (userid , email, his roles)
        var (newAccessToken, expiresAt) = tokenService.GenerateAccessToken(user.Id, user.Email, user.Roles);
        var newRefreshTokenTuble = tokenService.GenerateRefreshToken();

        // hash new refresh token
        var hashedRefreshToken = tokenService.HashRefreshToken(newRefreshTokenTuble.refreshToken);


        // hash the new refresh token and store in the db and send return the plain one 
        RefreshToken.Create(user.Id, hashedRefreshToken, newRefreshTokenTuble.expiresAt);
        var newRefreshTokenResult  = await mediator.Send(
            new CreateRefreshTokenCommand(user.Id, hashedRefreshToken, newRefreshTokenTuble.expiresAt), cancellationToken);

        if (!newRefreshTokenResult.Success)
            return newRefreshTokenResult.Errors.ToArray();

        // refresh token rotation
        var newRefreshTokenId = newRefreshTokenResult.Data;
        oldRefreshToken.Rotate(newRefreshTokenId, utcNow);

        var refreshTokenRotationResponse = new RotateRefreshTokenResponseDto(
            newAccessToken,
            expiresAt,
            newRefreshTokenTuble.refreshToken);

        return refreshTokenRotationResponse;
    }
}