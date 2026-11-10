namespace DonationHub.Application.Features.Authentication.CurrentProfile;

public sealed record CurrentProfileDto(
    Guid Id,
    string Email,
    string FullName, 
    IEnumerable<string> Roles);
