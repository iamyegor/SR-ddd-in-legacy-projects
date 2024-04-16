using System.Data.SqlClient;
using ACL.Synchronizers.Product.Models;

namespace ACL.Synchronizers.Product.Repositories;

public class LegacyProductRepository
{
    public List<ProductInLegacy> GetUpdatedAndResetFlags(SqlTransaction transaction)
    {
        throw new NotImplementedException();
    }
}
