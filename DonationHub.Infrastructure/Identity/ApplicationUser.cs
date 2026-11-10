using DonationHub.Domain.Constants;
using DonationHub.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Identity;
public sealed class ApplicationUser : IdentityUser<Guid>, ICreatedAt
{
    public string FullName { get; private set; } = string.Empty;

    public string PreferredLanguage { get; private set; } = WebApplicationConstants.DefaultPreferredLanguage;

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsActive { get; private set; }

    private ApplicationUser()
    {
        
    }

    private ApplicationUser(
        string email,
        string fullName,
        string preferredLanguage,
        bool isActice)
    {
        Email = email;
        UserName= email;
        FullName = fullName;
        PreferredLanguage = preferredLanguage;
        IsActive = isActice;
    }

    public static ApplicationUser Create(string email, string fullName, string preferredLanguage = WebApplicationConstants.DefaultPreferredLanguage)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        if(string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name cannot be null or empty.", nameof(fullName));
        }


        if(string.IsNullOrWhiteSpace(preferredLanguage) || preferredLanguage.Length != 2)
        {
            throw new ArgumentException("Preferred language cannot be null or not equal to 2 chars.");
        }
        
        return new ApplicationUser(email, fullName, preferredLanguage, true);
    }

    public static void Deactivate(ApplicationUser user)
    {
        if(user.IsActive)
            user.IsActive = false;
    }
}
