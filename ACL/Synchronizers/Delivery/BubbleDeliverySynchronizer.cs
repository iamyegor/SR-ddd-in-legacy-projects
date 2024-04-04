using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;
using AutoMapper;
using Dapper;
using Npgsql;

namespace ACL.Synchronizers.Delivery;

public class BubbleDeliverySynchronizer
{
    private readonly IMapper _mapper;

    public BubbleDeliverySynchronizer(IMapper mapper)
    {
        _mapper = mapper;
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
                connection,
                transaction
            );

            List<DeliveryInLegacy> mappedDeliveries = _mapper.Map<List<DeliveryInLegacy>>(
                deliveriesFromBubble
            );

            var outboxRepository = new BubbleOutboxRepository(connection, transaction);
            outboxRepository.Save(mappedDeliveries, "Delivery");

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

    private List<DeliveryInBubble> GetUpdatedDeliveriesFromBubble(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction
    )
    {
        var deliveryRepository = new BubbleDeliveryRepository(connection, transaction);
        var productLineRepository = new BubbleProductLineRepository(connection, transaction);
        var synchronizationRepository = new BubbleSynchronizationRepository(
            connection,
            transaction
        );

        int syncRowVersion = synchronizationRepository.GetRowVersionFor("Delivery");

        List<DeliveryInBubble> deliveries = deliveryRepository.GetAllThatNeedSync();
        List<ProductLineInBubble> productLines = productLineRepository.GetAllThatNeedSync();

        foreach (var delivery in deliveries)
        {
            delivery.ProductLines = productLines.Where(pl => pl.DeliveryId == delivery.Id).ToList();
        }

        deliveryRepository.SetSyncFlagsFalseForQueried();
        synchronizationRepository.SetSyncFlagFalse("Delivery", syncRowVersion);

        return deliveries;
    }
}
