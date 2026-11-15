using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Features.Users.Queries.GetById;
using DonationHub.Application.Shared.ResultPattern;
using MediatR;

namespace DonationHub.Application.Features.Authentication.CurrentProfile;

public sealed record GetCurrentProfileOrchestrator (Guid Id) 
    : IQuery<RequestResponse<CurrentProfileInfoDto>>;

public sealed class GetCurrentProfileQueryHandler ( IMediator mediator )
    : IQueryHandler<GetCurrentProfileOrchestrator, RequestResponse<CurrentProfileInfoDto>>
{
    public async Task<RequestResponse<CurrentProfileInfoDto>> Handle(GetCurrentProfileOrchestrator request, CancellationToken cancellationToken)
    {
        var userProfileResult = await mediator.Send(new GetUserByIdQuery(request.Id), cancellationToken);

        if (!userProfileResult.Success )
            return userProfileResult.Errors.ToArray();

        var userProfileData = userProfileResult.Data;

        if (!userProfileData.IsActive)
            return AuthErrors.AccountDeactivated;

        return new CurrentProfileInfoDto(
                userProfileData.Id, 
                userProfileData.Email, 
                userProfileData.FullName, 
                userProfileData.Roles);
    }
}