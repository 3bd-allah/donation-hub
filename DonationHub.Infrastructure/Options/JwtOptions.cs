using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;

namespace DonationHub.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string JwtSection = "Jwt";
    public const string IssuerKey = $"{JwtSection}:{nameof(Issuer)}";
    public const string AudienceKey = $"{JwtSection}:{nameof(Audience)}";
    public const string SecretKey = $"{JwtSection}:{nameof(Secret)}";


    [Required, MinLength(32, ErrorMessage = "Secret key must be at least 32 characters long.")]
    public string Secret { get; set; } = string.Empty; 


    [Required, Url(ErrorMessage = "Issuer must be a valid URL.")]
    public string Issuer { get; set; } = string.Empty; 


    [Required, Url(ErrorMessage = "Audience must be a valid URL.")]
    public string Audience { get; set; } = string.Empty;


    [Required, Range(1, 120, ErrorMessage = "Access token expiration minutes must be between 1 and 120.")]
    public double ExpiryMinutes { get; set; }


    [Required, Range(1, 7, ErrorMessage = "Refresh token expiration days must be between 1 and 7.")]
    public double RefreshTokenExpiryDays { get; set; }
}
