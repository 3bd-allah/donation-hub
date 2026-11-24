namespace DonationHub.Domain.Exceptions.Campaign;

public static class CampaignExceptions 
{
    public static DomainException InvalidGoalAmount
        => new(
            "Campaign.InvalidGoalAmount",
            "Goal amount cannot be lower than or equal to zero.",
            ExceptionType.UnprocessableEntity);

    public static DomainException GoalBelowRaised
        => new(
            "Campaign.GoalBelowRaised",
            "Goal amount cannot be lower than the raised amount.",
            ExceptionType.UnprocessableEntity);

    public static DomainException InvalidEndDate ( string message = "End date must be in the future.")
        => new("Campaign.EndDate", message, ExceptionType.UnprocessableEntity);

    public static DomainException InActiveCampaign
        => new("Campaign.Status", "Campaign is completed or expired.", ExceptionType.Conflict);


}
