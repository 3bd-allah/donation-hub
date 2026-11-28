using AutoFixture;
using DonationHub.Application.Behaviors;
using DonationHub.Application.Features.Campaigns.Commands.Create;
using DonationHub.Application.Shared.ResultPattern;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace DonationHub.Application.Test.Behaviors;

public sealed class ValidationBehvaiorTests
{

    [Fact]
    public async Task ValidationBehavior_WhenAnyInvalidValue_ShortCircuitsWithValidationFailed()
    {
        // Arrange
        var command = new CreateCampaignCommand(
            TitleAr: "",
            TitleEn: "",
            DescriptionAr: "Description",
            DescriptionEn: "Description",
            GoalAmount: 1000m,
            Currency: "EGP",
            EndDate: DateTimeOffset.UtcNow.AddDays(30),
            CategoryId: Guid.NewGuid(),
            BannerFileName: null);

        var validationResult = new ValidationResult(
        [
            new ValidationFailure("TitleAr", "Arabic title is required")
            {
                ErrorCode = "Validation.Failed"
            },
            new("TitleEn", "title english is required.")
            {
                ErrorCode = "Validation.Failed"
            }
        ]);

        var validator = new CreateCampaignCommandValidator();

       
        bool nextCalled = false;

        RequestHandlerDelegate<RequestResponse<Guid>> next = ct =>
        {
            nextCalled = true;
            return Task.FromResult(RequestResponse<Guid>.Ok(Guid.NewGuid()));
        };

        var validationBehavior = new ValidationBehavior<CreateCampaignCommand, RequestResponse<Guid>>
            ( [validator] );

        //var failures = validationResult.


        // Act
        var result = await validationBehavior.Handle(
            command,
            next,
            CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        foreach (var error in result.Errors)
            error.Type.Should().Match(e => e!.Value == ErrorType.Validation);

        nextCalled.Should().BeFalse();

        //validator.Verify(
        //    x => x.ValidateAsync(
        //        It.IsAny<ValidationContext<CreateCampaignCommand>>(),
        //        It.IsAny<CancellationToken>()),
        //    Times.Once);

    }

}
