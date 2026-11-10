using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Categories.Update;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn) : IRequest<RequestResponse<UpdateCategoryResponseDto>>;

public sealed record UpdateCategoryResponseDto(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn);

public sealed class UpdateCategoryCommandHandler(
    IBaseRepository<Category> repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, RequestResponse<UpdateCategoryResponseDto>>
{
    public async Task<RequestResponse<UpdateCategoryResponseDto>> Handle(
        UpdateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        // get the category to update => 1 trip to database 
        var category = await repository.GetById(request.Id, cancellationToken);
        if (category is null)
            return RequestResponse<UpdateCategoryResponseDto>.Fail(CategoryCodes.NotFound);

        //TODO: refactor with result pattern
        // check names duplication
        if (category.NameAr == request.NameAr ||
            category.NameEn == request.NameEn)
            return RequestResponse<UpdateCategoryResponseDto>.Fail(CategoryCodes.DuplicateNames,
                new()
                {
                    {"Name", ["there is duplicate in category name"] }
                });


        // update entity
        category.Update(request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn);

        //TODO: refactor update mechanism
        // 2 trip to save changes
        await unitOfWork.SaveChangesAsync(cancellationToken);


        var updateCategoryResponse = new UpdateCategoryResponseDto(
            category.NameAr,
            category.NameEn,
            category.DescriptionAr,
            category.DescriptionEn);
        return RequestResponse<UpdateCategoryResponseDto>.Ok(CategoryCodes.Success, updateCategoryResponse);

    }
}