using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Authentication;

public class LoginService(UserManager<ApplicationUser> userManager) : ILoginService
{
    public async Task<RequestResponse<UserDataDto>> LoginAsync(string email, string passowrd)
    {
        // check if user is exist 
        var user = await userManager.FindByEmailAsync(email);
        if(user is null)
        {
            return RequestResponse<UserDataDto>.Fail(AuthCodes.InvalidCredentials, new()
            {
                {"email", ["Invalid Credentials"] }
            });
        }

        // check if user is active 
        if(!user.IsActive)
            return RequestResponse<UserDataDto>.Fail(AuthCodes.AccountDeactivated, new()
            {
                {"Account", ["Account Deactivated"] }
            });

        // check user credentials 
        var isValidUser = await userManager.CheckPasswordAsync(user, passowrd);
        if (isValidUser)
        {
            var roles = await userManager.GetRolesAsync(user);
            var loginedUser = new UserDataDto(user.Id, user.Email!, roles);
            return RequestResponse<UserDataDto>.Ok(AuthCodes.Success,loginedUser);
        }
        else
            return RequestResponse<UserDataDto>.Fail(AuthCodes.InvalidCredentials, new()
            {
                { "", ["email or password is incorrect"]}
            });
    }

   
}
