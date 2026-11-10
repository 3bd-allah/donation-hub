using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;

namespace DonationHub.Application.Abstractions.Authentication;

public interface ILoginService 
{
    /// <summary>
    /// Check the user is exist or active and login him with its credentials if valid
    /// </summary>
    /// <param name="email">email of the user account</param>
    /// <param name="password">password of the user account</param>
    /// <returns>return a propert RequestResponse generic type of the user data</returns>
    Task<RequestResponse<UserDataDto>> LoginAsync(string email, string password);
}
