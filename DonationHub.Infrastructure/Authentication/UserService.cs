using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Features.Authentication.CurrentProfile;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Authentication;

public class UserService(
    UserManager<ApplicationUser> userManager) : IUserService
{
    // TODO: business logic in the dataaccess layer are not acceptable [keep it until now]
    public async Task<CurrentProfileDto?> GetUserProfileAsync(Guid id)
    {
        
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null or { IsActive: false })
            return null;

        var userRoles = await userManager.GetRolesAsync(user);
        return new CurrentProfileDto(user.Id, user.Email!, user.FullName, userRoles);
    }

    public async Task<UserDataDto?> IsUserActiveAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());

        if (user is null or { IsActive: false})
            return null;

        var userRoles = await userManager.GetRolesAsync(user);
        return new UserDataDto(user.Id, user.Email!, userRoles);
    }
}
