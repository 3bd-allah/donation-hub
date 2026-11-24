using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DonationHub.Domain.Interfaces;

namespace DonationHub.Infrastructure.Interceptors;

public class SoftDeleteInterceptor (TimeProvider timeProvider) : SaveChangesInterceptor
{

    private void SoftDeleteEntity (DbContext? context)
    {
        if (context == null)
        {
            return ;
        }
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletable entity })
                continue;

            // to restore owned types from state delete to unchanged
            var ownedEntries = entry.References
                .Select(reference => reference.TargetEntry)
                .Where(target => target?.Metadata.IsOwned() == true)
                .ToList();

            entry.State = EntityState.Modified;

            foreach (var ownedEntry in ownedEntries)
                ownedEntry!.State = EntityState.Unchanged;

            
            entity.IsDeleted = true;
            entity.DeletedAtUTC = timeProvider.GetUtcNow();
        }
    }
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        SoftDeleteEntity(context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        SoftDeleteEntity(context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
