using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Features.Authentication.Common;
using DonationHub.Application.Shared;
using MediatR;

namespace DonationHub.Application.Features.Authentication.CurrentProfile;

public sealed record CurrentProfileQuery (Guid Id) : IRequest<RequestResponse<CurrentProfileDto>>;

public sealed class CurrentProfileQueryHandler (
    IUserService userService): IRequestHandler<CurrentProfileQuery, RequestResponse<CurrentProfileDto>>
{
    public async Task<RequestResponse<CurrentProfileDto>> Handle(CurrentProfileQuery request, CancellationToken cancellationToken)
    {

        var result = await userService.GetUserProfileAsync(request.Id);
        if (result is null)
            return RequestResponse<CurrentProfileDto>.Fail(AuthCodes.AccountDeactivated);

        return RequestResponse<CurrentProfileDto>.Ok(AuthCodes.Success, 
            new(result.Id, result.Email, result.FullName, result.Roles));
    }
}