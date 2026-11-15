using DonationHub.Application.Features.Authentication.CurrentProfile;
using DonationHub.Application.Features.Users.Common;

namespace DonationHub.Application.Abstractions.Authentication;

public interface IUserService
{
    /// <summary>
    /// check if the user is active or not
    /// </summary>
    /// <param name="id">the id of the user you want to check if active or not</param>
    /// <returns>return true if the user is active, otherwise false.</returns>
    Task<bool> IsUserActiveAsync(Guid id);

    /// <summary>
    /// get the user profile data 
    /// </summary>
    /// <param name="id">the identifier of the user to get its data</param>
    /// <returns>the data of the user profile</returns>
    Task<UserDataDto?> GetUserByIdAsync(Guid id);


}
