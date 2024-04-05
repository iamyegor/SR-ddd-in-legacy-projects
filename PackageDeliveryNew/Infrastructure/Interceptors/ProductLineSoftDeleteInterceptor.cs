using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure.Interceptors;

public class ProductLineSoftDeleteInterceptor : SaveChangesInterceptor
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

        MarkSoftDeleted(context);

        return result;
    }

    private void MarkSoftDeleted(ApplicationContext context)
    {
        List<EntityEntry<ProductLine>> entriesOfDeletedProductLines = context
            .ChangeTracker.Entries<ProductLine>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        if (entriesOfDeletedProductLines.Count == 0)
        {
            return;
        }

        foreach (var entry in entriesOfDeletedProductLines)
        {
            ProductLine productLine = entry.Entity;
            entry.State = EntityState.Modified;
            productLine.IsDeleted = true;

            IEnumerable<Delivery> affectedDeliveries = context
                .ChangeTracker.Entries<Delivery>()
                .Where(e => e.Entity.ProductLines.Any(pl => pl.Id == productLine.Id))
                .Select(e => e.Entity);

            foreach (var delivery in affectedDeliveries)
            {
                delivery.IsSyncNeeded = true;
            }
        }

        context.Sync.Single(s => s.Name == "Delivery").IsSyncRequired = true;
    }
}
