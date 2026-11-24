using DonationHub.Application.Features.Users.Common;
using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Abstractions.Authentication;

public interface ILoginService 
{
    /// <summary>
    /// Check the user is exist and login him with its credentials if valid
    /// </summary>
    /// <param name="email">email of the user account</param>
    /// <param name="password">password of the user account</param>
    /// <returns>return a user data of the given email if exist or null</returns>
    Task<UserDataDto?> LoginAsync(string email, string password);
}
