using AutoFixture;
using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Campaigns.Commands.Update;
using DonationHub.Application.Features.Campaigns.Common;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Features.Categories.Queries.IsExist;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Runtime.CompilerServices;

namespace DonationHub.Application.Test.Features.Campaigns.Update;

public class UpdateCampaignCommandHandlerTests
{

    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly IMediator _mediator;
    private readonly Mock<IBaseRepository<Campaign>> _repositoryMock = new();
    private readonly IBaseRepository<Campaign> _repository;
    private readonly IFixture _autoFixture;

    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;
    private readonly FakeTimeProvider _timeProvider;
    public UpdateCampaignCommandHandlerTests()
    {
        _mediator = _mediatorMock.Object;
        _repository = _repositoryMock.Object;
        _autoFixture = new Fixture();
        _timeProvider = new FakeTimeProvider(_now);
    }

    [Fact]
    public async Task UpdateCampaignCommand_WhenCampaignNotExist_ShouldReturnCampaignNotFoundError()
    {
        // Arrange
        var updateCommand = _autoFixture.Create<UpdateCampaignCommand>();

        _repositoryMock.Setup(_repository => _repository.GetByIdAsync(updateCommand.Id, It.IsAny<CancellationToken>()))
            .Returns(null);

        _mediatorMock.Setup(mediator => mediator.Send(
                It.Is<IQuery<RequestResponse>>(query => query.GetType() == typeof(IsCategoryExistQuery) &&
                                                ((IsCategoryExistQuery)query).Id == updateCommand.CategoryId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(RequestResponse.Ok());

        var handler = new UpdateCampaignCommandHandler(
            _mediator,
            _repository,
            _timeProvider);

        // Act
        var result = await handler.Handle(updateCommand, It.IsAny<CancellationToken>());

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(CampaignErrors.NotFound);
    }


    [Fact]
    public async Task UpdateCampaignCommand_WhenUpdateConflict_ShouldReturnCamaignConcurrencyConflictError()
    {
        // Arrange
        var updateCommand = _autoFixture.Create<UpdateCampaignCommand>();

        var validCampaign = Campaign.Create(
            titleAr: "حملة تبرعات",
            titleEn: "Donation Campaign",
            descriptionAr: "وصف الحملة",
            descriptionEn: "Campaign description",
            goalAmount: new Money(1000, "USD"),
            endDate: _timeProvider.GetUtcNow().AddDays(30),
            categoryId: It.IsAny<Guid>(),
            bannerFileName: null,
            createdByUserId: It.IsAny<Guid>(),
            utcNow: _now);

        _repositoryMock.Setup(_repository => _repository.GetByIdAsync(updateCommand.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validCampaign);

        _mediatorMock.Setup(mediator => mediator.Send(
                It.Is<IQuery<RequestResponse>>(query => query.GetType() == typeof(IsCategoryExistQuery) &&
                                                ((IsCategoryExistQuery)query).Id == updateCommand.CategoryId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(RequestResponse.Ok());

        var handler = new UpdateCampaignCommandHandler(
            _mediator,
            _repository,
            _timeProvider);

        // Act
        var result = await handler.Handle(updateCommand, It.IsAny<CancellationToken>());

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(CampaignErrors.ConcurrencyConflict);
    }


}


