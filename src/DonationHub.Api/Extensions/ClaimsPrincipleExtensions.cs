using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DonationHub.Api.Extensions;

public static class ClaimsPrincipleExtensions
{

    public static Guid GetUserId(this ClaimsPrincipal principle)
    {
        var claim = principle.Claims.Where(claim => claim.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault();
        string claimValue = claim!.Value;
        Guid userId = Guid.Parse(claimValue);
        return userId;
    }

}
