using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;
using MediatR;

namespace DonationHub.Application.Features.Authentication.Register;

public class RegisterCommandHandler(
    IRegisterService _registerService) : IRequestHandler<RegisterCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Implement the logic for handling the registration command here
        // TODO: remove this it redundant, [but keep it until now for the return type requirement]
        var isEmailExistResult = await _registerService.IsEmailExistAsync(request.Email);

        if (!isEmailExistResult.Success)
            return RequestResponse.Fail(AuthCodes.EmailAlreadyExists, isEmailExistResult.Errors);

        var result = await _registerService.RegisterAsync(
            request.Email,
            request.FullName,
            request.Password,
            request.PreferredLanguage,
            cancellationToken);

        if (result.Success)
            return RequestResponse.Ok(AuthCodes.Created);
        else
            return RequestResponse.Fail(result.Code, result.Errors);
    }
}