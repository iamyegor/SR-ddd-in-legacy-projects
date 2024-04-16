using ACL.Synchronizers.CommonRepositories.Outbox;
using ACL.Synchronizers.Product.Models;
using ACL.Synchronizers.Product.Repositories;

namespace ACL.Synchronizers.Product;

public class LegacyOutboxProductSynchronizer
{
    private readonly BubbleProductRepository _bubbleProductRepository;
    private readonly LegacyOutboxRepository _outboxRepository;

    public LegacyOutboxProductSynchronizer(
        BubbleProductRepository bubbleProductRepository,
        LegacyOutboxRepository outboxRepository
    )
    {
        _bubbleProductRepository = bubbleProductRepository;
        _outboxRepository = outboxRepository;
    }

    public void Sync()
    {
        (List<int> ids, List<ProductInBubble> productsFromOutbox) =
            _outboxRepository.Get<ProductInBubble>();

        if (ids.Count == 0)
        {
            return;
        }

        _bubbleProductRepository.Save(productsFromOutbox);

        _outboxRepository.Remove(ids);
    }
}
