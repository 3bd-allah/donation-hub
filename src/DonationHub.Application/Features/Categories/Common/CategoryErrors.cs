using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Categories.Common;

public class CategoryErrors
{
    public static Error DuplicateName =>
        new("Category.DuplicateNames", "This category already exist.", ErrorType.Conflict);

    public static Error HasCampaigns =>
        new("Category.HasCampaigns", "Cannot delete a category with linked campaigns", ErrorType.Conflict);

    public static Error NotFound=>
        new("Category.NotFound", "Category does not exist", ErrorType.NotFound);
}
