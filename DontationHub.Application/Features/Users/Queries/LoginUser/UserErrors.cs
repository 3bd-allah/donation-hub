using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Users.Queries.LoginUser;

public class UserErrors
{
    public static Error UserNotFound =>
        new("User.NotFound", "User not found.", ErrorType.NotFound);
}
