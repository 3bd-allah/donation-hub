using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObjects;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configuration;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");

        builder.HasKey(c => c.Id);


        // Configure Properties
        builder.Property(c => c.TitleAr)
            .HasMaxLength(100)
            .IsRequired();  

        builder.Property(c => c.TitleEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.DescriptionAr)
            .HasMaxLength(500);

        builder.Property(c => c.DescriptionEn)
            .HasMaxLength(500);

        builder.Property(c => c.BannerFileName)
            .HasMaxLength(100);

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired();

        builder.Property(c => c.DeletedAtUTC)
            .IsRequired(false);

        builder.Property(c => c.CreatedByUserId)
            .IsRequired();
        
        // Category relationship one-to-many
        builder.HasOne(c => c.Category)
            .WithMany(ca => ca.Campaigns)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // ApplicationUser relationship one-to-many
        builder.HasOne<ApplicationUser>()
           .WithMany()
           .HasForeignKey(c => c.CreatedByUserId)
           .OnDelete(DeleteBehavior.NoAction);


        // Configure the owned types for GoalAmount and RaisedAmount
        builder.OwnsOne(c => c.GoalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("GoalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("GoalAmountCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .IsRequired();
        });

        builder.OwnsOne(c => c.RaisedAmount, money =>
        {
            money.Property(m => m.Amount)
            .HasColumnName("RaisedAmount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("RaisedAmountCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .IsRequired();
        });

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Query filter to exclude soft-deleted campaigns
        builder.HasQueryFilter("SoftDelete", c => !c.IsDeleted);

        // make index for IsDeleted cause every query will filter by IsDeleted to exclude soft deleted campaigns
        builder.HasIndex(c => c.IsDeleted);

    }
}
