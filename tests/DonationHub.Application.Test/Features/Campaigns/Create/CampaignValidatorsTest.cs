using DonationHub.Application.Features.Campaigns.Commands.Create;
using FluentAssertions;

namespace DonationHub.Application.Test.Features.Campaigns.Create
{
    public class CampaignValidatorsTest
    {
        private readonly CreateCampaignCommandValidator _validator = new();

        private static CreateCampaignCommand CreateValidCommand() =>
            new(
                TitleAr: "حملة تبرع",
                TitleEn: "Donation Campaign",
                DescriptionAr: "وصف الحملة",
                DescriptionEn: "Campaign description",
                GoalAmount: 1000m,
                "USD",
                EndDate: DateTimeOffset.UtcNow.AddDays(10),
                CategoryId: Guid.NewGuid(),
                BannerFileName: null);
        [Fact]
        public void Validate_WhenTitleArIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var campaign = CreateValidCommand() with { TitleAr = "" };

            // Act
            var result = _validator.Validate(campaign);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.PropertyName == nameof(campaign.TitleAr));
        }

        [Fact]
        public void Validate_WhenTitleEnIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = CreateValidCommand() with { TitleEn = "" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();

            result.Errors.Should().ContainSingle(
                error => error.PropertyName == nameof(command.TitleEn));
        }

        [Fact]
        public void Validate_WhenGoalAmountIsNegative_ShouldHaveValidationError()
        {
            // Arrange
            var command = CreateValidCommand() with { GoalAmount = -100 };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();

            result.Errors.Should().ContainSingle(
                error => error.PropertyName == nameof(command.GoalAmount));
        }

        [Fact]
        public void Validate_WhenGoalAmountIsZero_ShouldHaveValidationError()
        {
            // Arrange
            var command = CreateValidCommand() with { GoalAmount = 0 };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(error => error.PropertyName == nameof(command.GoalAmount));
        }


        [Fact]
        public void Validate_WhenCategoryIdIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = CreateValidCommand() with { CategoryId = Guid.Empty };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();

            result.Errors.Should().ContainSingle(
                error => error.PropertyName == nameof(command.CategoryId));
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }


    }
}
