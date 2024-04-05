using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;

namespace ACL.Synchronizers.Delivery.OutboxSynchronizers;

public class BubbleOutboxDeliverySynchronizer
{
    private readonly BubbleOutboxRepository _bubbleOutboxRepository;
    private readonly LegacyDeliveryRepository _legacyDeliveryRepository;

    public BubbleOutboxDeliverySynchronizer(
        BubbleOutboxRepository bubbleOutboxRepository,
        LegacyDeliveryRepository legacyDeliveryRepository
    )
    {
        _bubbleOutboxRepository = bubbleOutboxRepository;
        _legacyDeliveryRepository = legacyDeliveryRepository;
    }

    public void Sync()
    {
        (List<int>? ids, List<DeliveryInLegacy>? legacyDeliveries) =
            _bubbleOutboxRepository.Get<DeliveryInLegacy>("Delivery");

        if (ids.Count == 0)
        {
            return;
        }

        _legacyDeliveryRepository.Save(legacyDeliveries);

        _bubbleOutboxRepository.Remove(ids);
    }
}
