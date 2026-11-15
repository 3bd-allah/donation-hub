using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Campaigns.Queries.CheckCampaignByCategoryId;

public sealed record CheckCampaignByCategoryIdQuery(Guid CategoryId) : IQuery<RequestResponse>;

public sealed class GetByCategoryIdHandler (IBaseRepository<Campaign> repository )
    : IQueryHandler<CheckCampaignByCategoryIdQuery, RequestResponse>
{
    public async Task<RequestResponse> Handle(CheckCampaignByCategoryIdQuery request, CancellationToken cancellationToken)
    {

        var hasCategory = await repository
            .AnyAsync(c => c.CategoryId == request.CategoryId, cancellationToken);

        if (hasCategory)
            return RequestResponse.Ok();

        return RequestResponse.Fail();

    }
}