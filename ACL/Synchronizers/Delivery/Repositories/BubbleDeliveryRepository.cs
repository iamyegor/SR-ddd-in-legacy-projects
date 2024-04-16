using ACL.Synchronizers.Delivery.Models;
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
        throw new NotImplementedException();
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
