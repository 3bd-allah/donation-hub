using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Exceptions;
using DonationHub.Domain.Exceptions.Money;
using DonationHub.Domain.ValueObjects;
using FluentAssertions;


namespace DonationHub.Domain.Test.Entities;
public class CampaignTests
{

    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    private static readonly DateTimeOffset FutureDate =
        Now.AddDays(10);

    private static readonly Guid CategoryId =
        Guid.NewGuid();

    private static readonly Guid CreatedByUserId =
        Guid.NewGuid();

    public static Campaign CreateCampaign(
        decimal goalAmount = 100m,
        DateTimeOffset? endDate = null)
    {
        return Campaign.Create(
            titleAr: "حملة تبرعات",
            titleEn: "Donation Campaign",
            descriptionAr: "وصف الحملة",
            descriptionEn: "Campaign description",
            goalAmount: new Money(goalAmount, "USD"),
            endDate: endDate ?? FutureDate,
            categoryId: CategoryId,
            bannerFileName: null,
            createdByUserId: CreatedByUserId,
            utcNow: Now);
    }

    private static Money CreateMoney(decimal amount)
        => new(amount, "USD");

    private static DomainException GetDomainException( Action action )
    {
        return action.Should()
            .Throw<DomainException>()
            .Which;
    }

    // --------------------------------------------------
    // 1. Goal amount invariant
    // --------------------------------------------------

    [Theory]
    [InlineData(0)]
    public void Create_ShouldThrow_WhenGoalAmountIsZero( decimal goalAmount )
    {
        // Arrange
        var money = CreateMoney(goalAmount);

        // Act
        Action act = () => CreateCampaign(
            goalAmount: money.Amount);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should()
            .Be("Campaign.InvalidGoalAmount");

        exception.ExceptionType.Should()
            .Be(ExceptionType.UnprocessableEntity);
    }

    [Theory]
    [InlineData(-100)]
    public void Create_ShouldThrow_WhenGoalAmountIsNegative(decimal goalAmount)
    {
        // Act
        Action act = () => CreateMoney(goalAmount);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should()
            .Be("Money.InvalidAmount");

        exception.ExceptionType.Should()
            .Be(ExceptionType.UnprocessableEntity);
    }

    [Fact]
    public void Create_ShouldSucceed_WhenGoalAmountIsPositive()
    {
        // Arrange
        const decimal goalAmount = 100m;

        // Act
        var campaign = CreateCampaign(goalAmount);

        // Assert
        campaign.GoalAmount.Amount.Should().Be(goalAmount);
        campaign.RaisedAmount.Amount.Should().Be(0);
        campaign.Status.Should().Be(CampaignStatus.Active);
    }

    // --------------------------------------------------
    // 2. End date invariant
    // --------------------------------------------------

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_ShouldThrow_WhenEndDateIsNotInFuture(
        int daysFromNow)
    {
        // Arrange
        var endDate = Now.AddDays(daysFromNow);

        // Act
        Action act = () => CreateCampaign(
            endDate: endDate);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.EndDate");
        exception.ExceptionType.Should()
            .Be(ExceptionType.UnprocessableEntity);
    }

