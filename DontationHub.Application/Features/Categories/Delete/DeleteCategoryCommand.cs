using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.Delete;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<RequestResponse>;



// TODO: you should change this handler to orchestrator cause it require more than one step
public sealed class DeleteCategoryCommandHandler(
    IBaseRepository<Category> categoryRepository,
    IBaseRepository<Campaign> campaignRepository,
    IUnitOfWork unitOfWork ) : IRequestHandler<DeleteCategoryCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        //TODO: 3 round trips for the database to delete the category
        // find the category
        //var isCategoryExist = await categoryRepository.IsExistAsync(request.Id, cancellationToken);

        var category = await categoryRepository.GetById(request.Id, cancellationToken);

        // if missing return not found
        if (category is null)
            return RequestResponse.Fail(CategoryCodes.NotFound, new()
            {
                {"category", ["this category is not exist"] }
            });

        var isCategoryHasCampaigns = await campaignRepository.GetAll().Where(ca => ca.CategoryId == request.Id).AnyAsync(cancellationToken);
        if (isCategoryHasCampaigns)
            return RequestResponse.Fail(CategoryCodes.HasCampaigns, new()
            {
                {"category", ["this category has campaings"] }
            });

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse.Ok(CategoryCodes.Success);

    }
}