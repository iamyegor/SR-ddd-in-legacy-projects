using Microsoft.EntityFrameworkCore;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure;

public class DeliveryRepository
{
    public Delivery? GetById(int id)
    {
        using (var context = new ApplicationContext())
        {
            return context
                .Deliveries.Where(d => d.Id == id)
                .Include(d => d.ProductLines)
                .ThenInclude(pl => pl.Product)
                .SingleOrDefault();
        }
    }

    public void SaveDelivery(Delivery delivery)
    {
        throw new NotImplementedException();
    }
}
