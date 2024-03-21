using ACL.Synchronizers.Product.FromLegacyToBubble;

namespace ACL.Synchronizers.Product;

public class ProductSynchronizer
{
    private readonly FromLegacyToBubbleProductSynchronizer _fromLegacyToBubbleSynchronizer;

    public ProductSynchronizer(FromLegacyToBubbleProductSynchronizer fromLegacyToBubbleSynchronizer)
    {
        _fromLegacyToBubbleSynchronizer = fromLegacyToBubbleSynchronizer;
    }

    public void Sync()
    {
        _fromLegacyToBubbleSynchronizer.Sync();
    }
}
