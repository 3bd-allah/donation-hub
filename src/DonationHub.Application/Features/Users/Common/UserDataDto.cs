namespace DonationHub.Application.Features.Users.Common;

public sealed record UserDataDto(
    Guid Id,
    string FullName,
    string Email,
    IEnumerable<string> Roles,
    bool IsActive);
