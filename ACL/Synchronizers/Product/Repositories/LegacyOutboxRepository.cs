using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Product.Repositories;

public class LegacyOutboxRepository
{
    public LegacyOutboxRepository(SqlConnection connection, SqlTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public void Save(IEnumerable<object> objectsToSave, string type)
    {
        throw new NotImplementedException();
    }
}