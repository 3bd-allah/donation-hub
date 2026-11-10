using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Options;

public sealed class DonationOptions
{
    public const string DonationSection = "Donation";

    [Required(ErrorMessage ="{0} is required")]
    [Range(1,10000, ErrorMessage = "{0} should be between 1 and 10000")]
    public decimal MinAmount { get; set; }

    [Required(ErrorMessage ="{0} is required")]
    [Range(1,10000, ErrorMessage = "{0} should be between 1 and 10000")]
    public decimal MaxAmount { get; set; }

    [Required(ErrorMessage ="{0} is required")]
    public double PindingTimeoutMinutes { get; set; }

    
    [Required(ErrorMessage ="{0} is required")]
    [MinLength(3, ErrorMessage = "{0} can't be less than 3 chars.")]
    public string DefaultCurrency { get; set; } = string.Empty;
}
