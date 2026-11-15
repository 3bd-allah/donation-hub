
using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Authentication.Common;

public static class AuthErrors
{
    public static Error InvalidCredentials => 
        new("Auth.InvalidCredentials", "Wrong email or password.", ErrorType.Unauthorized);

    public static Error AccountDeactivated =>
        new("Auth.AccountDeactivated", "Deactivated user logs in or refreshes.", ErrorType.Forbidden);

    public static Error EmailAlreadyExist =>
        new("Auth.EmailAlreadyExist", "Registeration with an existiong emil.", ErrorType.Conflict, "Email");


}

