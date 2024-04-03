using ACL.ConnectionStrings;
using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;
using AutoMapper;
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
        throw new NotImplementedException();
    }

    private List<DeliveryInBubble> GetUpdatedDeliveriesFromBubble(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction
    )
    {
        throw new NotImplementedException();
    }
}
