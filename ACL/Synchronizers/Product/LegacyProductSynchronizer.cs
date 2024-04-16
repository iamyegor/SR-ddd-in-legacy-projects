using System.Data.SqlClient;
using ACL.ConnectionStrings;
using ACL.Synchronizers.CommonRepositories;
using ACL.Synchronizers.CommonRepositories.Synchronization;
using ACL.Synchronizers.Product.Models;
using ACL.Synchronizers.Product.Repositories;
using Dapper;
using Mapster;

namespace ACL.Synchronizers.Product;

public class LegacyProductSynchronizer
{
    private readonly LegacyOutboxRepository _outboxRepository;
    private readonly LegacyProductRepository _productRepository;
    private readonly LegacySynchronizationRepository _syncRepository;

    public LegacyProductSynchronizer(
        LegacyOutboxRepository outboxRepository,
        LegacyProductRepository productRepository,
        LegacySynchronizationRepository syncRepository)
    {
        _outboxRepository = outboxRepository;
        _productRepository = productRepository;
        _syncRepository = syncRepository;
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
        string query = "select is_sync_required from sync where name = 'Product'";
        
        using var connection = new SqlConnection(LegacyConnectionString.Value);
        return connection.QuerySingle<bool>(query);
    }

    private List<ProductInLegacy> GetUpdatedProductsFromLegacy(SqlTransaction transaction)
    {
        byte[] syncRowVersion = _syncRepository.GetRowVersionFor("Product", transaction);
        
        List<ProductInLegacy> productsInLegacy = _productRepository.GetUpdatedAndResetFlags(
            transaction
        );

        _syncRepository.SetSyncFlagsFalseFor("Product", syncRowVersion, transaction);

        return productsInLegacy;
    }
}
