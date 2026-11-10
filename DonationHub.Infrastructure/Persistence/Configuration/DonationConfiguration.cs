using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configuration;

public class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {

        builder.ToTable("Donations");
        // primary key
        builder.HasKey(d => d.Id);


        // configure properties 
        builder.Property(d => d.CampaignId)
            .IsRequired();

        builder.Property(d => d.DonorUserId)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>();

        builder.Property(d => d.ReceiptFileName)
            .HasMaxLength(100);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.ReceiptNumber)
            .IsRequired(false);

        // Owned type configuration for Price
        builder.OwnsOne(d => d.Amount, money =>
        {
            money.Property(m => m.Amount)
            .HasColumnName("Amount")
            .HasPrecision(18, 2)
            .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("AmountCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .IsRequired();
        });


        // configure relationships
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(d => d.DonorUserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(d => d.CampaignId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

    }
}
