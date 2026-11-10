using DonationHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonationHub.Infrastructure.Interceptors;

public class AuditableInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{

    private void UpdateAuditProperties(DbContext? context)
    {
        if (context == null)
        {
            return;
        }
        var utcNow = timeProvider.GetUtcNow();
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added &&
                entry.Entity is ICreatedAt createdAtEntity)
            {
                createdAtEntity.CreatedAt = utcNow;
            }

            if (entry.State == EntityState.Modified &&
                entry.Entity is IUpdatedAt updatedAtEntity)
            {
                updatedAtEntity.UpdatedAt = utcNow;
            }
        }
    }


    override public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;
        UpdateAuditProperties(context);
        return result;
    }


    override public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        UpdateAuditProperties(context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
