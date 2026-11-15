using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.Authentication.CurrentProfile;
using DonationHub.Application.Features.Users.Common;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Authentication;

public class UserService(
    UserManager<ApplicationUser> userManager) : IUserService 
{
    public async Task<UserDataDto?> GetUserByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null )
            return null;

        var userRoles = await userManager.GetRolesAsync(user);
        return new UserDataDto(user.Id, user.Email!, user.FullName, userRoles, user.IsActive);
    }

    public async Task<bool> IsUserActiveAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user is null or { IsActive: false})
            return false;

        return true;
    }
}
