using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Features.Users.Common;
using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Users.Queries.GetById;

public sealed record GetUserByIdQuery (Guid Id) : IQuery<RequestResponse<UserDataDto>>;

public sealed class GetUserByIdQueryHandler(IUserService userService) 
    : IQueryHandler<GetUserByIdQuery, RequestResponse<UserDataDto>>
{
    public async Task<RequestResponse<UserDataDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // get the user by id if exist, otherwise null;
        var userData = await userService.GetUserByIdAsync(request.Id, cancellationToken);

        return userData is null ?
            UserErrors.UserNotFound :
            userData;

    }
}