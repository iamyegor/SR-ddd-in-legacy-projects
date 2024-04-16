using Npgsql;

namespace ACL.Synchronizers.CommonRepositories.Outbox;

public class BubbleOutboxRepository
{
    public void Save<T>(IEnumerable<T> objectsToSave, NpgsqlTransaction transaction)
    {
        throw new NotImplementedException();
    }
}