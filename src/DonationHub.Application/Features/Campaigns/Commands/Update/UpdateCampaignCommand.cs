using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Campaigns.Common;
using DonationHub.Application.Features.Categories.Queries.IsExist;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObjects;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Commands.Update;

public sealed record UpdateCampaignCommand(
    Guid Id,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    decimal GoalAmount,
    string Currency,
    DateTimeOffset EndDate,
    Guid CategoryId,
    string? BannerFileName,
    byte[] RowVersion) : ICommand<RequestResponse>;

public sealed class UpdateCampaignCommandHandler(
    IMediator mediator,
    IBaseRepository<Campaign> repository, 
    TimeProvider timeProvider) : ICommandHandler<UpdateCampaignCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
    {

        var result = await mediator.Send(new IsCategoryExistQuery(request.CategoryId), cancellationToken);
         if (!result.Success)
            return result.Errors.ToArray();

        var campaign = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
            return CampaignErrors.NotFound;

        // detect conflict if already exist, otherwise detected when SaveChanges
        if (!campaign.RowVersion.SequenceEqual(request.RowVersion)) 
            return CampaignErrors.ConcurrencyConflict;

        if (!campaign.CanUpdateCampaign())
            return CampaignErrors.NotActive;

        var utcNow = timeProvider.GetUtcNow();
        if (!campaign.CanChangeEndDate(request.EndDate, utcNow))
            return CampaignErrors.EndDateInPast;

        var newGoalAmount = new Money(request.GoalAmount, request.Currency);
        if (!campaign.CheckCurrency(newGoalAmount))
            return CampaignErrors.CurrencyConflict;

        if (campaign.IsGoalBelowRaised(newGoalAmount))
            return CampaignErrors.GoalBelowRaised;

        campaign.Update(
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            newGoalAmount,
            request.EndDate,
            request.CategoryId,
            request.BannerFileName,
            utcNow
        );

        return RequestResponse.Ok();
    }
}
