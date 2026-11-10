using DonationHub.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace DonationHub.Infrastructure.Options;

public class SeedAdminOptions
{
    public const string SeedAdminSection = "SeedAdmin";

    [Required, EmailAddress(ErrorMessage = "Admin email must be a valid email address.")]
    public string Email { get; set; } = string.Empty;


    [Required(ErrorMessage = "Admin Full Name must be configured.")]
    public string FullName{ get; set; } = string.Empty;

    [Required(ErrorMessage = "Admin Password must be a configured."), MinLength(WebApplicationConstants.PasswordMinimumLength)]
    public string Password { get; set; } = string.Empty;
}
