using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery.Repositories;

public class BubbleDeliveryRepository
{
    public List<DeliveryInBubble> GetUpdatedAndResetFlags(NpgsqlTransaction transaction)
    {
        (List<int> deliveryIds, List<int> productLineIds) = LockDeliveriesAndProductLines(
            transaction
        );

        List<DeliveryInBubble> deliveries = GetDeliveriesForSync(deliveryIds, transaction);

        DeleteSoftDeletedProductLines(productLineIds, transaction);
        ResetSyncFlags(deliveryIds, transaction);

        return deliveries;
    }

    private (List<int> deliveryIds, List<int> productLineIds) LockDeliveriesAndProductLines(
        NpgsqlTransaction transaction
    )
    {
        string deliveriesQuery =
            "select id from deliveries where is_sync_needed = true for update;";

        NpgsqlConnection connection = transaction.Connection!;
        List<int> deliveryIds = connection
            .Query<int>(deliveriesQuery, transaction: transaction)
            .ToList();

        if (deliveryIds.Count == 0)
        {
            return ([], []);
        }

        string productLinesQuery =
            "select id from product_lines where delivery_id = any(@deliveryIds) for update;";

        List<int> productLineIds = connection
            .Query<int>(productLinesQuery, new { deliveryIds }, transaction: transaction)
            .ToList();

        return (deliveryIds, productLineIds);
    }

    private List<DeliveryInBubble> GetDeliveriesForSync(
        List<int> deliveryIds,
        NpgsqlTransaction transaction
    )
    {
        string query =
            @"
            select d.*, pl.id as pl_id, pl.*
            from deliveries d
            inner join product_lines pl on d.id = pl.delivery_id and pl.is_deleted = false
            where d.id = any(@deliveryIds)";

        var deliveryDictionary = new Dictionary<int, DeliveryInBubble>();

        NpgsqlConnection connection = transaction.Connection!;
        IEnumerable<DeliveryInBubble> deliveries = connection
            .Query<DeliveryInBubble, ProductLineInBubble, DeliveryInBubble>(
                query,
                (delivery, productLine) =>
                {
                    if (!deliveryDictionary.TryGetValue(delivery.Id, out var existingDelivery))
                    {
                        existingDelivery = delivery;
                        deliveryDictionary.Add(existingDelivery.Id, existingDelivery);
                    }

                    existingDelivery.ProductLines.Add(productLine);

                    return existingDelivery;
                },
                param: new { deliveryIds },
                splitOn: "pl_id",
                transaction: transaction
            )
            .Distinct();

        return deliveries.ToList();
    }

    private void DeleteSoftDeletedProductLines(
        List<int> productLineIds,
        NpgsqlTransaction transaction
    )
    {
        throw new NotImplementedException();
    }

    private void ResetSyncFlags(List<int> deliveryIds, NpgsqlTransaction transaction)
    {
        throw new NotImplementedException();
    }
}
