using DonationHub.Application.Shared;

namespace DonationHub.Application.Abstractions.Authentication;

public interface IEmailExist 
{
    Task<RequestResponse> IsEmailExistAsync(string email);
}
