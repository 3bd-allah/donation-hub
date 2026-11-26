using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Behaviors;
using DonationHub.Application.Features.Campaigns.Commands.Create;
using DonationHub.Application.Features.Campaigns.Commands.Update;
using DonationHub.Application.Features.Campaigns.Common;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Features.Categories.Queries.IsExist;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace DonationHub.Application.Test.Features.Campaigns.Create;

public class CreateCampaignCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<IBaseRepository<Campaign>> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ITransactionContext> _transactionContext = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly DateTimeOffset _now =
        new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

    private readonly FakeTimeProvider _timeProvider;

    public CreateCampaignCommandHandlerTests()
    {
        _timeProvider = new FakeTimeProvider(_now);
        _transactionContext.SetupProperty(x => x.HasTransactionContext, false);
        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private CreateCampaignCommand CreateCommand(Guid? categoryId = null) =>
        new(
            TitleAr: "حملة خيرية",
            TitleEn: "Charity Campaign",
            DescriptionAr: "وصف الحملة",
            DescriptionEn: "Campaign description",
            GoalAmount: 1000m,
            Currency: "EGP",
            EndDate: _now.AddDays(30),
            CategoryId: categoryId ?? Guid.NewGuid(),
            BannerFileName: null);

    private void SetupCategoryExists(Guid categoryId)
    {
        _mediator
            .Setup(x => x.Send(
                It.Is<IRequest<RequestResponse>>(
                    r => r.GetType() == typeof(IsCategoryExistQuery) && ((IsCategoryExistQuery)r).Id == categoryId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(RequestResponse.Ok());
    }

    [Fact]
    public async Task CreateCampaignCommandHandler_WhenCategoryDoesNotExist_ReturnsNotFound_AndDoesNotSave()
    {
        // Arrange
        var command = CreateCommand();

        _mediator
            .Setup(x => x.Send(
                It.Is<IRequest<RequestResponse>>(
                    r => r.GetType() == typeof(IsCategoryExistQuery) && 
                    ((IsCategoryExistQuery)r).Id == command.CategoryId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(RequestResponse.Fail(CategoryErrors.NotFound));

        var handler = new CreateCampaignCommandHandler(
            _mediator.Object,
            _repository.Object,
            _currentUser.Object,
            _timeProvider);

        var behavior = new SaveChangesBehavior<CreateCampaignCommand, RequestResponse<Guid>>(
            _unitOfWork.Object,
            _transactionContext.Object);

        // Act: execute the handler through the save behavior
        var result = await behavior.Handle(
            command,
            ct => handler.Handle(command, ct),
            CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(CategoryErrors.NotFound);

        _repository.Verify(
            x => x.Add(It.IsAny<Campaign>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateCampaignCommandHandler_WhenValid_AddsCampaign_AndSavesOnce()
    {
        // Arrange
        var command = CreateCommand();
        SetupCategoryExists(command.CategoryId);

        var handler = new CreateCampaignCommandHandler(
            _mediator.Object,
            _repository.Object,
            _currentUser.Object,
            _timeProvider);

        var behavior = new SaveChangesBehavior<CreateCampaignCommand, RequestResponse<Guid>>(
            _unitOfWork.Object,
            _transactionContext.Object);

        Campaign? addedCampaign = null;

        _repository
            .Setup(x => x.Add(It.IsAny<Campaign>()))
            .Callback<Campaign>(campaign => addedCampaign = campaign);

        // Act
        var result = await behavior.Handle(
            command,
            ct => handler.Handle(command, ct),
            CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        addedCampaign.Should().NotBeNull();

        _repository.Verify(
            x => x.Add(It.IsAny<Campaign>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
