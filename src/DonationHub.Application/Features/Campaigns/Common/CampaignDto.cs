using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;

namespace DonationHub.Application.Features.Campaigns.Common;

public sealed record CampaignDto
{
    public Guid Id { get; set; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Money GoalAmount { get; init; } = default!;
    public Money RaisedAmount { get; init; } = default!;
    public DateTimeOffset EndDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public string? BannerFileName { get; init; }
    public CampaignStatus Status { get; init; }
    public byte[] RowVersion { get; init; } = default!;
    public Guid CreatedByUserId { get; init; }
    public string CategoryName{ get; init; } = string.Empty;
    public double ProgressPercent { get; init; }
    public uint DaysLeft { get; init; }
}