    [Fact]
    public void Create_ShouldSucceed_WhenEndDateIsInFuture()
    {
        // Arrange
        var endDate = Now.AddDays(5);

        // Act
        var campaign = CreateCampaign(endDate: endDate);

        // Assert
        campaign.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void ChangeEndDate_ShouldUpdateEndDate_WhenNewDateIsInFuture()
    {
        // Arrange
        var campaign = CreateCampaign();
        var newEndDate = Now.AddDays(20);

        // Act
        campaign.ChangeEndDate(newEndDate, Now);

        // Assert
        campaign.EndDate.Should().Be(newEndDate);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void ChangeEndDate_ShouldThrow_WhenNewDateIsNotInFuture(
        int daysFromNow)
    {
        // Arrange
        var campaign = CreateCampaign();
        var newEndDate = Now.AddDays(daysFromNow);

        // Act
        Action act = () =>
            campaign.ChangeEndDate(newEndDate, Now);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.EndDate");
    }

    // --------------------------------------------------
    // 3. Donation eligibility invariant
    // --------------------------------------------------

    [Fact]
    public void EnsureCanAcceptDonation_ShouldNotThrow_WhenCampaignIsActiveAndNotEnded()
    {
        // Arrange
        var campaign = CreateCampaign();

        // Act
        Action act = () =>
            campaign.EnsureCanAcceptDonation(Now);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanAcceptDonation_ShouldThrow_WhenCampaignIsCompleted()
    {
        // Arrange
        var campaign = CreateCampaign();
        campaign.UpdateStatus(CampaignStatus.Completed);

        // Act
        Action act = () =>
            campaign.EnsureCanAcceptDonation(Now);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.Status");
        exception.ExceptionType.Should()
            .Be(ExceptionType.Conflict);
    }

    [Fact]
    public void EnsureCanAcceptDonation_ShouldThrow_WhenCampaignIsExpired()
    {
        // Arrange
        var campaign = CreateCampaign();
        campaign.UpdateStatus(CampaignStatus.Expired);

        // Act
        Action act = () =>
            campaign.EnsureCanAcceptDonation(Now);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.Status");
    }

    [Fact]
    public void EnsureCanAcceptDonation_ShouldThrow_WhenEndDateHasPassed()
    {
        // Arrange
        var campaign = CreateCampaign(
            endDate: Now.AddDays(1));

        var afterEndDate = Now.AddDays(2);

        // Act
        Action act = () =>
            campaign.EnsureCanAcceptDonation(afterEndDate);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.EndDate");
    }

    // --------------------------------------------------
    // 4. Goal cannot be lowered below RaisedAmount
    // --------------------------------------------------

    [Fact]
    public void UpdateGoalAmount_ShouldUpdate_WhenNewGoalIsValid()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 100m);
        var newGoal = CreateMoney(200m);

        // Act
        campaign.UpdateGoalAmount(newGoal);

        // Assert
        campaign.GoalAmount.Amount.Should().Be(200m);
    }

    [Fact]
    public void UpdateGoalAmount_ShouldThrow_WhenNewGoalIsZero()
    {
        // Arrange
        var campaign = CreateCampaign();
        var newGoal = CreateMoney(0m);

        // Act
        Action act = () => campaign.UpdateGoalAmount(newGoal);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.InvalidGoalAmount");
    }

    [Fact]
    public void UpdateGoalAmount_ShouldThrow_WhenNewGoalIsBelowRaisedAmount()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 200m);

        campaign.ApplySuccessfulDonation(
            CreateMoney(100m),
            Now);

        var newGoal = CreateMoney(50m);

