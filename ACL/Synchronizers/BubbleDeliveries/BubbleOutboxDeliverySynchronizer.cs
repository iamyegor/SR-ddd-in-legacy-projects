using ACL.Synchronizers.BubbleDeliveries.Models;
using ACL.Synchronizers.BubbleDeliveries.Repositories;
using ACL.Synchronizers.CommonRepositories.Outbox;

namespace ACL.Synchronizers.BubbleDeliveries;

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
        (List<int> ids, List<DeliveryInLegacy> deliverieFromOutbox) =
            _bubbleOutboxRepository.Get<DeliveryInLegacy>();

        if (ids.Count == 0)
        {
            return;
        }

        _legacyDeliveryRepository.Save(deliverieFromOutbox);

        _bubbleOutboxRepository.Remove(ids);
    }
}