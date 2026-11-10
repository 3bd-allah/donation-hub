using DonationHub.Domain.Enums;

namespace DonationHub.Application.Features.Categories.GetById;

public sealed record CategoryItemDto(Guid Id,
    string NameAr,
    string NameEn,
    string DescriptionAr,
    string DescriptionEn,
    DateTimeOffset CreatedAt,
    IReadOnlyList<CampaignInCategoryItemDto> Campaigns);
public sealed record CampaignInCategoryItemDto(
    Guid Id,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    CampaignStatus Status);