using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DonationHub.Domain.Interfaces;

namespace DonationHub.Infrastructure.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
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

            entry.State = EntityState.Modified;
            entity.Delete();
        }
    }
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        SoftDeleteEntity(context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        SoftDeleteEntity(context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
