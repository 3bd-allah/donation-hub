using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.RefreshTokens.Commands.CreateRefreshToken;
using DonationHub.Application.Features.Users.Queries.LoginUser;
using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Login;

public sealed record LoginOrchestrator(string Email, string Password)
    : IRequest<RequestResponse<LoginResponseDto>>;

public class LoginOrchestratorHandler(
    IMediator mediator,
    ITokenService tokenService) : IRequestHandler<LoginOrchestrator, RequestResponse<LoginResponseDto>>
{
    public async Task<RequestResponse<LoginResponseDto>> Handle( 
        LoginOrchestrator request, 
        CancellationToken cancellationToken)
    {

        var result = await mediator.Send(new LoginUserQuery(request.Email, request.Password), cancellationToken);

        if (!result.Success)
        {
            return result.Errors.ToArray();
        }
        var userData = result.Data;


        // generate access token and its expiration date
        var (accessToken, accessTokenExpiration) = tokenService.GenerateAccessToken(userData.Id, userData.Email, userData.Roles);

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


        // create refresh token and store in the DB.
        await mediator.Send(
            new CreateRefreshTokenCommand(userData.Id, tokenHash, refreshTokenExpiration), cancellationToken);
        
        return loginResponse;
    }
}