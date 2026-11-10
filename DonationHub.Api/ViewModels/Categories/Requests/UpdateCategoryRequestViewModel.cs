namespace DonationHub.Api.ViewModels.Categories.Requests;

public sealed record UpdateCategoryRequestViewModel(
    string? NameAr,
    string? NameEn,
    string? DescriptionAr,
    string? DescriptionEn);