using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.Users.Common;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Authentication;

public class LoginService(UserManager<ApplicationUser> userManager) : ILoginService
{
    public async Task<UserDataDto?> LoginAsync(string email, string passowrd)
    {
        // check if user is exist 
        var user = await userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            // check user credentials 
            var isValidUser = await userManager.CheckPasswordAsync(user, passowrd);
            if (isValidUser)
            {
                var roles = await userManager.GetRolesAsync(user);
                var loginedUser = new UserDataDto(user.Id, user.FullName, user.Email!, roles, user.IsActive);
                return loginedUser;
            }
            else return null;
        }
        else
            return null;
    }
}
