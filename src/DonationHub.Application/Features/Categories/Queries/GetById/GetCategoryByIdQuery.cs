using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Categories.Queries.GetById;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<RequestResponse<CategoryItemDto>>;


public sealed class GetCategoryByIdQueryHandler ( IBaseRepository<Category> repository )
    : IQueryHandler<GetCategoryByIdQuery, RequestResponse<CategoryItemDto>>
{
    public async Task<RequestResponse<CategoryItemDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        
        var category = await repository.FirstOrDefaultAsync(
            ct => ct.Id == request.Id,
            ct => new CategoryItemDto(
                ct.Id,
                ct.NameAr,
                ct.NameEn,
                ct.DescriptionAr!,
                ct.DescriptionEn!,
                ct.CreatedAt,
                Campaigns: ct.Campaigns.Select(c =>
                new CampaignInCategoryItemDto(
                    c.Id,
                    c.TitleAr,
                    c.TitleEn,
                    c.DescriptionAr,
                    c.DescriptionEn,
                    c.Status)).ToList()), 
            cancellationToken,
            noTracking: true);


        if (category is null)
            return CategoryErrors.NotFound;

        return category;
    }
}