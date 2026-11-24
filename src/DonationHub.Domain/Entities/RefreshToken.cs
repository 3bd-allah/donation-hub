using DonationHub.Domain.Interfaces;
using DonationHub.Domain.Primitives;

namespace DonationHub.Domain.Entities;

public sealed class RefreshToken : Entity, ICreatedAt
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }

    private RefreshToken()
    {
        
    }

    private RefreshToken(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        // business rules here if needed:
        return new RefreshToken(userId, tokenHash, expiresAt);
    }

    public void Rotate(Guid newTokenId, DateTimeOffset utcNow)
    {
        RevokedAt = utcNow;
        ReplacedByTokenId = newTokenId;
    }

    public void Revoke(DateTimeOffset utcNow)
    {
        RevokedAt = utcNow;
    }
}
