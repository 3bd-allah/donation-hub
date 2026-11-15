using DonationHub.Application.Shared.ResultPattern;

namespace DonationHub.Application.Features.Categories.Common;

public class CategoryErrors
{
    public static Error DuplicateNameAr =>
        new("Category.DuplicateNames", "there is category with entered NameAr.", ErrorType.Conflict, "NameAr");

    public static Error DuplicateNameEn =>
        new("Category.DuplicateNames", "there is category with entered NameEn.", ErrorType.Conflict, "NameEn");

    public static Error HasCampaigns =>
        new("Category.HasCampaigns", "Cannot delete a category with linked campaigns", ErrorType.Conflict);

    public static Error NotFound=>
        new("Category.NotFound", "Category does not exist", ErrorType.NotFound);
}
