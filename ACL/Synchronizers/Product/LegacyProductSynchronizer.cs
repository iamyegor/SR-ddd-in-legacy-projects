using System.Data;
using System.Data.SqlClient;
using ACL.Synchronizers.CommonRepositories;
using ACL.Synchronizers.Product.Models;
using ACL.Synchronizers.Product.Repositories;
using ACL.Utils;
using AutoMapper;
using Dapper;

namespace ACL.Synchronizers.Product;

public class LegacyProductSynchronizer
{
    private readonly IMapper _mapper;

    public LegacyProductSynchronizer(IMapper mapper)
    {
        _mapper = mapper;
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
            List<ProductInLegacy> productsInLegacy = GetUpdatedProductsFromLegacy(
                connection,
                transaction
            );

            List<ProductInBubble> productsToSave = _mapper.Map<List<ProductInBubble>>(
                productsInLegacy
            );

            var outboxRepository = new LegacyOutboxRepository(connection, transaction);
            outboxRepository.Save(productsToSave, "Product");

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
        }
    }

    private bool IsSyncNeeded()
    {
        using var connection = new SqlConnection(LegacyConnectionString.Value);

        string query = "select IsSyncRequired from [dbo].[Synchronization] where Name = 'Product'";
        return connection.QuerySingle<bool>(query);
    }

    private List<ProductInLegacy> GetUpdatedProductsFromLegacy(
        SqlConnection connection,
        SqlTransaction transaction
    )
    {
        var productRepository = new LegacyProductRepository(connection, transaction);
        var synchronizationRepository = new LegacySynchronizationRepository(
            connection,
            transaction
        );

        byte[] syncRowVersion = synchronizationRepository.GetRowVersionFor("Product");

        List<ProductInLegacy> products = productRepository.GetAllThatNeedSync();

        productRepository.SetSyncFlagFalseForQueriedProducts();
        synchronizationRepository.SetSyncFlagFalse("Product", syncRowVersion);

        return products;
    }
}