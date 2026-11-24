using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Application.Features.Campaigns.Common;
using DonationHub.Application.Shared.ResultPattern;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Campaigns.Commands.Delete;

public sealed record DeleteCampaignCommand(Guid Id) : ICommand<RequestResponse>;


public sealed class DeleteCampaignCommandHandler( IBaseRepository<Campaign> repository ) 
    : ICommandHandler<DeleteCampaignCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
            return CampaignErrors.NotFound;


        repository.Delete(campaign);

        return RequestResponse.Ok();
    }
}