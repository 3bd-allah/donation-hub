namespace DonationHub.Domain.Entities;

public class ProcessedStipeEvent
{
    public Guid EventId { get; private set; }

    public DateTimeOffset ProcessedAt { get; private set; }

    public string Type { get; private set; } = default!;

}
