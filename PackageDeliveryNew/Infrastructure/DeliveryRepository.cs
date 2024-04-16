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
            ProductLine? existingProductLine = context.ProductLines.Find(productLine.Id);

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

    private void RemoveProductLines(
        Delivery delivery,
        Delivery existingDelivery,
        ApplicationContext context
    )
    {
        foreach (var productLine in existingDelivery.ProductLines)
        {
            if (delivery.ProductLines.All(pl => pl.Id != productLine.Id))
            {
                context.Remove(productLine);
            }
        }
    }

    private void MarkProductsUnchanged(Delivery existingDelivery, ApplicationContext context)
    {
        List<int> productIds = [];
        foreach (var product in existingDelivery.ProductLines.Select(pl => pl.Product))
        {
            if (!productIds.Contains(product.Id))
            {
                context.Entry(product).State = EntityState.Unchanged;
                productIds.Add(product.Id);
            }
        }
    }
}
