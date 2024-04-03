using Microsoft.EntityFrameworkCore;
using PackageDeliveryNew.Deliveries;

namespace PackageDeliveryNew.Infrastructure.Repositories;

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

    public void Save(Delivery delivery)
    {
        using var context = new ApplicationContext();

        Delivery? existingDelivery = context.Deliveries.Find(delivery.Id);

        if (existingDelivery == null)
        {
            context.Deliveries.Add(delivery);
        }
        else
        {
            context.Entry(existingDelivery).CurrentValues.SetValues(delivery);

            AddOrUpdateProductLines(delivery, existingDelivery, context);

            RemoveProductLines(delivery, existingDelivery, context);

            MarkProductsUnchanged(existingDelivery, context);
        }

        context.SaveChanges();
    }

    private void AddOrUpdateProductLines(
        Delivery delivery,
        Delivery existingDelivery,
        ApplicationContext context
    )
    {
        foreach (var productLine in delivery.ProductLines)
        {
            ProductLine? existingProductLine = existingDelivery.ProductLines.SingleOrDefault(pl =>
                pl.Id == productLine.Id
            );

            if (existingProductLine == null)
            {
                existingDelivery.AddProductLine(productLine.Product, productLine.Amount);
            }
            else
            {
                context.Entry(existingProductLine).CurrentValues.SetValues(productLine);
            }
        }
    }

    private static void RemoveProductLines(
        Delivery delivery,
        Delivery existingDelivery,
        ApplicationContext context
    )
    {
        foreach (var existingProductLine in existingDelivery.ProductLines)
        {
            bool isProductLineRemoved = delivery.ProductLines.All(pl =>
                pl.Id != existingProductLine.Id
            );

            if (isProductLineRemoved)
            {
                context.Remove(existingProductLine);
            }
        }
    }

    private void MarkProductsUnchanged(Delivery existingDelivery, ApplicationContext context)
    {
        IEnumerable<Product> uniqueProducts = existingDelivery
            .ProductLines.Select(pl => pl.Product)
            .GroupBy(p => p.Id)
            .Select(group => group.First());

        foreach (var product in uniqueProducts)
        {
            context.Entry(product).State = EntityState.Unchanged;
        }
    }
}
