namespace DonationHub.Api.ViewModels.Categories.Responses;


public sealed record CategoryViewModel(
    Guid Id,
    string NameAr, 
    string NameEn,
    string DescriptionAr, 
    string DescriptionEn);

