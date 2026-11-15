using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Enums;

namespace DonationHub.Application.Features.Users.Commands.CreateUser;

public sealed record CreateNewUserCommand(
    string Email,
    string FullName,
    string Password,
    string PreferredLanguage = WebApplicationConstants.DefaultPreferredLanguage,
    Roles Role = Roles.Donor)
    : ICommand<RequestResponse<Guid>>;

public sealed class CreateNewUserCommandHandler( IRegisterService registerService ) 
    : ICommandHandler<CreateNewUserCommand, RequestResponse<Guid>>
{
    public async Task<RequestResponse<Guid>> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
    {
        var createdUserId = await registerService.RegisterAsync(
            request.Email,
            request.FullName,
            request.Password,
            request.PreferredLanguage,
            request.Role,
            cancellationToken);

        return createdUserId is not null ? 
            createdUserId : 
            AuthErrors.EmailAlreadyExist;
    }
}