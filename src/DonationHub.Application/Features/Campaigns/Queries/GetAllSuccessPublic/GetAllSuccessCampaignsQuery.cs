using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Campaigns.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;

namespace DonationHub.Application.Features.Campaigns.Queries.GetAllSuccessPublic;

public sealed record GetAllSuccessCampaignsQuery(string Language) : IQuery<RequestResponse<List<CampaignDto>>>;


public sealed class GetAllSuccessCampaignsQueryHandler(
    IBaseRepository<Campaign> repository,
    TimeProvider timeProvider)
    : IQueryHandler<GetAllSuccessCampaignsQuery, RequestResponse<List<CampaignDto>>>
{
    public async Task<RequestResponse<List<CampaignDto>>> Handle(GetAllSuccessCampaignsQuery request, CancellationToken cancellationToken)
    {
        string preferredLanguage = Enum.TryParse(request.Language, out PreferredLanguage acceptLanguage) ?
            acceptLanguage.ToString() :
            WebApplicationConstants.DefaultPreferredLanguage;
        var utcNow = timeProvider.GetUtcNow();

        var result = await repository.GetAllAsync(
            c => c.Status == CampaignStatus.Completed,
            c => new CampaignDto
            {
                Id = c.Id,
                Title = preferredLanguage == "en" ? c.TitleEn : c.TitleAr,
                Description = preferredLanguage == "en" ? c.DescriptionEn : c.DescriptionAr,
                BannerFileName = c.BannerFileName,
                CategoryName = request.Language == "en" ? c.Category.NameEn : c.Category.NameAr,
                CreatedAt = c.CreatedAt,
                EndDate = c.EndDate,
                GoalAmount = c.GoalAmount,
                RaisedAmount = c.RaisedAmount,
                RowVersion = c.RowVersion,
                CreatedByUserId = c.CreatedByUserId,
                Status = c.Status
            }, cancellationToken);

        return result;
    }
}
