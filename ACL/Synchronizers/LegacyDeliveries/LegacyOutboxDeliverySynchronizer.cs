using ACL.Synchronizers.CommonRepositories.Outbox;
using ACL.Synchronizers.LegacyDeliveries.Models;
using ACL.Synchronizers.LegacyDeliveries.Repositories;

namespace ACL.Synchronizers.LegacyDeliveries;

public class LegacyOutboxDeliverySynchronizer
{
    private readonly LegacyOutboxRepository _outboxRepository;
    private readonly BubbleDeliveryRepository _bubbleDeliveryRepository;

    public LegacyOutboxDeliverySynchronizer(
        LegacyOutboxRepository outboxRepository,
        BubbleDeliveryRepository bubbleDeliveryRepository
    )
    {
        _outboxRepository = outboxRepository;
        _bubbleDeliveryRepository = bubbleDeliveryRepository;
    }

    public void Sync()
    {
        (List<int>? ids, List<DeliveryInBubble>? deliveriesFromOutbox) =
            _outboxRepository.Get<DeliveryInBubble>();

        if (ids.Count == 0)
        {
            return;
        }

        _bubbleDeliveryRepository.Save(deliveriesFromOutbox);

        _outboxRepository.Remove(ids);
    }
}
