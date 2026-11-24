using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Campaigns.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;

namespace DonationHub.Application.Features.Campaigns.Queries.GetAllActiveAdmin;

public sealed record GetAllActiveCampaignsForAdminQuery : IQuery<RequestResponse<List<CampaignForAdminDto>>>;

public sealed class GetAllActiveCampaignsForAdminQueryHandler(
    IBaseRepository<Campaign> repository,
    TimeProvider timeProvider)
    : IQueryHandler<GetAllActiveCampaignsForAdminQuery, RequestResponse<List<CampaignForAdminDto>>>
{
    public async Task<RequestResponse<List<CampaignForAdminDto>>> Handle(
        GetAllActiveCampaignsForAdminQuery request, 
        CancellationToken cancellationToken)
    {

        var utcNow = timeProvider.GetUtcNow();

        var result = await repository.GetAllAsync(
            c => c.Status == CampaignStatus.Active,
            c => new CampaignForAdminDto
            {
                Id = c.Id,
                TitleAr = c.TitleAr,
                TitleEn = c.TitleEn,
                DescriptionAr = c.DescriptionAr,
                DescriptionEn= c.DescriptionEn,
                BannerFileName = c.BannerFileName,
                CategoryNameAr = c.Category.NameAr,
                CategoryNameEn = c.Category.NameEn,
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