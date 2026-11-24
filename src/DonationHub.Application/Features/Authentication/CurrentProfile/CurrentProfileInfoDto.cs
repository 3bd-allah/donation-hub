namespace DonationHub.Application.Features.Authentication.CurrentProfile;

public sealed record CurrentProfileInfoDto(
    Guid Id,
    string Email,
    string FullName, 
    IEnumerable<string> Roles);
