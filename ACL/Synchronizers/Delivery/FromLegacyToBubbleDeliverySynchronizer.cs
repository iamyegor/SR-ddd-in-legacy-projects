using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace ACL.Synchronizers.Delivery;

public class FromLegacyToBubbleDeliverySynchronizer
{
    public void Sync()
    {
        if (!IsSyncFromLegacyNeeded())
        {
            return;
        }

        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    List<DeliveryInLegacy> deliveriesFromLegacy = GetUpdatedDeliveriesFromLegacy(
                        connection,
                        transaction
                    );

                    List<DeliveryInBubble> deliveriesToSave = MapLegacyDeliveries(
                        deliveriesFromLegacy
                    );

                    SaveDeliveriesToOutbox(deliveriesToSave, connection, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }
    }

    private bool IsSyncFromLegacyNeeded()
    {
        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            string query =
                "SELECT IsSyncRequired FROM [dbo].[Synchronization] WHERE Name = 'Delivery'";
            return connection.Query<bool>(query).Single();
        }
    }

    private List<DeliveryInLegacy> GetUpdatedDeliveriesFromLegacy(
        SqlConnection connection,
        SqlTransaction transaction
    )
    {
        byte[] syncVersion = connection.QuerySingle<byte[]>(
            "SELECT RowVersion FROM Synchronization WHERE Name = 'Delivery'",
            transaction: transaction
        );

        string tempDeliveriesToSyncTableQuery =
            @"
            SELECT d.NMB_CLM, a.*
            INTO #deliveries_to_sync
            FROM [dbo].[DLVR_TBL] d with (UPDLOCK)
            INNER JOIN [dbo].[ADDR_TBL] a on a.DLVR = d.NMB_CLM
            WHERE d.IsSyncNeeded = 1";
        connection.Execute(tempDeliveriesToSyncTableQuery, transaction: transaction);

        List<DeliveryInLegacy> deliveriesFromLegacy = connection
            .Query<DeliveryInLegacy>("SELECT * FROM #deliveries_to_sync", transaction: transaction)
            .ToList();

        string setIsSyncNeededToFalseInDeliveriesQuery =
            @"
            UPDATE [dbo].[DLVR_TBL]
            SET IsSyncNeeded = 0
            WHERE NMB_CLM IN (SELECT NMB_CLM FROM #deliveries_to_sync)";
        connection.Execute(setIsSyncNeededToFalseInDeliveriesQuery, transaction: transaction);

        string setIsSyncRequiredQuery =
            @"
            UPDATE [dbo].[Synchronization]
            SET IsSyncRequired = 0
            WHERE Name = 'Delivery' AND RowVersion = @syncVersion";

        int rowsAffected = connection.Execute(
            setIsSyncRequiredQuery,
            new { syncVersion },
            transaction: transaction
        );

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Synch version conflict");
        }

        return deliveriesFromLegacy;
    }

    private List<DeliveryInBubble> MapLegacyDeliveries(List<DeliveryInLegacy> deliveriesFromLegacy)
    {
        return deliveriesFromLegacy.Select(MapLegacyDelivery).ToList();
    }

    private DeliveryInBubble MapLegacyDelivery(DeliveryInLegacy deliveryFromLegacy)
    {
        if (deliveryFromLegacy.CT_ST == null || !deliveryFromLegacy.CT_ST.Contains(' '))
        {
            throw new Exception("Invalid city and state");
        }

        string[] cityAndState = deliveryFromLegacy.CT_ST.Split(' ');

        return new DeliveryInBubble
        {
            Id = deliveryFromLegacy.NMB_CLM,
            DestinationStreet = (deliveryFromLegacy.STR ?? "").Trim(),
            DestinationCity = cityAndState[0].Trim(),
            DestinationState = cityAndState[1].Trim(),
            DestinationZipCode = (deliveryFromLegacy.ZP ?? "").Trim()
        };
    }

    private void SaveDeliveriesToOutbox(
        List<DeliveryInBubble> deliveriesInBubble,
        SqlConnection connection,
        SqlTransaction transaction
    )
    {
        var deliveriesToSave = deliveriesInBubble.Select(d => new
        {
            Content = JsonConvert.SerializeObject(d)
        });

        string query =
            @"
                 INSERT INTO outbox (content, type)
                 VALUES (@Content, 'Delivery')";

        connection.Execute(query, deliveriesToSave, transaction: transaction);
    }
}
