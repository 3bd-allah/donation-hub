using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Categories.Queries.GetAll;

public sealed record CategoriesListQuery : IQuery<RequestResponse<List<CategoryDto>>>;

public sealed class CategoriesListQueryHandler ( IBaseRepository<Category> repository )
    : IQueryHandler<CategoriesListQuery, RequestResponse<List<CategoryDto>>>
{
    public async Task<RequestResponse<List<CategoryDto>>> Handle(CategoriesListQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetAllAsync(null,
            c => new CategoryDto(c.Id,
            c.NameAr,
            c.NameEn,
            c.DescriptionAr!,
            c.DescriptionEn!,
            c.CreatedAt), cancellationToken);

        if (categories is null)
            return CategoryErrors.NotFound;

        return categories;
    }
}