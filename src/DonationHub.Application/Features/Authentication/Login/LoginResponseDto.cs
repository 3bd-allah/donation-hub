namespace DonationHub.Application.Features.Authentication.Login;

public sealed record LoginResponseDto(
    Guid Id,
    string Email,
    IEnumerable<string> Roles,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);
