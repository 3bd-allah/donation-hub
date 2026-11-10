using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Infrastructure.Services;

public class TokenService(
    IOptions<JwtOptions> options,
    TimeProvider timeProvider) : ITokenService
{
    private readonly JwtOptions jwt = options.Value;
    public (string token, DateTimeOffset expiresAt) GenerateAccessToken(Guid userId, string userEmail, IEnumerable<string> userRoles)
    {
        var expiresAt = timeProvider.GetUtcNow().AddMinutes(jwt.ExpiryMinutes);

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, userEmail),
                new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            ];

        claims.AddRange(userRoles.Select(role => new Claim("role", role)));

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Audience = jwt.Audience,
            Issuer = jwt.Issuer, 
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(descriptor);

        return (token, expiresAt);
    }

    public (string, DateTimeOffset ) GenerateRefreshToken()
    {
        byte[] randomGeneratedBytes = RandomNumberGenerator.GetBytes(64);
        string refreshToken = Convert.ToBase64String(randomGeneratedBytes);
        DateTimeOffset expiresAt = timeProvider.GetUtcNow().AddDays(jwt.RefreshTokenExpiryDays);
        return (refreshToken, expiresAt); // this sent to the client
    }

    public string HashRefreshToken(string refreshToken)
    {
        byte[] tokenBytes = Convert.FromBase64String(refreshToken);
        var hashedTokenBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashedTokenBytes); // => saved in the database 
    }

    public bool VerifyRefreshToken(string token, string hashedToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);
        ArgumentException.ThrowIfNullOrEmpty(hashedToken);

        byte[] computedHashed = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        byte[] storedHased;
        try
        {
            storedHased = Convert.FromBase64String(hashedToken);
        }catch(FormatException)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(computedHashed, storedHased);
    }
}
