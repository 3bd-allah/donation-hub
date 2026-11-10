using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Login;

public class LoginCommandHandler(
    ILoginService loginService,
    ITokenService tokenService,
    IBaseRepository<RefreshToken> refreshTokenRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, RequestResponse<LoginResponseDto>>
{
    public async Task<RequestResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Login 
        var userDataResult = await loginService.LoginAsync(request.Email, request.Password);
        
        if (!userDataResult.Success)
            return RequestResponse<LoginResponseDto>.Fail(userDataResult.Code, userDataResult.Errors);

        var userData = userDataResult.Data;

        // generate access token and its expiration date
        var( accessToken, accessTokenExpiration) = tokenService.GenerateAccessToken(userData!.Id, userData.Email, userData.Roles);

        // generate refresh token and its expiration date
        var (refreshToken, refreshTokenExpiration) = tokenService.GenerateRefreshToken();

        var loginResponse = new LoginResponseDto(userData.Id,
            userData.Email,
            userData.Roles,
            accessToken,
            accessTokenExpiration,
            refreshToken,
            refreshTokenExpiration);

        var tokenHash = tokenService.HashRefreshToken(refreshToken);
        var refreshTokenEntity = RefreshToken.Create(userData.Id, tokenHash, refreshTokenExpiration);

        refreshTokenRepository.Add(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return RequestResponse<LoginResponseDto>.Ok(AuthCodes.Success, loginResponse);
    }
}