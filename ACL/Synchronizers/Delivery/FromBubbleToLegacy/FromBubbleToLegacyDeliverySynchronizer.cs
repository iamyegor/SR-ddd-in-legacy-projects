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

        List<DeliveryInBubble>? deliveriesFromBubble = GetUpdatedDeliveriesFromBubble();
        if (deliveriesFromBubble == null || deliveriesFromBubble.Count == 0)
        {
            return;
        }

        List<DeliveryInLegacy> deliveriesToSave = MapBubbleDeliveries(deliveriesFromBubble);

        SaveDeliveries(deliveriesToSave);
    }

    private bool IsSyncNeeded()
    {
        using (var connection = new NpgsqlConnection(_bubbleConnectionString))
        {
            string query = "SELECT is_sync_required FROM sync";
            return connection.Query<bool>(query).Single();
        }
    }

    private List<DeliveryInBubble>? GetUpdatedDeliveriesFromBubble()
    {
        using (var connection = new NpgsqlConnection(_bubbleConnectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    List<DeliveryInBubble> updatedDeliveriesFromBubble =
                        GetUpdatedDeliveriesFromBubble(connection, transaction);

                    transaction.Commit();

                    return updatedDeliveriesFromBubble;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }

    private List<DeliveryInBubble> GetUpdatedDeliveriesFromBubble(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction
    )
    {
        string versionQuery = "SELECT version_number FROM sync WHERE name = 'Delivery';";
        int currentVersion = connection.QuerySingle<int>(versionQuery, transaction: transaction);

        string deliveryQuery =
            @"
            SELECT *
            INTO TEMP deliveries_to_sync
            FROM deliveries
            WHERE is_sync_needed = true
            FOR UPDATE;

            SELECT id as Id, 
            cost_estimate as CostEstimate 
            FROM deliveries_to_sync;
            ";
        List<DeliveryInBubble> deliveriesFromBubble = connection
            .Query<DeliveryInBubble>(deliveryQuery, transaction: transaction)
            .ToList();

        string productLinesQuery =
            @"
            SELECT *
            INTO TEMP product_lines_to_sync
            FROM product_lines
            WHERE delivery_id IN (SELECT id FROM deliveries_to_sync)
            FOR UPDATE;

            SELECT product_id as ProductId,
            amount as Amount,
            delivery_id as DeliveryId,
            is_deleted as IsDeleted
            FROM product_lines_to_sync";
        List<ProductLineInBubble> productLinesFromBubble = connection
            .Query<ProductLineInBubble>(productLinesQuery, transaction: transaction)
            .ToList();

        foreach (DeliveryInBubble delivery in deliveriesFromBubble)
        {
            delivery.ProductLines = productLinesFromBubble
                .Where(pl => pl.DeliveryId == delivery.Id)
                .ToList();
        }

        string deleteProductLinesAndUpdateIsSyncNeededQuery =
            @"
            DELETE FROM product_lines 
            WHERE is_deleted = TRUE AND id IN (SELECT id FROM product_lines_to_sync);
            
            UPDATE deliveries 
            SET is_sync_needed = FALSE 
            WHERE id IN (SELECT id FROM deliveries_to_sync);";
        connection.Execute(deleteProductLinesAndUpdateIsSyncNeededQuery, transaction: transaction);

        string setIsSyncRequiredToFalseQuery =
            @"
            UPDATE sync
            SET is_sync_required = false
            WHERE name = 'Delivery' AND version_number = @CurrentVersion;";
        int rowsAffected = connection.Execute(
            setIsSyncRequiredToFalseQuery,
            new { CurrentVersion = currentVersion },
            transaction
        );

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Sync version conflict");
        }

        return deliveriesFromBubble;
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
