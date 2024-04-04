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
        bool isSyncNeededRaised = false;
        foreach (var entityEntry in context.ChangeTracker.Entries())
        {
            if (entityEntry.Entity is Delivery syncNeeded)
            {
                if (
                    entityEntry.State == EntityState.Added
                    || entityEntry.State == EntityState.Modified
                )
                {
                    syncNeeded.IsSyncNeeded = true;
                    isSyncNeededRaised = true;
                }
            }
        }

        if (isSyncNeededRaised)
        {
            context.Sync.Single(s => s.Name == "Delivery").IsSyncRequired = true;
        }
    }
}
