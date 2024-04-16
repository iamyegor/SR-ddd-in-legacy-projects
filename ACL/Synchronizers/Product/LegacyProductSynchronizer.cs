using System.Data.SqlClient;
using ACL.ConnectionStrings;
using ACL.Synchronizers.CommonRepositories;
using ACL.Synchronizers.Product.Models;
using Mapster;

namespace ACL.Synchronizers.Product;

public class LegacyProductSynchronizer
{
    private readonly LegacyOutboxRepository _outboxRepository;

    public LegacyProductSynchronizer(LegacyOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    public void Sync()
    {
        if (!IsSyncNeeded())
        {
            return;
        }

        using var connection = new SqlConnection(LegacyConnectionString.Value);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            List<ProductInLegacy> productsFromLegacy = GetUpdatedProductsFromLegacy(transaction);

            List<ProductInBubble> mappedProducts = productsFromLegacy.Adapt<
                List<ProductInBubble>
            >();

            _outboxRepository.Save(mappedProducts, transaction);

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

    private List<ProductInLegacy> GetUpdatedProductsFromLegacy(SqlTransaction transaction)
    {
        throw new NotImplementedException();
    }
}
