using System.Data.SqlClient;
using ACL.ConnectionStrings;
using ACL.Synchronizers.CommonRepositories.Outbox;
using ACL.Synchronizers.CommonRepositories.Synchronization;
using ACL.Synchronizers.LegacyDeliveries.Models;
using ACL.Synchronizers.LegacyDeliveries.Repositories;
using Dapper;
using Mapster;
using Serilog;

namespace ACL.Synchronizers.LegacyDeliveries;

public class LegacyDeliverySynchronizer
{
    private readonly LegacyOutboxRepository _outboxRepository;
    private readonly LegacyDeliveryRepository _deliveryRepository;
    private readonly LegacySynchronizationRepository _syncRepository;

    public LegacyDeliverySynchronizer(
        LegacyOutboxRepository outboxRepository,
        LegacyDeliveryRepository deliveryRepository,
        LegacySynchronizationRepository syncRepository
    )
    {
        _outboxRepository = outboxRepository;
        _deliveryRepository = deliveryRepository;
        _syncRepository = syncRepository;
    }

    public void Sync()
    {
        if (!IsSyncNeeded())
        {
            return;
        }

        using var connection = new SqlConnection(LegacyConnectionString.Value);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            List<DeliveryInLegacy> deliveriesFromLegacy = GetUpdatedDeliveriesFromLegacy(
                transaction
            );
            
            List<DeliveryInBubble> mappedDeliveries = deliveriesFromLegacy.Adapt<
                List<DeliveryInBubble>
            >();

            _outboxRepository.Save(mappedDeliveries, transaction);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "LegacyDeliverySynchronizer caught the exception");
            transaction.Rollback();
        }
    }

    private bool IsSyncNeeded()
    {
        string query = "select is_sync_required from sync where name = 'Delivery'";

        using var connection = new SqlConnection(LegacyConnectionString.Value);
        return connection.QuerySingle<bool>(query);
    }

    private List<DeliveryInLegacy> GetUpdatedDeliveriesFromLegacy(SqlTransaction transaction)
    {
        byte[] syncRowVersion = _syncRepository.GetRowVersionFor("Delivery", transaction);

        List<DeliveryInLegacy> deliveries = _deliveryRepository.GetUpdatedAndResetFlags(
            transaction
        );

        _syncRepository.SetSyncFlagsFalseFor("Delivery", syncRowVersion, transaction);

        return deliveries;
    }
}
