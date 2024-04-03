using ACL.Synchronizers.Delivery.Models;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class BubbleProductLineRepository
{
    public BubbleProductLineRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public List<ProductLineInBubble> GetAllThatNeedSync()
    {
        throw new NotImplementedException();
    }
}