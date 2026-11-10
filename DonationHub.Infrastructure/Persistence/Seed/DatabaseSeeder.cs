using DonationHub.Domain.Enums;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Persistence.Seed;

public class DatabaseSeeder(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<SeedAdminOptions> adminOptions)
{

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminAsync();
    }

    //TODO:
    private async Task SeedCategoriesAsync()
    {

    }

    private async Task SeedRolesAsync()
    {
        var roles = Enum.GetValues<Roles>()
            .Where(role => role != Roles.Anonymous);

        foreach (var role in roles)
        {
            var roleName = role.ToString();

            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(
                new ApplicationRole { Name = roleName });

            EnsureSucceeded(result);
        }
    }

    private async Task SeedAdminAsync()
    {
        var options = adminOptions.Value;

        var admin = await userManager.FindByEmailAsync(options.Email);

        if (admin is null)
        {
            admin = ApplicationUser.Create(options.Email, options.FullName);

            var result = await userManager.CreateAsync(
                admin,
                options.Password);

            EnsureSucceeded(result);
        }

        // Ensure the admin has the Admin role.
        if (!await userManager.IsInRoleAsync(admin, Roles.Admin.ToString()))
        {
            var result = await userManager.AddToRoleAsync(
                admin,
                Roles.Admin.ToString());

            EnsureSucceeded(result);
        }
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join(
            ", ",
            result.Errors.Select(error => error.Description));

        throw new InvalidOperationException(
            $"Identity seeding failed: {errors}");
    }
}

