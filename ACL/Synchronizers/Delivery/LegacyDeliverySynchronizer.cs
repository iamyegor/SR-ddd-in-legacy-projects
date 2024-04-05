using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;
using ACL.Synchronizers.Product.Repositories;
using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ACL.Synchronizers.Delivery;

public class LegacyDeliverySynchronizer
{
    private readonly IMapper _mapper;

    public LegacyDeliverySynchronizer(IMapper mapper)
    {
        _mapper = mapper;
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
                connection,
                transaction
            );

            List<DeliveryInBubble> mappedDeliveries = _mapper.Map<List<DeliveryInBubble>>(
                deliveriesFromLegacy
            );

            var outboxRepository = new LegacyOutboxRepository(connection, transaction);
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
        string query = "select IsSyncRequired from Synchronization where Name = 'Delivery'";
        using var connection = new SqlConnection(LegacyConnectionString.Value);
        return connection.QuerySingle<bool>(query);
    }

    private List<DeliveryInLegacy> GetUpdatedDeliveriesFromLegacy(
        SqlConnection connection,
        SqlTransaction transaction
    )
    {
        var deliveryRepository = new LegacyDeliveryRepository(connection, transaction);
        var synchronizationRepository = new LegacySynchronizationRepository(
            connection,
            transaction
        );

        byte[] syncRowVersion = synchronizationRepository.GetRowVersionFor("Delivery");

        List<DeliveryInLegacy> deliveries = deliveryRepository.GetAllThatNeedSync();

        deliveryRepository.SetSyncFlagFalse();
        synchronizationRepository.SetSyncFlagFalseFor("Delivery", syncRowVersion);

        return deliveries;
    }
}
