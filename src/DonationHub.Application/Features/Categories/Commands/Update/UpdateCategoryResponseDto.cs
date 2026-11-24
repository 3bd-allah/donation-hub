namespace DonationHub.Application.Features.Categories.Commands.Update;

public sealed record UpdateCategoryResponseDto(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn);
