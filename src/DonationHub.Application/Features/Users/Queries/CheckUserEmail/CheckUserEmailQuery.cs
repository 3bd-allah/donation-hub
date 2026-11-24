using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Application.Abstractions.Messaging;
using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Users.Queries.CheckUserEmail;

public sealed record CheckUserEmailQuery(string Email) : IQuery<RequestResponse<bool>>;

public sealed class CheckUserEmailQueryHandler( IEmailExist emailExist )
    : IQueryHandler<CheckUserEmailQuery, RequestResponse<bool>>
{
    public async Task<RequestResponse<bool>> Handle(CheckUserEmailQuery request, CancellationToken cancellationToken)
    {
        return await emailExist.IsEmailExistAsync(request.Email);
    }
}