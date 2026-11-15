using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using MediatR;
using System.Windows.Input;

namespace DonationHub.Application.Features.Categories.Commands.Update;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn) : ICommand<RequestResponse<UpdateCategoryResponseDto>>;

public sealed class UpdateCategoryCommandHandler( IBaseRepository<Category> repository )
    : ICommandHandler<UpdateCategoryCommand, RequestResponse<UpdateCategoryResponseDto>>
{
    public async Task<RequestResponse<UpdateCategoryResponseDto>> Handle(
        UpdateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        // get the category to update => 1 trip to database 
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return CategoryErrors.NotFound;

        // check names duplication
        if (category.NameAr == request.NameAr)
            return CategoryErrors.DuplicateNameAr;

        if (category.NameEn == request.NameEn)
            return CategoryErrors.DuplicateNameEn;

        // update entity
        category.Update(request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn);

        //TODO: refactor update mechanism
        // 2 trip to save changes


        var updateCategoryResponse = new UpdateCategoryResponseDto(
            category.NameAr,
            category.NameEn,
            category.DescriptionAr,
            category.DescriptionEn);
        return updateCategoryResponse;

    }
}