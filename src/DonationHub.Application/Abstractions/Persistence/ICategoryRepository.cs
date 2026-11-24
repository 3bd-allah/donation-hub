using DonationHub.Domain.Entities;

namespace DonationHub.Application.Abstractions.Persistence;

public interface ICategoryRepository : IBaseRepository<Category>
{
    /// <summary>
    /// get the category with all its campaigns even that soft-deleted
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<Category?> GetCategoryWithCampaigns(Guid categoryId, CancellationToken ct = default);
}