        // Act
        Action act = () => campaign.UpdateGoalAmount(newGoal);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.GoalBelowRaised");
        exception.ExceptionType.Should()
            .Be(ExceptionType.UnprocessableEntity);
    }

    [Fact]
    public void UpdateGoalAmount_ShouldAllowGoalEqualToRaisedAmount()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 200m);

        campaign.ApplySuccessfulDonation(
            CreateMoney(100m),
            Now);

        var newGoal = CreateMoney(100m);

        // Act
        campaign.UpdateGoalAmount(newGoal);

        // Assert
        campaign.GoalAmount.Amount.Should().Be(100m);
    }

    // --------------------------------------------------
    // 5. Final state immutability
    // --------------------------------------------------

    [Theory]
    [InlineData(CampaignStatus.Completed)]
    [InlineData(CampaignStatus.Expired)]
    public void ChangeEndDate_ShouldThrow_WhenCampaignIsInFinalState(
        CampaignStatus finalStatus)
    {
        // Arrange
        var campaign = CreateCampaign();
        campaign.UpdateStatus(finalStatus);

        // Act
        Action act = () =>
            campaign.ChangeEndDate(Now.AddDays(30), Now);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.Status");
    }

    [Theory]
    [InlineData(CampaignStatus.Completed)]
    [InlineData(CampaignStatus.Expired)]
    public void UpdateGoalAmount_ShouldThrow_WhenCampaignIsInFinalState(
        CampaignStatus finalStatus)
    {
        // Arrange
        var campaign = CreateCampaign();
        campaign.UpdateStatus(finalStatus);

        // Act
        Action act = () =>
            campaign.UpdateGoalAmount(CreateMoney(200m));

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.Status");
    }

    [Theory]
    [InlineData(CampaignStatus.Completed)]
    [InlineData(CampaignStatus.Expired)]
    public void UpdateStatus_ShouldThrow_WhenCampaignIsAlreadyInFinalState(
        CampaignStatus finalStatus)
    {
        // Arrange
        var campaign = CreateCampaign();
        campaign.UpdateStatus(finalStatus);

        var anotherStatus =
            finalStatus == CampaignStatus.Completed
                ? CampaignStatus.Expired
                : CampaignStatus.Completed;

        // Act
        Action act = () => campaign.UpdateStatus(anotherStatus);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Campaign.Status");
    }

    // --------------------------------------------------
    // 6. Successful donation and automatic completion
    // --------------------------------------------------

    [Fact]
    public void ApplySuccessfulDonation_ShouldIncreaseRaisedAmount()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 200m);

        // Act
        campaign.ApplySuccessfulDonation(
            CreateMoney(50m),
            Now);

        // Assert
        campaign.RaisedAmount.Amount.Should().Be(50m);
        campaign.Status.Should().Be(CampaignStatus.Active);
    }

    [Fact]
    public void ApplySuccessfulDonation_ShouldCompleteCampaign_WhenGoalIsReached()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 100m);

        // Act
        campaign.ApplySuccessfulDonation(
            CreateMoney(100m),
            Now);

        // Assert
        campaign.RaisedAmount.Amount.Should().Be(100m);
        campaign.Status.Should().Be(CampaignStatus.Completed);
    }

    [Fact]
    public void ApplySuccessfulDonation_ShouldCompleteCampaign_WhenRaisedAmountExceedsGoal()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 100m);

        // Act
        campaign.ApplySuccessfulDonation(
            CreateMoney(150m),
            Now);

        // Assert
        campaign.RaisedAmount.Amount.Should().Be(150m);
        campaign.Status.Should().Be(CampaignStatus.Completed);
    }

    [Fact]
    public void ApplySuccessfulDonation_ShouldThrow_WhenDonationAmountIsZero()
    {
        // Arrange
        var campaign = CreateCampaign();

        // Act
        Action act = () =>
            campaign.ApplySuccessfulDonation(
                CreateMoney(0m),
                Now);

        // Assert
        var exception = GetDomainException(act);

        exception.Code.Should().Be("Money.InvalidAmount");
    }

    // --------------------------------------------------
    // 7. Progress percentage calculation
    // --------------------------------------------------

    [Fact]
    public void ProgressPercent_ShouldReturnCorrectPercentage()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 200m);

        campaign.ApplySuccessfulDonation(
            CreateMoney(50m),
            Now);

        // Act
        var progress = campaign.ProgressPercent;

        // Assert
        progress.Should().Be(25d);
    }

    [Fact]
    public void ProgressPercent_ShouldNeverExceed100Percent()
    {
        // Arrange
        var campaign = CreateCampaign(goalAmount: 100m);

        campaign.ApplySuccessfulDonation(
            CreateMoney(150m),
            Now);

        // Act
        var progress = campaign.ProgressPercent;

        // Assert
        progress.Should().Be(100d);
    }

    // --------------------------------------------------
    // 8. Days left calculation
    // --------------------------------------------------

    [Fact]
    public void GetDaysLeft_ShouldReturnRemainingDaysRoundedUp()
    {
        // Arrange
        var endDate = Now.AddDays(2).AddHours(3);
        var campaign = CreateCampaign(endDate: endDate);

        // Act
        var daysLeft = campaign.GetDaysLeft(Now);

        // Assert
        daysLeft.Should().Be(3);
    }

    [Fact]
    public void GetDaysLeft_ShouldReturnZero_WhenCampaignHasEnded()
    {
        // Arrange
        var campaign = CreateCampaign(
            endDate: Now.AddDays(1));

        var afterEndDate = Now.AddDays(2);

        // Act
        var daysLeft = campaign.GetDaysLeft(afterEndDate);

        // Assert
        daysLeft.Should().Be(0);
    }
}

    