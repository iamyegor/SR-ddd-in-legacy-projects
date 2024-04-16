using ACL.Synchronizers.Delivery.Models;
using Npgsql;

namespace ACL.Synchronizers.Delivery.Repositories;

public class BubbleDeliveryRepository
{
    public List<DeliveryInBubble> GetUpdatedAndResetFlags(NpgsqlTransaction transaction)
    {
        throw new NotImplementedException();
    }
}