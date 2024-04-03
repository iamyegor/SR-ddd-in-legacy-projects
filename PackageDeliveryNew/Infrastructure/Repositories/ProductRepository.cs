using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure;

public class ProductRepository
{
    public IReadOnlyList<Product> GetAll()
    {
        using (var context = new ApplicationContext())
        {
            return context.Products.ToList();
        }
    }
}
