using ACL.Synchronizers.Delivery.FromBubbleToLegacy;
using ACL.Synchronizers.Delivery.FromLegacyToBubble;

namespace ACL.Synchronizers.Delivery;

public class DeliverySynchronizer
{
    private readonly FromLegacyToBubbleDeliverySynchronizer _fromLegacyToBubbleSynchronizer;
    private readonly FromBubbleToLegacyDeliverySynchronizer _fromBubbleToLegacySynchronizer;

    public DeliverySynchronizer(
        FromLegacyToBubbleDeliverySynchronizer fromLegacyToBubbleSynchronizer,
        FromBubbleToLegacyDeliverySynchronizer fromBubbleToLegacySynchronizer)
    {
        _fromLegacyToBubbleSynchronizer = fromLegacyToBubbleSynchronizer;
        _fromBubbleToLegacySynchronizer = fromBubbleToLegacySynchronizer;
    }

    public void Sync()
    {
        _fromLegacyToBubbleSynchronizer.Sync();
        _fromBubbleToLegacySynchronizer.Sync();
    }
}
