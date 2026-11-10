using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Persistence;

public class AppDbContext (DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid> (options)
{

    public DbSet<Campaign> Campaigns{ get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    override protected void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    }

}
