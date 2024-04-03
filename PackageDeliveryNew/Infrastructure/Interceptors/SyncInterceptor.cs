using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure.Interceptors;

public class SyncInterceptor : SaveChangesInterceptor
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

        RaiseSyncFlags(context);

        return result;
    }

    private void RaiseSyncFlags(ApplicationContext context)
    {
        bool isSyncFlagRaised = false;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is ISyncNeeded syncNeeded)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    syncNeeded.IsSyncNeeded = true;
                    isSyncFlagRaised = true;
                }
            }
        }

        if (isSyncFlagRaised)
        {
            context.Synchronization.Single(s => s.Name == "Delivery").IsSyncRequired = true;
        }
    }
}
