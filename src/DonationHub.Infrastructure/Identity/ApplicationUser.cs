using DonationHub.Domain.Constants;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Exceptions;
using DonationHub.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Identity;
public sealed class ApplicationUser : IdentityUser<Guid>, ICreatedAt
{
    public string FullName { get; private set; } = string.Empty;

    public PreferredLanguage PreferredLanguage { get; private set; } 

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsActive { get; private set; }

    private ApplicationUser()
    {
        
    }

    private ApplicationUser(
        string email,
        string fullName,
        PreferredLanguage preferredLanguage)
    {
        Email = email;
        UserName= email;
        FullName = fullName;
        PreferredLanguage = preferredLanguage;
        IsActive = true;
    }

    public static ApplicationUser Create(
        string email, 
        string fullName, 
        string preferredLanguage = WebApplicationConstants.DefaultPreferredLanguage)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentNullException("Email cannot be null or empty.");
        }

        if(string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentNullException("Full name cannot be null or empty.");
        }


        if(string.IsNullOrWhiteSpace(preferredLanguage) || 
            !Enum.TryParse<PreferredLanguage>(preferredLanguage, out var prefResult ))
        {
            throw new ArgumentNullException("Preferred language cannot be null or not valid language.");
        }
        
        return new ApplicationUser(email, fullName, prefResult);
    }

    public static void Deactivate(ApplicationUser user)
    {
        if(user.IsActive)
            user.IsActive = false;
    }
}
