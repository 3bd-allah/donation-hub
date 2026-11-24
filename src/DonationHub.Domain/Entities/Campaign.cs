using DonationHub.Domain.Enums;
using DonationHub.Domain.Exceptions;
using DonationHub.Domain.Exceptions.Campaign;
using DonationHub.Domain.Exceptions.Money;
using DonationHub.Domain.Interfaces;
using DonationHub.Domain.Primitives;
using DonationHub.Domain.ValueObjects;
using System.Data;

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


    public double ProgressPercent { 
        get
        {
            var percent = Convert.ToDouble((RaisedAmount / GoalAmount).Amount * 100);
            return percent > 100 ? 100 : percent;
        } 
    }

    private Campaign(){} // for ef core

    private Campaign (
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        Money goalAmount,
        DateTimeOffset endDate,
        Guid categoryId,
        string? bannerFileName,
        Guid createdByUserId)
    {
        TitleAr = titleAr;
        TitleEn = titleEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        GoalAmount = goalAmount;
        EndDate = endDate;
        CategoryId = categoryId;
        BannerFileName = bannerFileName;
        Status = CampaignStatus.Active;
        CreatedByUserId = createdByUserId;
        RaisedAmount = new Money(0, GoalAmount.Currency.ToString());
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
        Guid createdByUserId,
        DateTimeOffset utcNow)
    {
        if(endDate <= utcNow)
        {
            throw CampaignExceptions.InvalidEndDate();
        }

        // Goal Amount must be greater than zero
        if (goalAmount.Amount <= 0)
        {
            throw CampaignExceptions.InvalidGoalAmount;
        }

        return new Campaign(
            titleAr,
            titleEn,
            descriptionAr,
            descriptionEn,
            goalAmount,
            endDate,
            categoryId,
            bannerFileName,
            createdByUserId
            );
        
    }

    public void Update(
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        Money goalAmount,
        DateTimeOffset endDate,
        Guid categoryId,
        string? bannerFileName,
        DateTimeOffset utcNow)
    {
        TitleAr = string.IsNullOrEmpty(titleAr) ? TitleAr : titleAr;
        TitleEn = string.IsNullOrEmpty(titleEn) ? TitleEn : titleEn;
        DescriptionAr = string.IsNullOrEmpty(descriptionAr) ? DescriptionAr : descriptionAr;
        DescriptionEn = string.IsNullOrEmpty(descriptionEn) ? DescriptionEn : descriptionEn;

        UpdateGoalAmount(goalAmount);
        ChangeEndDate(endDate, utcNow);

        CategoryId = categoryId;
        BannerFileName = bannerFileName;
        

    }

    public bool CanChangeEndDate(DateTimeOffset newEndDate, DateTimeOffset utcNow)
        => newEndDate > utcNow ;

    public void ChangeEndDate(DateTimeOffset newEndDate, DateTimeOffset utcNow)
    {
        if (Status != CampaignStatus.Active)
            throw CampaignExceptions.InActiveCampaign;

        if (newEndDate <= utcNow)
            throw CampaignExceptions.InvalidEndDate();

        if (newEndDate != EndDate)
            EndDate = newEndDate;
    }


    public void UpdateStatus(CampaignStatus newStatus)
    {
        if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired)
        {
            throw CampaignExceptions.InActiveCampaign;
        }
        if (Status != newStatus)
        {
            Status = newStatus;
        }
    }
    public bool CanUpdateCampaign() 
        => Status == CampaignStatus.Active;

    public void EnsureCanAcceptDonation (DateTimeOffset utcNow)
    {
        if (Status != CampaignStatus.Active)
        {
            throw CampaignExceptions.InActiveCampaign;
        }
        if (EndDate <= utcNow)
        {
            throw CampaignExceptions.InvalidEndDate("Cannot accept donations for a campaign that has ended.");
        }
    }

    public bool IsGoalBelowRaised(Money newGoalAmount) 
        => RaisedAmount > newGoalAmount;

    public void UpdateGoalAmount(Money newGoalAmount)
    {
        if (Status != CampaignStatus.Active)
            throw CampaignExceptions.InActiveCampaign;

        if (newGoalAmount.Amount <= 0)
            throw CampaignExceptions.InvalidGoalAmount;

        if (newGoalAmount < RaisedAmount)
            throw CampaignExceptions.GoalBelowRaised;

        GoalAmount = newGoalAmount;
    }

    public void ApplySuccessfulDonation(Money donationAmount, DateTimeOffset utcNow)
    {
        EnsureCanAcceptDonation(utcNow);

        if (donationAmount.Amount <= 0)
            throw MoneyExceptions.InvalidAmount;

        // The Money operator checks that both currencies match.
        RaisedAmount += donationAmount;

        if (RaisedAmount >= GoalAmount)
            Status = CampaignStatus.Completed;
    }
    
    public uint GetDaysLeft(DateTimeOffset utcNow)
    {
        double remainingDays = (EndDate - utcNow).TotalDays;
        return (uint)Math.Ceiling(Convert.ToDouble(remainingDays));
    }

    public bool CheckCurrency(Money newGoalAmount)
        => newGoalAmount.Currency == RaisedAmount.Currency;
}
