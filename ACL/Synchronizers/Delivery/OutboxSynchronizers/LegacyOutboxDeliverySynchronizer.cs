using ACL.Synchronizers.Delivery.Models;
using ACL.Synchronizers.Delivery.Repositories;
using ACL.Synchronizers.Product.Repositories;

namespace ACL.Synchronizers.Delivery.OutboxSynchronizers;

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
            _outboxRepository.Get<DeliveryInBubble>("Delivery");

        if (ids.Count == 0)
        {
            return;
        }

        _bubbleDeliveryRepository.Save(deliveriesFromOutbox);

        _outboxRepository.Remove(ids);
    }
}
