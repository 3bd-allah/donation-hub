namespace DonationHub.Api.ViewModels.Campaign.Responses;

public sealed record CampaignListResponseViewModel(
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
    byte[] RowVersion,
    Guid CreatedByUserId,
    string CategoryName);
