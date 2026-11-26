using DonationHub.Api.Extensions;
using DonationHub.Application.Abstractions.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DonationHub.Api.Infrastructure;

internal sealed class CurrentUser (IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId => accessor.HttpContext?.User.GetUserId() ??
        throw new InvalidOperationException("No authenticated user in the current context.");

}
