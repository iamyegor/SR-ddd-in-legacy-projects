using ACL.ConnectionStrings;
using ACL.Synchronizers.BubbleDeliveries.Models;
using ACL.Synchronizers.BubbleDeliveries.Repositories;
using ACL.Synchronizers.CommonRepositories.Outbox;
using ACL.Synchronizers.CommonRepositories.Synchronization;
using Dapper;
using Mapster;
using Npgsql;
using Serilog;

namespace ACL.Synchronizers.BubbleDeliveries;

public class BubbleDeliverySynchronizer
{
    private readonly BubbleOutboxRepository _outboxRepository;
    private readonly BubbleDeliveryRepository _deliveryRepository;
    private readonly BubbleSynchronizationRepository _syncRepository;

    public BubbleDeliverySynchronizer(
        BubbleOutboxRepository outboxRepository,
        BubbleDeliveryRepository deliveryRepository,
        BubbleSynchronizationRepository syncRepository
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

        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            List<DeliveryInBubble> deliveriesFromBubble = GetUpdatedDeliveriesFromBubble(
                transaction
            );

            List<DeliveryInLegacy> mappedDeliveries = deliveriesFromBubble.Adapt<
                List<DeliveryInLegacy>
            >();

            _outboxRepository.Save(mappedDeliveries, transaction);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "BubbleDeliverySynchronizer caught the exception");
            transaction.Rollback();
        }
    }

    private bool IsSyncNeeded()
    {
        string query = "select is_sync_required from sync where name = 'Delivery'";
        using var connection = new NpgsqlConnection(BubbleConnectionString.Value);

        return connection.QuerySingle<bool>(query);
    }

    private List<DeliveryInBubble> GetUpdatedDeliveriesFromBubble(NpgsqlTransaction transaction)
    {
        int syncRowVersion = _syncRepository.GetRowVersionFor("Delivery", transaction);

        List<DeliveryInBubble> deliveries = _deliveryRepository.GetUpdatedAndResetFlags(
            transaction
        );

        _syncRepository.SetSyncFlagFalse("Delivery", syncRowVersion, transaction);

        return deliveries;
    }
}
