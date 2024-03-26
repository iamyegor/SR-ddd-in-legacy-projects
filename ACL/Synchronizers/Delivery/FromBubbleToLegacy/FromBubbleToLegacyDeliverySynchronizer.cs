using System.Numerics;
using ACL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

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
            string query = "SELECT is_sync_required FROM sync";
            return connection.Query<bool>(query).Single();
        }
    }

    private List<DeliveryInBubble> GetUpdatedDeliveriesFromBubble()
    {
        string query1 =
            @"
            SELECT *
            FROM deliveries
            WHERE is_sync_needed = 1
            FOR UPDATE;
    
            SELECT pl.*
            FROM deliveries d 
            INNER JOIN product_lines pl on d.Id = pl.delivery_id
            WHERE d.is_sync_needed = 1;

            SELECT version
            FROM sync
            WHERE name = 'Delivery'";

        List<DeliveryInBubble> retrievedDeliveries;
        
        using (var connection = new NpgsqlConnection(_bubbleConnectionString))
        {
            SqlMapper.GridReader reader = connection.QueryMultiple(query1);
            retrievedDeliveries = reader.Read<DeliveryInBubble>().ToList();
            List<ProductLineInBubble> productLines = reader.Read<ProductLineInBubble>().ToList();

            foreach (var delivery in retrievedDeliveries)
            {
                delivery.ProductLines = productLines
                    .Where(pl => pl.DeliveryId == delivery.Id)
                    .ToList();
            }
        }

        string query2 =
            @"
            DELETE FROM product_lines
            WHERE is_deleted = 1;
        
            UPDATE deliveries
            SET is_sync_needed = 0
            WHERE id = @Id AND version = @Version;
        
            UPDATE sync
            SET is_sync_required = 0";

        using (var connection = new NpgsqlConnection(_bubbleConnectionString))
        {
            connection.Execute(query2, retrievedDeliveries);
        }
    }

    private List<DeliveryInLegacy> MapBubbleDeliveries(List<DeliveryInBubble> deliveriesFromBubble)
    {
        List<DeliveryInLegacy> deliveriesToReturn = [];

        foreach (var deliveryFromBubble in deliveriesFromBubble)
        {
            DeliveryInLegacy legacyDelivery = new DeliveryInLegacy
            {
                NMB_CLM = deliveryFromBubble.Id,
                ESTM_CLM = (double)(deliveryFromBubble.CostEstimate ?? 0),
            };
            if (deliveryFromBubble.ProductLines.Count > 0)
            {
                ProductLineInBubble productLine = deliveryFromBubble.ProductLines[0];
                bool isDeleted = productLine.IsDeleted;

                legacyDelivery.PRD_LN_1 = isDeleted ? null : productLine.ProductId;
                legacyDelivery.PRD_LN_1_AMN = isDeleted ? null : productLine.Amount.ToString();
            }

            if (deliveryFromBubble.ProductLines.Count > 1)
            {
                ProductLineInBubble productLine = deliveryFromBubble.ProductLines[1];
                bool isDeleted = productLine.IsDeleted;

                legacyDelivery.PRD_LN_2 = isDeleted ? null : productLine.ProductId;
                legacyDelivery.PRD_LN_2_AMN = isDeleted ? null : productLine.Amount.ToString();
            }

            if (deliveryFromBubble.ProductLines.Count > 2)
            {
                ProductLineInBubble productLine = deliveryFromBubble.ProductLines[2];
                bool isDeleted = productLine.IsDeleted;

                legacyDelivery.PRD_LN_3 = isDeleted ? null : productLine.ProductId;
                legacyDelivery.PRD_LN_3_AMN = isDeleted ? null : productLine.Amount.ToString();
            }

            if (deliveryFromBubble.ProductLines.Count > 3)
            {
                ProductLineInBubble productLine = deliveryFromBubble.ProductLines[3];
                bool isDeleted = productLine.IsDeleted;

                legacyDelivery.PRD_LN_4 = isDeleted ? null : productLine.ProductId;
                legacyDelivery.PRD_LN_4_AMN = isDeleted ? null : productLine.Amount.ToString();
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
