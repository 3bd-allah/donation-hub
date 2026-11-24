using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Campaigns.Common;

public sealed class CampaignErrors
{
    public static Error NotFound =>
        new("Campaign.NotFound", "Campaign ID not found.", ErrorType.NotFound);

    public static Error NotActive=>
        new("Campaign.NotActive", "Campaign is completed or expired.", ErrorType.Conflict);

    public static Error GoalBelowRaised =>
        new("Campaign.GoalBelowRaised", "Goal cannot be lower than raised amount.", ErrorType.UnprocessableEntity);

    public static Error EndDateInPast =>
        new("Campaign.EndDateInPast", "EndDate must be in the future.", ErrorType.UnprocessableEntity);

    public static Error ConcurrencyConflict =>
        new("Campaign.ConcurrencyConflict", "Concurrent update detected.", ErrorType.Conflict);

    public static Error CurrencyConflict =>
        new("Campaign.CurrencyConflict", "Cannot update different currency.", ErrorType.UnprocessableEntity);
}
