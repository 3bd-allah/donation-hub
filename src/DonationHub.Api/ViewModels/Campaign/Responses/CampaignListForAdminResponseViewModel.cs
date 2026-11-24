namespace DonationHub.Api.ViewModels.Campaign.Responses;

public sealed record CampaignListForAdminResponseViewModel(
    Guid Id,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    decimal GoalAmount,
    decimal RaisedAmount,
    string Currency,
    DateTimeOffset EndDate,
    DateTimeOffset CreatedAt,
    string? BannerFileName,
    string Status,
    byte[] RowVersion,
    Guid CreatedByUserId,
    string CategoryNameAr,
    string CategoryNameEn
    );
