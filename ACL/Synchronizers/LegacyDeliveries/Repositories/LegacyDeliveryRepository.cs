using System.Data.SqlClient;
using ACL.Synchronizers.LegacyDeliveries.Models;
using Dapper;

namespace ACL.Synchronizers.LegacyDeliveries.Repositories;

public class LegacyDeliveryRepository
{
    public List<DeliveryInLegacy> GetUpdatedAndResetFlags(SqlTransaction transaction)
    {
        List<DeliveryInLegacy> deliveries = GetDeliveriesForSync(transaction);
        ResetSyncFlags(deliveries, transaction);

        return deliveries;
    }

    private void ResetSyncFlags(List<DeliveryInLegacy> deliveries, SqlTransaction transaction)
    {
        string query = "update DLVR_TBL set is_sync_needed = 0 where NMB_CLM in @Ids";

        SqlConnection connection = transaction.Connection!;
        connection.Execute(
            query,
            new { Ids = deliveries.Select(d => d.NMB_CLM) },
            transaction: transaction
        );
    }

    public List<DeliveryInLegacy> GetDeliveriesForSync(SqlTransaction transaction)
    {
        string query =
            @"
            select d.*, a.* 
            from DLVR_TBL d with (updlock) 
            inner join ADDR_TBL a on d.NMB_CLM = a.DLVR
            where is_sync_needed = 1";

        SqlConnection connection = transaction.Connection!;
        return connection.Query<DeliveryInLegacy>(query, transaction: transaction).ToList();
    }
}
