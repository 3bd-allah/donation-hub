using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.RefreshTokens.Commands.Revoke;
using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Logout;

public sealed record LogoutOrchestrator(string RefreshToken) : IRequest<RequestResponse>;


public sealed class LogoutOrchestratorHandler(
    ITokenService tokenService,
    IMediator mediator) : IRequestHandler<LogoutOrchestrator, RequestResponse>
{
    public async Task<RequestResponse> Handle(LogoutOrchestrator request, CancellationToken cancellationToken)
    {
        // revoke the refresh token 
        var computedHashedToken = tokenService.HashRefreshToken(request.RefreshToken);


        var result = await mediator.Send(new RevokeRefreshTokenCommand(computedHashedToken), cancellationToken);
        if (!result.Success)
            return result.Errors.ToArray();

        return RequestResponse.Ok();
    }
}