using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PackageDeliveryNew.Deliveries.InterfacesForSynchronization;

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

        MarkSoftDeleted(context);

        return result;
    }

    private void MarkSoftDeleted(ApplicationContext context)
    {
        bool isSoftDeleted = false;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is { Entity: ISoftDelete softDelete, State: EntityState.Deleted })
            {
                softDelete.IsDeleted = true;
                isSoftDeleted = true;
            }
        }

        if (isSoftDeleted)
        {
            context.Sync.Single(s => s.Name == "Delivery").IsSyncRequired = true;
        }
    }
}
