using DonationHub.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace DonationHub.Infrastructure.Repositories;

public class EfDatabaseTransaction(IDbContextTransaction contextTransaction) : IDatabaseTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return contextTransaction.CommitAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await contextTransaction.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return contextTransaction.RollbackAsync(cancellationToken);
    }
}
