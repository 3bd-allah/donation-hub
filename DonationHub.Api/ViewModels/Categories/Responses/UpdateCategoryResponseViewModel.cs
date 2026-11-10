namespace DonationHub.Api.ViewModels.Categories.Responses;

public sealed record UpdateCategoryResponseViewModel(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn);