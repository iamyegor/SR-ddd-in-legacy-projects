﻿using Microsoft.EntityFrameworkCore;
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
        using (var context = new ApplicationContext())
        {
            context.ChangeTracker.TrackGraph(
                delivery,
                e =>
                {
                    if (e.Entry.Entity.GetType() == typeof(Delivery))
                    {
                        e.Entry.State = EntityState.Modified;
                    }
                    else if (e.Entry.IsKeySet)
                    {
                        e.Entry.State = EntityState.Unchanged;
                    }
                    else
                    {
                        e.Entry.State = EntityState.Added;
                    }
                }
            );
            
            List<Guid> removedProductLinesIds = delivery
                .PopRemovedProductLines()
                .Select(pl => pl.Id)
                .ToList();

            List<ProductLine> productLinesFromDb = context
                .ProductLines.Where(pl => removedProductLinesIds.Contains(pl.Id))
                .ToList();

            context.ProductLines.RemoveRange(productLinesFromDb);

            context.SaveChanges();
        }
    }
}