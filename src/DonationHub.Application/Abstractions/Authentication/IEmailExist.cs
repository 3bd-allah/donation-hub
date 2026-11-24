namespace DonationHub.Application.Abstractions.Authentication;

public interface IEmailExist 
{
    Task<bool> IsEmailExistAsync(string email);
}
