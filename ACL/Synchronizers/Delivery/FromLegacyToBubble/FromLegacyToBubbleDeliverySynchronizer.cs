using ACL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace ACL.Synchronizers.Delivery.FromLegacyToBubble;

public class FromLegacyToBubbleDeliverySynchronizer
{
    private readonly string _legacyConnectionString;
    private readonly string _bubbleConnectionString;

    public FromLegacyToBubbleDeliverySynchronizer(ConnectionStrings connectionStrings)
    {
        _legacyConnectionString = connectionStrings.Legacy;
        _bubbleConnectionString = connectionStrings.Bubble;
    }

    public void Sync()
    {
        if (!IsSyncFromLegacyNeeded())
        {
            return;
        }

        List<DeliveryInLegacy>? deliveriesFromLegacy = GetUpdatedDeliveriesFromLegacy();
        if (deliveriesFromLegacy == null)
        {
            return;
        }

        List<DeliveryInBubble> deliveriesToSave = MapLegacyDeliveries(deliveriesFromLegacy);

        SaveDeliveriesInBubble(deliveriesToSave);
    }

    private void SaveDeliveriesInBubble(List<DeliveryInBubble> deliveriesToSave)
    {
        string query =
            @"
            INSERT INTO deliveries (
	            id,
	            destination_city,
	            destination_state,
	            destination_street,
	            destination_zip_code
	            )
            VALUES (
	            @Id,
	            @DestinationStreet,
	            @DestinationState,
	            @DestinationStreet,
	            @DestinationZipCode
	            ) 
            ON CONFLICT (id) 
            DO UPDATE SET 
                destination_city = EXCLUDED.destination_city,
	            destination_state = EXCLUDED.destination_state,
	            destination_street = EXCLUDED.destination_street,
	            destination_zip_code = EXCLUDED.destination_zip_code";

        using (var connection = new NpgsqlConnection(_bubbleConnectionString))
        {
            connection.Execute(query, deliveriesToSave);
        }
    }

    private bool IsSyncFromLegacyNeeded()
    {
        using (var connection = new SqlConnection(_legacyConnectionString))
        {
            string query =
                "SELECT IsSyncRequired FROM [dbo].[Synchronization] WHERE Name = 'Delivery'";
            return connection.Query<bool>(query).Single();
        }
    }

    private List<DeliveryInLegacy>? GetUpdatedDeliveriesFromLegacy()
    {
        using (var connection = new SqlConnection(_legacyConnectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    List<DeliveryInLegacy> updatedDeliveriesFromLegacy =
                        GetUpdatedDeliveriesFromLegacy(connection, transaction);

                    transaction.Commit();

                    return updatedDeliveriesFromLegacy;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
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
}
