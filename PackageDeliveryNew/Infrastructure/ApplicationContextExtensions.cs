using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure;

public static class ApplicationContextExtensions
{
    public static int GetIdOf(this ApplicationContext context, ProductLine productLine)
    {
        return context.Entry(productLine).Property<int>("id").CurrentValue;
    }
}
