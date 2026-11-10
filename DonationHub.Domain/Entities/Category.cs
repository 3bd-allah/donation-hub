using DonationHub.Domain.Interfaces;
using DonationHub.Domain.Primitives;
using System.Runtime.InteropServices;

namespace DonationHub.Domain.Entities;

public sealed class Category : Entity, IAuditable
{
    // private fields
    private readonly List<Campaign> _campaigns = [];

    // properties
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string? DescriptionAr { get; private set; } = string.Empty;
    public string? DescriptionEn { get; private set; } = string.Empty;

    // Audit properties => interceptors handle these properties
    public DateTimeOffset? UpdatedAt { get;  set; }
    public DateTimeOffset CreatedAt { get;  set; }

    // Navigation property for related campaigns
    public IReadOnlyCollection<Campaign> Campaigns => _campaigns;

    private Category(){} // for ef core

    private Category(string nameAr, string nameEn, string? descriptionAr, string? descriptionEn)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
    }

    public static Category Create(string nameAr, string nameEn, string? descriptionAr, string? descriptionEn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameAr);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        return new Category(nameAr, nameEn, descriptionAr, descriptionEn);
    }

    public static Category Create(
        string nameAr, 
        string nameEn, 
        string? descriptionAr, 
        string? descriptionEn, 
        List<Campaign> campaigns)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameAr);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);

        
        return new Category(nameAr, nameEn, descriptionAr, descriptionEn);
    }


    public void Update(string? nameAr, string? nameEn, string? descriptionAr, string? descriptionEn)
    {

        NameAr = string.IsNullOrEmpty(nameAr) || string.IsNullOrWhiteSpace(nameAr) ? NameAr : nameAr;
        NameEn = string.IsNullOrEmpty(nameEn) || string.IsNullOrWhiteSpace(nameEn) ? NameEn : nameEn ;
        DescriptionAr = string.IsNullOrEmpty(descriptionAr) || string.IsNullOrWhiteSpace(descriptionAr) ? DescriptionAr : descriptionAr;
        DescriptionEn = string.IsNullOrEmpty(descriptionEn) || string.IsNullOrWhiteSpace(descriptionEn) ? DescriptionEn : descriptionEn;
    }



}
