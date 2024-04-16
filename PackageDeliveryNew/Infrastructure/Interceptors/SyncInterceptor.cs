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
        bool isFlagRaised = false;
        foreach (var deliveryEntry in context.ChangeTracker.Entries<Delivery>())
        {
            if (
                deliveryEntry.State == EntityState.Added
                || deliveryEntry.State == EntityState.Modified
            )
            {
                deliveryEntry.Entity.IsSyncNeeded = true;
                isFlagRaised = true;
            }
        }

        if (isFlagRaised)
        {
            context.Sync.Single(s => s.Name == "Delivery").IsSyncRequired = true;
        }
    }
}
