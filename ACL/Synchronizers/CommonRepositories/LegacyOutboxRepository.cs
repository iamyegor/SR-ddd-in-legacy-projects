using System.Data.SqlClient;

namespace ACL.Synchronizers.CommonRepositories;

public class LegacyOutboxRepository
{
    public void Save<T>(IEnumerable<T> objectsToSave, SqlTransaction transaction)
    {
        throw new NotImplementedException();
    }
}
