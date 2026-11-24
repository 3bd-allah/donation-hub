using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace DonationHub.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context): base(context)
    {
        _context = context;
    }
    public async Task<Category?> GetCategoryWithCampaigns(Guid categoryId, CancellationToken ct = default)
    {

        var category = await _context.Categories.IgnoreQueryFilters().Where(c => c.Id == categoryId)
            .Include(c => c.Campaigns).FirstOrDefaultAsync(ct);

        return category;
    }
}
