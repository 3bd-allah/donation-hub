using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Users.Common;

public class UserErrors
{
    public static Error UserNotFound =>
        new("User.NotFound", "User not found.", ErrorType.NotFound);
}
