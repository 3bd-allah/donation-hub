namespace DonationHub.Api.ViewModels.Categories.Requests;

public sealed record CreateCategoryRequestViewModel(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn);
