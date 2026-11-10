using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using DonationHub.Domain.Enums;
using DonationHub.Application.Shared;
using DonationHub.Application.Features.Authentication.Common;
namespace DonationHub.Infrastructure.Authentication;

public class RegisterService(
    UserManager<ApplicationUser> userManager, 
    AppDbContext context) : IRegisterService
{
    public async Task<RequestResponse> IsEmailExistAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is not null?
            RequestResponse.Fail(AuthCodes.EmailAlreadyExists, new() { { "email", ["EmailAlreadyExist"] } }) :
            RequestResponse.Ok();
    }

    public async Task<RequestResponse> RegisterAsync(string email, string fullName,string password, string preferredLanguage, CancellationToken ct)
    {

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        var createdUser = ApplicationUser.Create(email, fullName, preferredLanguage);

        
        var createUserResult = await userManager.CreateAsync(createdUser, password);
        if (!createUserResult.Succeeded)
        {
            await transaction.RollbackAsync(ct);
            return RequestResponse.Fail(500, new() 
                {
                    { "User Creation", [.. createUserResult.Errors.Select(err => err.Description)]}
                }
            );
        }

        var assignRoleResult = await userManager.AddToRoleAsync(createdUser, Roles.Donor.ToString());
        if (!assignRoleResult.Succeeded)
        {
            await transaction.RollbackAsync(ct);
            var errors = assignRoleResult.Errors.Select(er => er.Description);
            return RequestResponse.Fail(500, new()
            {
                { "Assigning User Role", [.. errors] }
            });
        }
        
        await transaction.CommitAsync(ct);
        return RequestResponse.Ok();
    }
}
