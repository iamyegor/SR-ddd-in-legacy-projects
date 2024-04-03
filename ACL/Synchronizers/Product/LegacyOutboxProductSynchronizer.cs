using System.Data.SqlClient;
using ACL.Synchronizers.CommonRepositories;
using ACL.Synchronizers.Product.Models;
using ACL.Synchronizers.Product.Repositories;
using ACL.Utils;

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
        using var connection = new SqlConnection(LegacyConnectionString.Value);

        (List<int> ids, List<ProductInBubble> products) = _outboxRepository.Get<ProductInBubble>(
            "Product"
        );
        if (ids.Count == 0)
        {
            return;
        }

        _bubbleProductRepository.Save(products);
        _outboxRepository.Remove(ids);
    }
}
