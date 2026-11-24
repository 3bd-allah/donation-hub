using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Categories.Commands.Delete;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand<RequestResponse>;


public sealed class DeleteCategoryCommandHandler( ICategoryRepository repository) 
    : ICommandHandler<DeleteCategoryCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryWithCampaigns(request.Id, cancellationToken);

        if (category is null)
            return CategoryErrors.NotFound;

        if (!category.EnsureCanBeDeleted())
            return CategoryErrors.HasCampaigns;

        await repository.DeleteAsync(request.Id, cancellationToken);

        return RequestResponse.Ok();
    }
}