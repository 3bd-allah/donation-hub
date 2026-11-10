using DonationHub.Application.Shared;

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
    /// <param name="ct">cancellation token to abort the request, if user cancelled from the client</param>
    /// <returns>return a proper RequestResponse type</returns>
    Task<RequestResponse> RegisterAsync(string email, string fullName, string password, string preferredLanguage, CancellationToken ct);
}
