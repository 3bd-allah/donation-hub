using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.RefreshTokens.Common;

public class RefreshTokenErrors
{
    public static Error RefreshTokenInvalid =>
    new("Auth.RefreshTokenInvalid", "Refresh token unknown, expired or revoked.", ErrorType.Unauthorized);
}
