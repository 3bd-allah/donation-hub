using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Campaigns.Queries.CheckCampaignByCategoryId;

public sealed record CheckCampaignByCategoryIdQuery(Guid CategoryId) : IQuery<RequestResponse<bool>>;

public sealed class GetByCategoryIdHandler (IBaseRepository<Campaign> repository )
    : IQueryHandler<CheckCampaignByCategoryIdQuery, RequestResponse<bool>>
{
    public async Task<RequestResponse<bool>> Handle(CheckCampaignByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        // check the category has campaign or not
        var hasCategory = await repository
            .AnyAsync(c => c.CategoryId == request.CategoryId, cancellationToken);

        return hasCategory;
    }
}