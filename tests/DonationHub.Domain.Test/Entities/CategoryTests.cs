using DonationHub.Domain.Entities;
using FluentAssertions;

namespace DonationHub.Domain.Test.Entities;

public class CategoryTests
{

    [Fact]
    public void EnsureCanBeDeleted_WhenCategoryHasNoCampaigns_ReturnsTrue()
    {
        // Arrange
        var category = Category.Create(
            "التبرعات",
            "Donations",
            null,
            null);

        // Act
        var result = category.EnsureCanBeDeleted();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EnsureCanBeDeleted_WhenCategoryHasCampaigns_ReturnsFalse()
    {
        // Arrange
        var category = Category.Create(
            "التبرعات",
            "Donations",
            null,
            null);
        category.AddCampaign(CampaignTests.CreateCampaign());

        // Act
        var result = category.EnsureCanBeDeleted();

        // Assert
        result.Should().BeFalse();
    }

}
