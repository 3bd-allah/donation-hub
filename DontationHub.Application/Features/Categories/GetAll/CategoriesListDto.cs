namespace DonationHub.Application.Features.Categories.GetAll;

public sealed record CategoryDto(
    Guid Id,
    string NameAr,
    string NameEn,
    string DescriptionAr,
    string DescriptionEn,
    DateTimeOffset CreatedAt);


