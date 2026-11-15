using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Features.Users.Common;
using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Users.Queries.LoginUser;

public sealed record LoginUserQuery(string Email, string Password) 
    : IQuery<RequestResponse<UserDataDto>>;

public sealed class LoginUserQueryHandler ( ILoginService loginService ) 
    : IQueryHandler<LoginUserQuery, RequestResponse<UserDataDto>>
{
    public async Task<RequestResponse<UserDataDto>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        // Login 
        var userData = await loginService.LoginAsync(request.Email, request.Password);

        // check if credentials is valid
        if (userData is null)
            return AuthErrors.InvalidCredentials;

        // check if user is active 
        if (!userData.IsActive)
            return AuthErrors.AccountDeactivated;

        return userData;
    }
}