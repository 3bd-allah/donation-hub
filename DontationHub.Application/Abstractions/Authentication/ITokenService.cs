namespace DonationHub.Application.Abstractions.Authentication;

public interface ITokenService
{

    (string token, DateTimeOffset expiresAt) GenerateAccessToken(Guid userId, string userEmail, IEnumerable<string> userRoles);

    (string refreshToken, DateTimeOffset expiresAt) GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);

    bool VerifyRefreshToken(string token, string hashedToken);
}
