using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Infrastructure;

public class SyncNeededInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not ApplicationContext context)
        {
            return result;
        }

        SetSyncNeeded(context);

        return result;
    }

    private void SetSyncNeeded(ApplicationContext context)
    {
        bool anythingWasMarkedAsIsSyncNeeded = false;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is ISyncNeeded syncNeeded)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    syncNeeded.IsSyncNeeded = true;
                    anythingWasMarkedAsIsSyncNeeded = true;
                }
            }
        }

        if (anythingWasMarkedAsIsSyncNeeded)
        {
            context.Synchronization.MarkIsSyncRequired("Delivery");
        }
    }
}
