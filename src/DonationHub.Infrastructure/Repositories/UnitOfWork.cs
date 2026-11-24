using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Infrastructure.Exceptions;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;


namespace DonationHub.Infrastructure.Repositories;

public sealed class UnitOfWork (AppDbContext context): IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction; 
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }


    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            throw new InfrastructureException("A transaction is already active.");
        _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }
    

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InfrastructureException("No actvie transaction exist");
        
        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null; 
    }
    

    public async Task RollbackAsync(CancellationToken cancellationToken =default)
    {
        if (_currentTransaction is null)
            throw new InfrastructureException("No active transaction exist");
        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

}
