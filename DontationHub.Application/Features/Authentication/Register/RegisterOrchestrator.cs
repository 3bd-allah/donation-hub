using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Features.Users.Commands.CreateUser;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Constants;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Register;

public record RegisterOrchestrator(
    string Email,
    string FullName,
    string Password,
    string ConfirmPassword,
    string PreferredLanguage = WebApplicationConstants.DefaultPreferredLanguage)
    : ITransaction<RequestResponse>;

public class RegisterOrchestratorHandler( IMediator mediator )
    : ITransactionHandler<RegisterOrchestrator, RequestResponse>
{
    public async Task<RequestResponse> Handle(RegisterOrchestrator request, CancellationToken cancellationToken)
    {
        // Implement the logic for handling the registration command here

        var result = await mediator.Send(
            new CreateNewUserCommand(request.Email, request.FullName, request.PreferredLanguage), cancellationToken);

        return result.Success ?
            RequestResponse.Ok() :
            result.Errors.ToArray();
    }
}