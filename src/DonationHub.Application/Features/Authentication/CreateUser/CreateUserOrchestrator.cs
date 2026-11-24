using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Features.Users.Commands.CreateUser;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Enums;
using MediatR;

namespace DonationHub.Application.Features.Authentication.CreateUser;

public sealed record CreateUserOrchestrator(
    string Email,
    string FullName,
    string Password,
    string ConfirmPassword,
    string Role,
    string PreferredLanguage = WebApplicationConstants.DefaultPreferredLanguage) : ITransaction<RequestResponse>;


public sealed class CreateUserOrchestratorHandler ( IMediator mediator ) 
    : ITransactionHandler<CreateUserOrchestrator, RequestResponse>
{
    public async Task<RequestResponse> Handle(CreateUserOrchestrator request, CancellationToken cancellationToken)
    {
        var result =  await mediator.Send(
            new CreateNewUserCommand(
                request.Email,
                request.FullName,
                request.Password,
                request.PreferredLanguage,
                Enum.Parse<Roles>(request.Role)), cancellationToken);

        return result.Success ?
            RequestResponse.Ok() :
            result.Errors.ToArray();

    }
}