namespace DonationHub.Api.ViewModels.Campaign.Requests;

public sealed record UpdateCampaignRequestViewModel(
        string TitleAr,
        string TitleEn,
        string DescriptionAr,
        string DescriptionEn,
        decimal GoalAmount,
        string Currency,
        DateTimeOffset EndDate,
        Guid CategoryId,
        string? BannerFileName,
        byte[] RowVersion
    );