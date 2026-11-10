using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.GetById;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<RequestResponse<CategoryItemDto>>;


public sealed class GetCategoryByIdQueryHandler (
    IBaseRepository<Category> repository): IRequestHandler<GetCategoryByIdQuery, RequestResponse<CategoryItemDto>>
{
    public async Task<RequestResponse<CategoryItemDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await repository.GetAll()
            .Where(ct => ct.Id == request.Id)
            .Select(ct =>
            new CategoryItemDto(
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
                    c.Status)).ToList())
            ).FirstOrDefaultAsync(cancellationToken);

        if (category is null)
            return RequestResponse<CategoryItemDto>.Fail(CategoryCodes.NotFound, new()
            {
                {"category",["category not found"]}
            });

        return RequestResponse<CategoryItemDto>.Ok(AuthCodes.Success, category);
    }
}