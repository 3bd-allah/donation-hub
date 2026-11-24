using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configuration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        // primary key
        builder.HasKey(rt => rt.Id);

        // configure properties
        builder.Property(rt => rt.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        // configure relationships
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .IsRequired();

        // Add Index for TokanHash cause we will retrieve based on it many times
        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique();

    }
}
