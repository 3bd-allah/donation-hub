using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;


namespace DonationHub.Infrastructure.Repositories;

public class UnitOfWork (AppDbContext context): IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }


    public async Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new EfDatabaseTransaction(transaction);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken =default)
    {
        await context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}
