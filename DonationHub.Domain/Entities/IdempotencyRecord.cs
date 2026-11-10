using DonationHub.Domain.Interfaces;

namespace DonationHub.Domain.Entities;

public sealed class IdempotencyRecord : ICreatedAt
{
    public Guid Key { get; private set; }
    public Guid UserId { get; private set; }
    public string RequestHash { get; private set; } = string.Empty;
    public int ResponseStatusCode{ get; private set; }
    public string ResponseBody { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt{ get; private set; }

}
