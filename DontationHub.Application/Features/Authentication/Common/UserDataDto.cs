namespace DonationHub.Application.Features.Authentication.Common;

public sealed record UserDataDto(Guid Id,
    string Email,
    IEnumerable<string> Roles);
