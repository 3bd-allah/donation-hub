using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Enums;

namespace DonationHub.Application.Abstractions.Authentication;

public interface IRegisterService : IEmailExist
{
    /// <summary>
    /// create user account if the email entered is not exist
    /// </summary>
    /// <param name="email">email of the user account, must be unique</param>
    /// <param name="fullName">the name of the user account</param>
    /// <param name="password">passowrd of the user account</param>
    /// <param name="preferredLanguage">preferred language of the user account</param>
    /// <param name="role">the role of the registered user</param>
    /// <param name="ct">cancellation token to abort the request, if user cancelled from the client</param>
    /// <returns>return true if the user registered successfully, otherwise false</returns>
    Task<Guid?> RegisterAsync(
        string email,
        string fullName,
        string password,
        string preferredLanguage, 
        Roles role,
        CancellationToken ct);
}
