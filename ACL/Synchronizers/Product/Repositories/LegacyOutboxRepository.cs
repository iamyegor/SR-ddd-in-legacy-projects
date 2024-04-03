using ACL.Synchronizers.Product.Models;
using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Product.Repositories;

public class LegacyOutboxRepository
{
    public LegacyOutboxRepository(SqlConnection connection, SqlTransaction transaction)
    {
        throw new NotImplementedException();
    }

    public void Save(List<ProductInBubble> mappedProducts)
    {
        throw new NotImplementedException();
    }
}