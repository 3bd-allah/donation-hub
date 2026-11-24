using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Categories.Commands.Update;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string DescriptionAr,
    string DescriptionEn) : ICommand<RequestResponse<UpdateCategoryResponseDto>>;

public sealed class UpdateCategoryCommandHandler( IBaseRepository<Category> repository )
    : ICommandHandler<UpdateCategoryCommand, RequestResponse<UpdateCategoryResponseDto>>
{
    public async Task<RequestResponse<UpdateCategoryResponseDto>> Handle(
        UpdateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        // check if there is any duplicate in names
        var isDuplicate = await repository.AnyAsync(c => c.Id != request.Id &&
                                                (c.NameAr == request.NameAr || c.NameEn == request.NameEn), 
                                                cancellationToken
                                            );
        if (isDuplicate)
            return CategoryErrors.DuplicateName;

        
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)       
            return CategoryErrors.NotFound;

        // update entity
        category.Update(request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn);

        //TODO: refactor update mechanism
        // 2 trip to save changes
        // congratulations become 3 trips to delete  

        var updateCategoryResponse = new UpdateCategoryResponseDto(
            category.NameAr,
            category.NameEn,
            category.DescriptionAr,
            category.DescriptionEn);
        return updateCategoryResponse;

    }
}