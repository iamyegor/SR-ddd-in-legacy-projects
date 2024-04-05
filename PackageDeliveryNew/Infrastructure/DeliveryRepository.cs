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
                .Include(d => d.ProductLines.Where(pl => !pl.IsDeleted))
                .ThenInclude(pl => pl.Product)
                .SingleOrDefault();
        }
    }

    public void SaveDelivery(Delivery delivery)
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

            MarkProductsAsUnchanged(existingDelivery, context);
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

            if (existingProductLine == null || productLine.Id == 0)
            {
                existingDelivery.AddProductLine(productLine.Product, productLine.Amount);
            }
            else
            {
                context.Entry(existingProductLine).CurrentValues.SetValues(productLine);
            }
        }
    }

    private void RemoveProductLines(
        Delivery delivery,
        Delivery existingDelivery,
        ApplicationContext context
    )
    {
        foreach (var existingProductLine in existingDelivery.ProductLines)
        {
            if (delivery.ProductLines.All(pl => pl.Id != existingProductLine.Id))
            {
                context.Remove(existingProductLine);
            }
        }
    }

    private void MarkProductsAsUnchanged(Delivery existingDelivery, ApplicationContext context)
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
