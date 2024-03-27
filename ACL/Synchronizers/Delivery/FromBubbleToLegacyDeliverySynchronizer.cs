using System.Data;
using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using Dapper;
using Newtonsoft.Json;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class FromBubbleToLegacyDeliverySynchronizer
{
    public void Sync()
    {
        if (!IsSyncNeeded())
        {
            return;
        }

        using (var connection = new NpgsqlConnection(BubbleConnectionString.Value))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    List<DeliveryInBubble> deliveriesFromBubble = GetUpdatedDeliveriesFromBubble(
                        connection,
                        transaction
                    );

                    List<DeliveryInLegacy> deliveriesToSave = MapBubbleDeliveries(
                        deliveriesFromBubble
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

    private bool IsSyncNeeded()
    {
        using (var connection = new NpgsqlConnection(BubbleConnectionString.Value))
        {
            string query = "SELECT is_sync_required FROM sync";
            return connection.Query<bool>(query).Single();
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
            FROM deliveries_to_sync;";
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
            throw new DBConcurrencyException("Sync version conflict");
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

    private void SaveDeliveriesToOutbox(
        List<DeliveryInLegacy> deliveriesToSave,
        NpgsqlConnection connection,
        NpgsqlTransaction transaction
    )
    {
        var deliveriesJson = deliveriesToSave.Select(delivery => new
        {
            Content = JsonConvert.SerializeObject(delivery)
        });

        string query =
            @"
            INSERT INTO outbox (content, type) 
            VALUES (@Content::json, 'Delivery')";

        connection.Execute(query, deliveriesJson, transaction: transaction);
    }
}
