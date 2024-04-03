using ACL.Synchronizers.Delivery.Models;
using Npgsql;

namespace ACL.Synchronizers.Delivery.Repositories;

public class BubbleOutboxRepository
{
    public BubbleOutboxRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public void Save(List<DeliveryInLegacy> mappedDeliveries, string delivery)
    {
        throw new NotImplementedException();
    }
}