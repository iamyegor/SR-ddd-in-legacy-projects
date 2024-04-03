using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;

namespace ACL.Synchronizers.Delivery.OutboxSynchronizers;

public class BubbleOutboxDeliverySynchronizer
{
    private readonly BubbleOutboxRepository _outboxRepository;
    private readonly LegacyDeliveryRepository _legacyDeliveryRepository;

    public BubbleOutboxDeliverySynchronizer(
        BubbleOutboxRepository outboxRepository,
        LegacyDeliveryRepository legacyDeliveryRepository
    )
    {
        _outboxRepository = outboxRepository;
        _legacyDeliveryRepository = legacyDeliveryRepository;
    }

    public void Sync()
    {
        (List<int>? ids, List<DeliveryInLegacy>? deliveriesFromOutbox) =
            _outboxRepository.Get<DeliveryInLegacy>("Delivery");

        if (ids.Count == 0)
        {
            return;
        }

        _legacyDeliveryRepository.Save(deliveriesFromOutbox);
        
        _outboxRepository.Remove(ids);
    }
}
