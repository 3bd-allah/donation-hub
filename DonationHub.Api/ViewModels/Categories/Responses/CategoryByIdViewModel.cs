using DonationHub.Application.Features.Categories.GetById;
using DonationHub.Domain.Enums;

namespace DonationHub.Api.ViewModels.Categories.Responses;

public sealed record CategoryByIdViewModel(
    string NameAr,
    string NameEn,
    string DescriptionAr,
    string DescriptionEn,
    List<CampaignInCategoryItemViewModel> Campaigns);

public sealed record CampaignInCategoryItemViewModel(
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    CampaignStatus Status);


