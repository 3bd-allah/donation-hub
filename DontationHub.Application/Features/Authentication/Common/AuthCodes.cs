namespace DonationHub.Application.Features.Authentication.Common;

public static class AuthCodes
{
    public const int InvalidCredentials = 401;
    public const int EmailAlreadyExists = 409;
    public const int RefreshTokenInvalid = 401;
    public const int AccountDeactivated = 403;
    public const int Success = 200 ;
    public const int Created = 201;
    
}
