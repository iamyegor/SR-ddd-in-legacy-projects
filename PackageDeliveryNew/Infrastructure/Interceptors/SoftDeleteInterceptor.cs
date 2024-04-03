using Microsoft.EntityFrameworkCore;
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
        bool isSoftDeleted = false;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Deleted, Entity: ISoftDelete softDelete })
            {
                softDelete.IsDeleted = true;
                isSoftDeleted = true;
            }
        }

        if (isSoftDeleted)
        {
            context.Synchronization.Single(s => s.Name == "Delivery").IsSyncRequired = true;
        }
    }
}
