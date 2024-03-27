using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Outbox.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Npgsql;

namespace ACL.Synchronizers.Outbox;

public class BubbleOutboxDeliverySynchronizer
{
    public void Sync()
    {
        (List<int> outboxIds, List<DeliveryInLegacy> deliveriesFromOutbox) =
            GetDeliveriesFromOutbox();

        if (outboxIds.Count != 0)
        {
            SaveDeliveries(deliveriesFromOutbox);
            RemoveDeliveriesFromOutbox(outboxIds);
        }
    }

    private (List<int>, List<DeliveryInLegacy>) GetDeliveriesFromOutbox()
    {
        using (var connection = new NpgsqlConnection(BubbleConnectionString.Value))
        {
            string query =
                @"
                SELECT id as Id, content as Content
                FROM outbox
                WHERE type='Delivery' LIMIT 20";

            List<OutboxModel> idsAndDeliveries = connection.Query<OutboxModel>(query).ToList();

            List<DeliveryInLegacy> deliveriesToReturn = [];
            foreach (var deliveryJson in idsAndDeliveries.Select(x => x.Content))
            {
                DeliveryInLegacy? delivery = JsonConvert.DeserializeObject<DeliveryInLegacy>(
                    deliveryJson
                );

                if (delivery == null)
                {
                    throw new Exception("Delivery is null");
                }

                deliveriesToReturn.Add(delivery);
            }

            List<int> idsToReturn = idsAndDeliveries.Select(x => x.Id).ToList();

            return (idsToReturn, deliveriesToReturn);
        }
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

        using (var connection = new SqlConnection(LegacyConnectionString.Value))
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

        using (var connection = new NpgsqlConnection(BubbleConnectionString.Value))
        {
            connection.Execute(query, outboxIds.Select(x => new { Id = x }));
        }
    }
}
