namespace DonationHub.Api.ViewModels.Authentication.Responses;

public sealed record LoginResponseViewModel(
    Guid Id,
    string Email,
    IEnumerable<string> Roles,
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string RefreshToken);
