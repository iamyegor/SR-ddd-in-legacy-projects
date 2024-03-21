using ACL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;

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

        List<DeliveryInLegacy> deliveriesFromLegacy = GetUpdatedLegacyDeliveries();
        List<DeliveryInBubble> deliveriesToSave = MapLegacyDeliveries(deliveriesFromLegacy);

        SaveDeliveriesInBubble(deliveriesToSave);
    }

    private void SaveDeliveriesInBubble(List<DeliveryInBubble> deliveriesToSave)
    {
        string query =
            @"
                UPDATE [dbo].[Delivery]
                SET DestinationStreet = @DestinationStreet,
                    DestinationCity = @DestinationCity,
                    DestinationState = @DestinationState,
                    DestinationZipCode = @DestinationZipCode
                WHERE DeliveryID = @DeliveryID;
    
                IF (@@ROWCOUNT = 0)
                BEGIN
                    INSERT [dbo].[Delivery] (DeliveryID, DestinationStreet, DestinationCity, DestinationState, DestinationZipCode)
                    VALUES (@DeliveryID, @DestinationStreet, @DestinationCity, @DestinationState, @DestinationZipCode)
                END;";

        using (var connection = new SqlConnection(_bubbleConnectionString))
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

    private List<DeliveryInLegacy> GetUpdatedLegacyDeliveries()
    {
        using (var connection = new SqlConnection(_legacyConnectionString))
        {
            string query =
                @"
                    SELECT d.NMB_CLM, a.*
                    FROM [dbo].[DLVR_TBL] d with (UPDLOCK)
                    INNER JOIN [dbo].[ADDR_TBL] a on a.DLVR = d.NMB_CLM
                    WHERE d.IsSyncNeeded = 1
    
                    UPDATE [dbo].[DLVR_TBL]
                    SET IsSyncNeeded = 0
                    WHERE IsSyncNeeded = 1
    
                    UPDATE [dbo].[Synchronization]
                    SET IsSyncRequired = 0
                    WHERE Name = 'Delivery'";

            return connection.Query<DeliveryInLegacy>(query).ToList();
        }
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
            DeliveryID = deliveryFromLegacy.NMB_CLM,
            DestinationStreet = (deliveryFromLegacy.STR ?? "").Trim(),
            DestinationCity = cityAndState[0].Trim(),
            DestinationState = cityAndState[1].Trim(),
            DestinationZipCode = (deliveryFromLegacy.ZP ?? "").Trim()
        };
    }
}
