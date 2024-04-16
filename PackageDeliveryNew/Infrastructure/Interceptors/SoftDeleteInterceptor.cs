using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
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

        SoftDelete(context);

        return result;
    }

    private void SoftDelete(ApplicationContext context)
    {
        List<EntityEntry<ProductLine>> deletedProductLineEntries = context
            .ChangeTracker.Entries<ProductLine>()
            .Where(pl => pl.State == EntityState.Deleted)
            .ToList();

        foreach (var deletedProductLineEntry in deletedProductLineEntries)
        {
            deletedProductLineEntry.State = EntityState.Modified;
            ProductLine productLine = deletedProductLineEntry.Entity;
            productLine.IsDeleted = true;

            Delivery affectedDelivery = context
                .ChangeTracker.Entries<Delivery>()
                .Single(d => d.Entity.ProductLines.Any(pl => pl.Id == productLine.Id))
                .Entity;

            affectedDelivery.IsSyncNeeded = true;
        }

        if (deletedProductLineEntries.Count > 0)
        {
            context.Sync.Single(s => s.Name == "Delivery").IsSyncRequired = true;
        }
    }
}
