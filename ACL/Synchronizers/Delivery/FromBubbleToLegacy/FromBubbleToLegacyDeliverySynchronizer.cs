using ACL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Delivery.FromBubbleToLegacy;

public class FromBubbleToLegacyDeliverySynchronizer
{
    private readonly string _legacyConnectionString;
    private readonly string _bubbleConnectionString;

    public FromBubbleToLegacyDeliverySynchronizer(ConnectionStrings connectionStrings)
    {
        _legacyConnectionString = connectionStrings.Legacy;
        _bubbleConnectionString = connectionStrings.Bubble;
    }

    public void Sync()
    {
        if (!IsSyncNeeded())
        {
            return;
        }

        List<DeliveryInBubble> deliveriesFromBubble = GetUpdatedDeliveriesFromBubble();
        List<DeliveryInLegacy> deliveriesToSave = MapBubbleDeliveries(deliveriesFromBubble);

        SaveDeliveries(deliveriesToSave);
    }

    private bool IsSyncNeeded()
    {
        using (var connection = new SqlConnection(_bubbleConnectionString))
        {
            string query = "SELECT IsSyncRequired FROM [dbo].[Synchronization]";
            return connection.Query<bool>(query).Single();
        }
    }

    private List<DeliveryInBubble> GetUpdatedDeliveriesFromBubble()
    {
        string query =
            @"
                SELECT *
                FROM [dbo].[Delivery] d with (UPDLOCK)
                WHERE d.IsSyncNeeded = 1
    
                SELECT pl.*
                FROM [dbo].[Delivery] d 
                INNER JOIN dbo.ProductLine pl on d.DeliveryID = pl.DeliveryID
                WHERE d.IsSyncNeeded = 1
    
                UPDATE [dbo].[Delivery]
                SET IsSyncNeeded = 0
                WHERE IsSyncNeeded = 1
                
                UPDATE [dbo].[Synchronization]
                SET IsSyncRequired = 0";

        using (var connection = new SqlConnection(_bubbleConnectionString))
        {
            SqlMapper.GridReader reader = connection.QueryMultiple(query);
            List<DeliveryInBubble> deliveries = reader.Read<DeliveryInBubble>().ToList();
            List<ProductLineInBubble> productLines = reader.Read<ProductLineInBubble>().ToList();

            foreach (var delivery in deliveries)
            {
                delivery.ProductLines = productLines
                    .Where(pl => pl.DeliveryId == delivery.DeliveryId)
                    .ToList();
            }

            return deliveries;
        }
    }

    private List<DeliveryInLegacy> MapBubbleDeliveries(List<DeliveryInBubble> deliveriesFromBubble)
    {
        List<DeliveryInLegacy> deliveriesToReturn = [];

        foreach (var deliveryFromBubble in deliveriesFromBubble)
        {
            DeliveryInLegacy legacyDelivery = new DeliveryInLegacy
            {
                NMB_CLM = deliveryFromBubble.DeliveryId,
                ESTM_CLM = (double)(deliveryFromBubble.CostEstimate ?? 0),
            };
            if (deliveryFromBubble.ProductLines.Count > 0)
            {
                legacyDelivery.PRD_LN_1 = deliveryFromBubble.ProductLines[0].ProductId;
                legacyDelivery.PRD_LN_1_AMN = deliveryFromBubble.ProductLines[0].Amount.ToString();
            }

            if (deliveryFromBubble.ProductLines.Count > 1)
            {
                legacyDelivery.PRD_LN_2 = deliveryFromBubble.ProductLines[1].ProductId;
                legacyDelivery.PRD_LN_2_AMN = deliveryFromBubble.ProductLines[1].Amount.ToString();
            }

            if (deliveryFromBubble.ProductLines.Count > 2)
            {
                legacyDelivery.PRD_LN_3 = deliveryFromBubble.ProductLines[2].ProductId;
                legacyDelivery.PRD_LN_3_AMN = deliveryFromBubble.ProductLines[2].Amount.ToString();
            }

            if (deliveryFromBubble.ProductLines.Count > 3)
            {
                legacyDelivery.PRD_LN_4 = deliveryFromBubble.ProductLines[3].ProductId;
                legacyDelivery.PRD_LN_4_AMN = deliveryFromBubble.ProductLines[3].Amount.ToString();
            }

            deliveriesToReturn.Add(legacyDelivery);
        }

        return deliveriesToReturn;
    }

    private void SaveDeliveries(List<DeliveryInLegacy> deliveriesToSave)
    {
        string query =
            @"
                UPDATE [dbo].[DLVR_TBL]
                SET PRD_LN_1 = @PRD_LN_1, PRD_LN_1_AMN = @PRD_LN_1_AMN, PRD_LN_2 = @PRD_LN_2, 
                    PRD_LN_2_AMN = @PRD_LN_2_AMN, ESTM_CLM = @ESTM_CLM, STS = 'R'
                WHERE NMB_CLM = @NMB_CLM
                
                IF EXISTS (SELECT 1 FROM [dbo].[DLVR_TBL2] WHERE NMB_CLM = @NMB_CLM)
                BEGIN
                    UPDATE [dbo].[DLVR_TBL2]
                    SET PRD_LN_3 = @PRD_LN_3, PRD_LN_3_AMN = @PRD_LN_3_AMN, PRD_LN_4 = @PRD_LN_4, 
                        PRD_LN_4_AMN = @PRD_LN_4_AMN
                    WHERE NMB_CLM = @NMB_CLM
                END
                ELSE
                BEGIN
                    INSERT [dbo].[DLVR_TBL2] (NMB_CLM, PRD_LN_3, PRD_LN_3_AMN, PRD_LN_4, PRD_LN_4_AMN)
                    VALUES (@NMB_CLM, @PRD_LN_3, @PRD_LN_3_AMN, @PRD_LN_4, @PRD_LN_4_AMN)
                END";

        using (var connection = new SqlConnection(_legacyConnectionString))
        {
            connection.Execute(query, deliveriesToSave);
        }
    }
}
