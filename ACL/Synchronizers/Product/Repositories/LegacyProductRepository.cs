using System.Data.SqlClient;
using ACL.Synchronizers.Product.Models;
using Dapper;

namespace ACL.Synchronizers.Product.Repositories;

public class LegacyProductRepository
{
    public List<ProductInLegacy> GetUpdatedAndResetFlags(SqlTransaction transaction)
    {
        List<ProductInLegacy> productsInLegacy = GetAndLockProductsForSync(transaction);
        ResetSyncFlags(productsInLegacy.Select(p => p.NMB_CM), transaction);

        return productsInLegacy;
    }

    private List<ProductInLegacy> GetAndLockProductsForSync(SqlTransaction transaction)
    {
        string query = "select * from PRD_TBL with (updlock) where is_sync_needed = 1";

        SqlConnection connection = transaction.Connection!;
        return connection.Query<ProductInLegacy>(query, transaction: transaction).ToList();
    }

    private void ResetSyncFlags(IEnumerable<int> ids, SqlTransaction transaction)
    {
        string query = "update PRD_TBL set is_sync_needed = 0 where NMB_CM in @ids";

        SqlConnection connection = transaction.Connection!;
        connection.Execute(query, new { ids }, transaction: transaction);
    }
}
