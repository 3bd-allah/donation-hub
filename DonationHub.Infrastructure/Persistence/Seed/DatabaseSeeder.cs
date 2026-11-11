using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Persistence.Seed;

public class DatabaseSeeder(
    AppDbContext dbContext,
    TimeProvider timeProvider,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<SeedAdminOptions> adminOptions)
{

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminAsync();
        await SeedCategoriesAsync();
        await SeedTestCampaignAsync();
    }

    
    private async Task SeedCategoriesAsync()
    {
        var categories = new[]
       {
            Category.Create(
                "الصحة",
                "Health",
                "دعم الخدمات الصحية والعلاجية",
                "Supporting healthcare and medical services"),

            Category.Create(
                "التعليم",
                "Education",
                "دعم التعليم وتوفير الفرص التعليمية",
                "Supporting education and educational opportunities"),

            Category.Create(
                "الإغاثة",
                "Relief",
                "تقديم المساعدات الإنسانية والإغاثية",
                "Providing humanitarian and emergency relief"),

            Category.Create(
                "الأيتام",
                "Orphans",
                "رعاية الأيتام وتوفير احتياجاتهم",
                "Supporting orphans and providing for their needs"),

            Category.Create(
                "البيئة",
                "Environment",
                "حماية البيئة والاستدامة",
                "Protecting the environment and promoting sustainability")
        };

        var existingNames = await dbContext.Categories
            .Select(c => new { c.NameAr, c.NameEn })
            .ToListAsync();

        var existingArabicNames = existingNames
            .Select(c => c.NameAr)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingEnglishNames = existingNames
            .Select(c => c.NameEn)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newCategories = categories
            .Where(c =>
                !existingArabicNames.Contains(c.NameAr) &&
                !existingEnglishNames.Contains(c.NameEn))
            .ToList();

        if (newCategories.Count == 0)
            return;

        await dbContext.Categories.AddRangeAsync(newCategories);
        await dbContext.SaveChangesAsync();
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


    private async Task SeedTestCampaignAsync()
    {
        // Find the Health category.
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.NameEn == "Health");

        if (category is null)
            throw new InvalidOperationException(
                "Health category was not found. Seed categories first.");

        // Find the seeded admin.
        var admin = await userManager.FindByEmailAsync(adminOptions.Value.Email);

        if (admin is null)
            throw new InvalidOperationException(
                "Admin user was not found. Seed admin first.");

        // Prevent duplicate test campaigns.
        var campaignExists = await dbContext.Campaigns
            .AnyAsync(c => c.TitleEn == "Test Health Campaign");

        if (campaignExists)
            return;

        var now = timeProvider.GetUtcNow();

        var campaign = Campaign.Create(
            titleAr: "حملة صحية تجريبية",
            titleEn: "Test Health Campaign",
            descriptionAr: "حملة تجريبية لاختبار علاقة التصنيف بالحملات",
            descriptionEn: "A test campaign to verify the category-campaign relationship.",
            goalAmount: new Money(10000m),
            endDate: now.AddMonths(2),
            categoryId: category.Id,
            bannerFileName: null,
            status: CampaignStatus.Active,
            createdByUserId: admin.Id,
            utcNow: now);

        await dbContext.Campaigns.AddAsync(campaign);

        await dbContext.SaveChangesAsync();
    }

}

