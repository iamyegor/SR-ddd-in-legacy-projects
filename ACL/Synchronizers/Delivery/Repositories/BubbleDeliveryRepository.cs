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
        throw new NotImplementedException();
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
