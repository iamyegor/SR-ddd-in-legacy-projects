using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure;

public class MarkIsSyncRequiredInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context == null)
        {
            throw new NullReferenceException("DbContext can not be null");
        }

        SetSyncRequired(eventData.Context);
        return result;
    }

    private void SetSyncRequired(DbContext context)
    {
        IEnumerable<Delivery> deliveries = context
            .ChangeTracker.Entries<Delivery>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .Where(d => d.IsSyncNeeded);

        if (deliveries.Any())
        {
            Synchronization sync = context.Set<Synchronization>().Single(s => s.Name == "Delivery");
            sync.IsSyncRequired = true;

            if (context.Entry(sync).State == EntityState.Detached)
            {
                context.Add(sync);
            }
        }
    }
}
