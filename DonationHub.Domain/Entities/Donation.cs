using DonationHub.Domain.Enums;
using DonationHub.Domain.Interfaces;
using DonationHub.Domain.Primitives;
using DonationHub.Domain.ValueObjects;
using System.Drawing;

namespace DonationHub.Domain.Entities;

public sealed class Donation : Entity, IAuditable
{
    // Campaign
    public Guid CampaignId { get; private set; }

    // Application User
    public Guid DonorUserId { get; private set; }
    
    // Donation Options
    public Money Amount { get; private set; } = null!;
    public DonationStatus Status{ get; private set; }
    public DateTimeOffset? CompletedAt{ get; private set; }

    public string? FailureReason { get; private set; }
    public int? ReceiptNumber { get; private set; }
    public string? ReceiptFileName { get; private set; }

    // Audit
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }


    // private pramaterless constructor for EF Core
    private Donation()
    {
        
    }
    private Donation(
        Guid campaignId,
        Guid donorUserId,
        Money amount)
    {
        CampaignId = campaignId;
        DonorUserId = donorUserId;
        Amount = amount;
        Status = DonationStatus.Pending;
    }

    private bool CanChangeStatus(DonationStatus newStatus)
    {
        // Implement logic to determine if the status can be changed
        if(Status == DonationStatus.Succeeded || Status == DonationStatus.Failed)
        {
            return false; // Cannot change status if it's already succeeded or failed
        }
        return true;
    }

    public static Donation Create(Guid campaignId, Guid donorUserId, Money amount)
    {
        // Business rules validation
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Donation amount must be greater than zero.", nameof(amount));
        }

        return new Donation(campaignId, donorUserId, amount);
    }

}
