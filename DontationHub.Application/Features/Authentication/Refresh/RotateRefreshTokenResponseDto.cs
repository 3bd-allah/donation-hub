namespace DonationHub.Application.Features.Authentication.Refresh;

public sealed record RotateRefreshTokenResponseDto(
    string NewAccessToken,
    DateTimeOffset AccessTokenExpiration,
    string NewRefreshToken);
