namespace DonationHub.Api.ViewModels.Campaign.Responses;

public sealed record CampaignByIdResponseViewModel(
    Guid Id,
    string Title,
    string Description,
    decimal GoalAmount,
    decimal RaisedAmount,
    string Currency,
    DateTimeOffset EndDate,
    DateTimeOffset CreatedAt,
    string? BannerFileName,
    string Status,
    Guid CreatedByUserId,
    string CategoryName,
    double? ProgressPercent,
    uint? DaysLeft
    );
