using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.GetAll;

public sealed record CategoriesListQuery : IRequest<RequestResponse<List<CategoryDto>>>;

public sealed class CategoriesListQueryHandler (
    IBaseRepository<Category> repository): IRequestHandler<CategoriesListQuery, RequestResponse<List<CategoryDto>>>
{
    public async Task<RequestResponse<List<CategoryDto>>> Handle(CategoriesListQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetAll()
            .Select(c => new CategoryDto(c.Id,c.NameAr, c.NameEn, c.DescriptionAr!, c.DescriptionEn!, c.CreatedAt))
            .ToListAsync(cancellationToken);

        if (categories is null)
            return RequestResponse<List<CategoryDto>>.Fail();
        return RequestResponse<List<CategoryDto>>.Ok(AuthCodes.Success, categories);
    }
}