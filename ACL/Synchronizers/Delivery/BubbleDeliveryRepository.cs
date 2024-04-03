using ACL.Synchronizers.Delivery.Models;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class BubbleDeliveryRepository
{
    public BubbleDeliveryRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public List<DeliveryInBubble> GetAllThatNeedSync()
    {
        throw new NotImplementedException();
    }

    public void SetSyncFlagsFalseForQueried()
    {
        throw new NotImplementedException();
    }
}