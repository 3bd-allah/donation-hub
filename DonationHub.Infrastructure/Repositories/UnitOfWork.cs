using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Infrastructure.Persistence;


namespace DonationHub.Infrastructure.Repositories;

public class UnitOfWork (AppDbContext context): IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}
