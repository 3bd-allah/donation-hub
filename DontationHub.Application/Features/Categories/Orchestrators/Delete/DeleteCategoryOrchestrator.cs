using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Campaigns.Queries.CheckCampaignByCategoryId;
using DonationHub.Application.Features.Categories.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Categories.Orchestrators.Delete;

public sealed record DeleteCategoryOrchestrator(Guid Id) : ICommand<RequestResponse>;



// TODO: you should change this handler to orchestrator cause it require more than one step
public sealed class DeleteCategoryOrchestratorHandler(
    IMediator mediator,
    IBaseRepository<Category> repository) 
    : ICommandHandler<DeleteCategoryOrchestrator, RequestResponse>
{
    public async Task<RequestResponse> Handle(DeleteCategoryOrchestrator request, CancellationToken cancellationToken)
    {
        //TODO Done: 3 round trips for the database to delete the category
        // now become only 2 trips
        var hasCampaignsResult = await mediator.Send(new CheckCampaignByCategoryIdQuery(request.Id), cancellationToken);
        if (hasCampaignsResult.Success)
            return CategoryErrors.HasCampaigns;

        var rowEffected = await repository.DeleteAsync(request.Id, cancellationToken);

        return rowEffected == 0 ?
            CategoryErrors.NotFound :
            RequestResponse.Ok();
    }
}