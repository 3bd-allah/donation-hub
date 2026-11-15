using DonationHub.Application.Abstractions.Authentication;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using DonationHub.Domain.Enums;
namespace DonationHub.Infrastructure.Authentication;

public class RegisterService(
    UserManager<ApplicationUser> userManager, 
    AppDbContext context) : IRegisterService
{
    public async Task<bool> IsEmailExistAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<Guid?> RegisterAsync(
        string email,
        string fullName,
        string password, 
        string preferredLanguage,
        Roles userRole,
        CancellationToken ct)
    {

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        var createdUser = ApplicationUser.Create(email, fullName, preferredLanguage);

        
        var createUserResult = await userManager.CreateAsync(createdUser, password);
        if (createUserResult.Succeeded)
        {
            var assignRoleResult = await userManager.AddToRoleAsync(createdUser, userRole.ToString());
            if (assignRoleResult.Succeeded)
            {

                await transaction.CommitAsync(ct);
                return createdUser.Id;
            }
        }
        await transaction.RollbackAsync(ct);
        return null;
    }
}
