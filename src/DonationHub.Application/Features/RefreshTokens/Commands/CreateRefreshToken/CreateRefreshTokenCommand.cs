using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.RefreshTokens.Commands.CreateRefreshToken;

public sealed record CreateRefreshTokenCommand(Guid UserId, string TokenHash, DateTimeOffset ExpirationDate) 
    : ICommand<RequestResponse<Guid>>;

public sealed class CreateRefreshTokenCommandHandler(
    IBaseRepository<RefreshToken> repository) : ICommandHandler<CreateRefreshTokenCommand, RequestResponse<Guid>>
{
    public async Task<RequestResponse<Guid>> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = RefreshToken.Create(request.UserId, request.TokenHash, request.ExpirationDate);
        repository.Add(refreshToken);
        

        return refreshToken.Id;
    }
}

