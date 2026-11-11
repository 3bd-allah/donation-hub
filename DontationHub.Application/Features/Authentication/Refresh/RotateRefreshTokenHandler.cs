using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace DonationHub.Application.Features.Authentication.Refresh;

public sealed class RotateRefreshTokenHandler(
    ITokenService tokenService,
    TimeProvider timeProvider,
    IUserService userActive,
    IUnitOfWork unitOfWork,
    IBaseRepository<RefreshToken> repository) : IRequestHandler<RotateRefreshTokenCommand, RequestResponse<RotateRefreshTokenResponseDto>>
{
    public async Task<RequestResponse<RotateRefreshTokenResponseDto>> Handle(RotateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        /// receive the incoming token  + hash it 
        /// tokenService.Hash() => hashedToken
        var computedToken = tokenService.HashRefreshToken(request.RefreshToken);

        /// find it 
        // getByTokenHash => refresh Repo
        //var oldRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(computedToken, cancellationToken);
        var oldRefreshToken = await repository.GetAll().AsTracking()
            .FirstOrDefaultAsync(rt => rt.TokenHash == computedToken, cancellationToken);

        if (oldRefreshToken is null || 
            oldRefreshToken.RevokedAt is not null)
        {
            return RequestResponse<RotateRefreshTokenResponseDto>.Fail(AuthCodes.RefreshTokenInvalid,
                new()
                {
                    { "refreshToken", ["RefreshTokenInvlid"]}
                });
        }

        /// check not revoked or check not expired 
        var utcNow = timeProvider.GetUtcNow();
        if (oldRefreshToken?.ExpiresAt < utcNow)
        {
            return RequestResponse<RotateRefreshTokenResponseDto>.Fail(AuthCodes.RefreshTokenInvalid,
                new()
                {
                    { "refreshToken", ["RefreshTokenInvlid"]}
                });
        }

        /// check its user is active 
        // get the user from refresh token (userId)
        Guid userId = oldRefreshToken!.UserId;

        /// check the user is active 
        var user = await userActive.IsUserActiveAsync(userId);
        if (user is null)
            return RequestResponse<RotateRefreshTokenResponseDto>.Fail(AuthCodes.AccountDeactivated);

        /// create new one (access and refresh)
        /// to create new access token (userid , email, his roles)

        var (newAccessToken, expiresAt) = tokenService.GenerateAccessToken(user.Id, user.Email, user.Roles);
        var newRefreshTokenTuble = tokenService.GenerateRefreshToken();

        var refreshTokenRotationResponse = new RotateRefreshTokenResponseDto(
            newAccessToken, 
            expiresAt, 
            newRefreshTokenTuble.refreshToken);

        var hashedRefreshToken = tokenService.HashRefreshToken(newRefreshTokenTuble.refreshToken);
        var newRefreshToken = RefreshToken.Create(user.Id, hashedRefreshToken, newRefreshTokenTuble.expiresAt);

        // refresh token rotation
        RefreshToken.Rotation(oldRefreshToken, newRefreshToken, utcNow);
        
        repository.Add(newRefreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<RotateRefreshTokenResponseDto>.Ok(AuthCodes.Success, refreshTokenRotationResponse);
    }
}