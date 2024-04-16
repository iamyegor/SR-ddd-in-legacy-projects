using ACL.ConnectionStrings;
using ACL.Synchronizers.CommonRepositories.Outbox;
using ACL.Synchronizers.CommonRepositories.Synchronization;
using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;
using Dapper;
using Mapster;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

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
        catch (Exception)
        {
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