using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PackageDeliveryNew.Common;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure;

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

        bool anythingWasSoftDeleted = false;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDelete delete })
            {
                continue;
            }

            anythingWasSoftDeleted = true;
            entry.State = EntityState.Modified;
            delete.IsDeleted = true;
        }

        if (anythingWasSoftDeleted)
        {
            context.Synchronization.MarkIsSyncRequired("Delivery");
        }

        return result;
    }
}
