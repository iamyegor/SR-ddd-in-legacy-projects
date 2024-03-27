using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Outbox.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Npgsql;

namespace ACL.Synchronizers.Outbox;

public class LegacyOutboxDeliverySynchronizer
{
    public void Sync()
    {
        (List<int> outboxIds, List<DeliveryInBubble> deliveriesFromOutbox) =
            GetDeliveriesFromOutbox();

        if (outboxIds.Count != 0)
        {
            SaveDeliveriesInBubble(deliveriesFromOutbox);
            RemoveDeliveriesFromOutbox(outboxIds);
        }
    }

    private (List<int>, List<DeliveryInBubble>) GetDeliveriesFromOutbox()
    {
        string query =
            @"
            SELECT TOP 20 id as Id, content as Content
            FROM outbox
            WHERE type = 'Delivery'";

        List<OutboxModel> idsAndDeliveries;
        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            idsAndDeliveries = connection.Query<OutboxModel>(query).ToList();
        }

        List<DeliveryInBubble> deliveriesToReturn = [];
        foreach (var deliveryJson in idsAndDeliveries.Select(x => x.Content))
        {
            DeliveryInBubble? deliveryInBubble = JsonConvert.DeserializeObject<DeliveryInBubble>(
                deliveryJson
            );

            if (deliveryInBubble == null)
            {
                throw new Exception("Delivery is null");
            }

            deliveriesToReturn.Add(deliveryInBubble);
        }

        List<int> idsToReturn = idsAndDeliveries.Select(x => x.Id).ToList();

        return (idsToReturn, deliveriesToReturn);
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

        using (var connection = new NpgsqlConnection(BubbleConnectionString.Value))
        {
            connection.Execute(query, deliveriesToSave);
        }
    }

    private void RemoveDeliveriesFromOutbox(List<int> outboxIds)
    {
        string query =
            @"
            DELETE FROM outbox
            WHERE id = @Id";

        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            connection.Execute(query, outboxIds.Select(x => new { Id = x }));
        }
    }
}
