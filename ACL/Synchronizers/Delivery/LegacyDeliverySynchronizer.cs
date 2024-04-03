using System.Data.SqlClient;
using ACL.Synchronizers.CommonRepositories;
using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;
using ACL.Synchronizers.Product;
using ACL.Utils;
using AutoMapper;
using Dapper;

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
            List<DeliveryInLegacy> deliveriesFromLegacy = GetUpdateDeliveriesFromLegacy(
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
        using var connection = new SqlConnection(LegacyConnectionString.Value);
        string query = "select IsSyncRequired from Synchronization where Name = 'Delivery'";

        return connection.QuerySingle<bool>(query);
    }

    private List<DeliveryInLegacy> GetUpdateDeliveriesFromLegacy(
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

        deliveryRepository.SetSyncFlagFalseForQueriedDeliveries();
        synchronizationRepository.SetSyncFlagFalse("Delivery", syncRowVersion);

        return deliveries;
    }
}
