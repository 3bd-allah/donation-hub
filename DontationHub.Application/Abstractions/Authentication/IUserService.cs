using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Features.Authentication.CurrentProfile;

namespace DonationHub.Application.Abstractions.Authentication;

public interface IUserService
{
    /// <summary>
    /// method if the user is active gets its requried data
    /// </summary>
    /// <param name="id">the id of the user you want to check if active or not</param>
    /// <returns>return UserDataDto object of proper data</returns>
    Task<UserDataDto?> IsUserActiveAsync(Guid id);

    Task<CurrentProfileDto?> GetUserProfileAsync(Guid id);
}
