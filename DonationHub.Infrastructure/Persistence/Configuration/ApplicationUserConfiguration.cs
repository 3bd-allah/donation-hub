using DonationHub.Domain.Constants;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {

        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PreferredLanguage)
            .HasDefaultValue(WebApplicationConstants.DefaultPreferredLanguage)
            .HasMaxLength(2);

        // add unique index on the email column
        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique();

    }
}
