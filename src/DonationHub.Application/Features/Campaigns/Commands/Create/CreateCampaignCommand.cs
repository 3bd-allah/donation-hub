using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Campaigns.Common;
using DonationHub.Application.Features.Categories.Queries.IsExist;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObjects;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Commands.Create;

public sealed record CreateCampaignCommand(
        string TitleAr,
        string TitleEn,
        string DescriptionAr,
        string DescriptionEn,
        decimal GoalAmount,
        string Currency,
        DateTimeOffset EndDate,
        Guid CategoryId,
        string? BannerFileName,
        Guid CreatedByUserId ) : ICommand<RequestResponse<Guid>>;


public sealed class CreateCampaignCommandHandler( 
    IMediator mediator,
    IBaseRepository<Campaign> repository,
    TimeProvider timeProvider) 
    : ICommandHandler<CreateCampaignCommand, RequestResponse<Guid>>
{
    public async Task<RequestResponse<Guid>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        var utcNow = timeProvider.GetUtcNow();
        if (request.EndDate < utcNow)
            return CampaignErrors.EndDateInPast;

        var result = await mediator.Send(new IsCategoryExistQuery(request.CategoryId), cancellationToken);
        if (!result.Success)
            return result.Errors.ToArray();

        var goalAmount = new Money(request.GoalAmount, request.Currency);
        var campaign = Campaign.Create(
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            goalAmount,
            request.EndDate,
            request.CategoryId,
            request.BannerFileName,
            request.CreatedByUserId,
            utcNow
            );

        repository.Add(campaign);

        return campaign.Id;
    }
}