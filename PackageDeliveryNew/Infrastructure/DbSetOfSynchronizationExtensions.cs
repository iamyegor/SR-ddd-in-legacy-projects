using Microsoft.EntityFrameworkCore;

namespace PackageDeliveryNew.Infrastructure;

public static class DbSetOfSynchronizationExtensions
{
    public static void MarkIsSyncRequired(this DbSet<Synchronization> sync, string name)
    {
        Synchronization synchronization = sync.Single(s => s.Name == name);
        synchronization.IsSyncRequired = true;
    }
}
