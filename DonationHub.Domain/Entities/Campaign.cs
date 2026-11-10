using DonationHub.Domain.Enums;
using DonationHub.Domain.Interfaces;
using DonationHub.Domain.Primitives;
using DonationHub.Domain.ValueObjects;

namespace DonationHub.Domain.Entities;

public class Campaign : Entity, ISoftDeletable, IAuditable
{
    public string TitleAr { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;

    // Money value objects for GoalAmount and RaisedAmount
    public Money GoalAmount { get; private set; } = null!;
    public Money RaisedAmount { get; private set; } = null!;

    public DateTimeOffset EndDate { get; private set; }

    // Audit properties
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public string? BannerFileName{ get; private set; }

    // Campaign status
    public CampaignStatus Status{ get; private set; }


    // Soft delete properties
    public bool IsDeleted { get ; set ; }
    public DateTimeOffset? DeletedAtUTC { get; set; }

    // Concurrency token for optimistic concurrency control
    public byte[] RowVersion { get; private set; } = [];

    // Application User who created the campaign
    public Guid CreatedByUserId { get; private set; }

    // Category to which the campaign belongs
    public Category Category { get; set; } = null!;
    public Guid CategoryId { get; private set; }

    private Campaign()
    {
        
    }
    private Campaign (Guid id,
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        Money goalAmount,
        DateTimeOffset endDate,
        Guid categoryId,
        string? bannerFileName,
        CampaignStatus status, 
        Guid createdByUserId)
        :base(id)
    {
        TitleAr = titleAr;
        TitleEn = titleEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        GoalAmount = goalAmount;
        EndDate = endDate;
        CategoryId = categoryId;
        BannerFileName = bannerFileName;
        Status = status;
        CreatedByUserId = createdByUserId;
    }

    public static Campaign Create(
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        Money goalAmount,
        DateTimeOffset endDate,
        Guid categoryId,
        string? bannerFileName,
        CampaignStatus status,
        Guid createdByUserId,
        DateTimeOffset utcNow)
    {
        if (status != CampaignStatus.Active)
        {
            throw new ArgumentException("Can't create Campaign with non-active status.", nameof(endDate));
        }

        if(endDate <= utcNow)
        {
            throw new ArgumentException("End date must be in the future.", nameof(endDate));
        }
        // Goal Amount must be greater than zero
        if (goalAmount.Amount <= 0)
        {
            throw new ArgumentException("Goal amount must be greater than zero.", nameof(goalAmount));
        }

        return new Campaign(Guid.CreateVersion7(),
            titleAr,
            titleEn,
            descriptionAr,
            descriptionEn,
            goalAmount,
            endDate,
            categoryId,
            bannerFileName,
            status,
            createdByUserId
            );
        
    }

    public void ChangeEndDate(DateTimeOffset newEndDate, DateTimeOffset utcNow)
    {
        if(Status != CampaignStatus.Active || newEndDate <= utcNow)
        {
            throw new ArgumentException("End date must be in the future for active campaigns.", nameof(newEndDate));
        }

        if (newEndDate != UpdatedAt)
        {
            EndDate = newEndDate;
            UpdatedAt = utcNow;
        }
    }


    public void UpdateStatus(CampaignStatus newStatus, DateTimeOffset utcNow)
    {
        
        if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired)
        {
            throw new InvalidOperationException("Cannot update status for a completed or expired campaign.");
        }
        if (Status != newStatus)
        {
            Status = newStatus;
            UpdatedAt = utcNow;
        }
    }

    public void EnsureCanAcceptDonation(DateTimeOffset utcNow)
    {
        if (Status != CampaignStatus.Active)
        {
            throw new InvalidOperationException("Cannot accept donations for a campaign that is not active.");
        }
        if (EndDate <= utcNow)
        {
            throw new InvalidOperationException("Cannot accept donations for a campaign that has ended.");
        }
    }

    public void UpdateGoalAmount(Money newGoalAmount)
    {
        if (newGoalAmount.Amount <= 0)
        {
            throw new ArgumentException("Goal amount must be greater than zero.", nameof(newGoalAmount));
        }
        if (newGoalAmount != GoalAmount && newGoalAmount > RaisedAmount)
        {
            GoalAmount = newGoalAmount;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
