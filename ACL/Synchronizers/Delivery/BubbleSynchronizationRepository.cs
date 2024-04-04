using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class BubbleSynchronizationRepository
{
    public BubbleSynchronizationRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public int GetRowVersionFor(string name)
    {
        throw new NotImplementedException();
    }

    public void SetSyncFlagFalse(string name, int rowVersion)
    {
        throw new NotImplementedException();
    }
}