using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Features.Categories.Update;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.Create;

public sealed record CreateCategoryCommand(
    string NameAr, 
    string NameEn, 
    string? DescriptionAr, 
    string? DescriptionEn) : IRequest<RequestResponse>;


public class CreateCategoryCommandHandler(
    IBaseRepository<Category> repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // duplication check
        var duplicationCheck = await repository.GetAll()
            .Where(c => c.NameAr == request.NameAr || c.NameEn == request.NameEn)
            .Select(c => new { c.NameAr, c.NameEn})
            .FirstOrDefaultAsync(cancellationToken);

        if(duplicationCheck is not null)
            return RequestResponse<UpdateCategoryResponseDto>.Fail(CategoryCodes.DuplicateNames,
                new()
                {
                    {"Name", ["there is duplicate in category name"] }
                });


        var category = Category.Create(request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn);
        repository.Add(category);
        var result = await unitOfWork.SaveChangesAsync(cancellationToken);


        return result >= 1 ? RequestResponse.Ok() : RequestResponse.Fail();
    }
}