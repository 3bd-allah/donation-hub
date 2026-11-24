namespace DonationHub.Application.Features.RefreshTokens.Orchestrators.Refresh;

public sealed record RotateRefreshTokenResponseDto(
    string NewAccessToken,
    DateTimeOffset AccessTokenExpiration,
    string NewRefreshToken);
