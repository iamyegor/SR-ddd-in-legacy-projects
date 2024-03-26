using System.Transactions;
using ACL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace ACL.Synchronizers.Product.FromLegacyToBubble;

public class FromLegacyToBubbleProductSynchronizer
{
    private const double PoundsInKilogram = 2.20462;
    private readonly string _legacyConnectionString;
    private readonly string _bubbleConnectionString;

    public FromLegacyToBubbleProductSynchronizer(ConnectionStrings connectionStrings)
    {
        _legacyConnectionString = connectionStrings.Legacy;
        _bubbleConnectionString = connectionStrings.Bubble;
    }

    public void Sync()
    {
        if (!IsSyncNeeded())
        {
            return;
        }

        List<ProductInLegacy>? productsFromLegacy = GetUpdatedProductsFromLegacy();
        if (productsFromLegacy == null)
        {
            return;
        }

        List<ProductInBubble> mappedProductsToSave = MapLegacyProducts(productsFromLegacy);

        SaveProductsInBubble(mappedProductsToSave);
    }

    private bool IsSyncNeeded()
    {
        using (var connection = new SqlConnection(_legacyConnectionString))
        {
            string query =
                "SELECT IsSyncRequired FROM [dbo].[Synchronization] WHERE Name = 'Product'";
            return connection.Query<bool>(query).Single();
        }
    }

    private List<ProductInLegacy>? GetUpdatedProductsFromLegacy()
    {
        using (var connection = new SqlConnection(_legacyConnectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    List<ProductInLegacy> updatedProductsFromLegacy = GetUpdatedProductsFromLegacy(
                        connection,
                        transaction
                    );

                    transaction.Commit();

                    return updatedProductsFromLegacy;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }

    private List<ProductInLegacy> GetUpdatedProductsFromLegacy(
        SqlConnection connection,
        SqlTransaction transaction
    )
    {
        string syncVersionQuery = "SELECT RowVersion FROM Synchronization WHERE Name = 'Product'";
        byte[] syncVersion = connection.QuerySingle<byte[]>(
            syncVersionQuery,
            transaction: transaction
        );

        string tempProductsTableQuery =
            @"
            SELECT *
            INTO #products_to_sync
            FROM [dbo].[PRD_TBL] with (UPDLOCK)
            WHERE IsSyncNeeded = 1";
        connection.Execute(tempProductsTableQuery, transaction: transaction);

        List<ProductInLegacy> productsInLegacy = connection
            .Query<ProductInLegacy>("SELECT * FROM #products_to_sync", transaction: transaction)
            .ToList();

        string setIsSyncNeededToFalseInProduct =
            @"
            UPDATE [dbo].[PRD_TBL] 
            SET IsSyncNeeded = 0 
            WHERE NMB_CM IN (SELECT NMB_CM FROM #products_to_sync)";
        connection.Execute(setIsSyncNeededToFalseInProduct, transaction: transaction);

        string setIsSyncRequiredToFalse =
            @"
            UPDATE Synchronization
            SET IsSyncRequired = 0
            WHERE Name='Product' AND RowVersion = @syncVersion";
        connection.Execute(setIsSyncRequiredToFalse, new { syncVersion }, transaction: transaction);

        return productsInLegacy;
    }

    private List<ProductInBubble> MapLegacyProducts(List<ProductInLegacy> productsFromLegacy)
    {
        return productsFromLegacy.Select(MapLegacyProduct).ToList();
    }

    private ProductInBubble MapLegacyProduct(ProductInLegacy productInLegacy)
    {
        if (productInLegacy.WT == null && productInLegacy.WT_KG == null)
        {
            throw new Exception($"Invalid weight in product {productInLegacy.NMB_CM}");
        }

        double weightInPounds =
            productInLegacy.WT ?? productInLegacy.WT_KG!.Value * PoundsInKilogram;

        return new ProductInBubble
        {
            ProductId = productInLegacy.NMB_CM,
            Name = (productInLegacy.NM_CLM ?? "").Trim(),
            WeightInPounds = weightInPounds
        };
    }

    private void SaveProductsInBubble(List<ProductInBubble> productsToSave)
    {
        string query =
            @"
            WITH updated AS (
                UPDATE products
                SET weight_in_pounds = @WeightInPounds, 
                    name = @Name
                WHERE id = @ProductId
                RETURNING *
            )
            INSERT INTO products (id, weight_in_pounds, name)
            SELECT @ProductId, @WeightInPounds, @Name
            WHERE NOT EXISTS (SELECT 1 FROM updated);";

        using (var connection = new NpgsqlConnection(_bubbleConnectionString))
        {
            connection.Execute(query, productsToSave);
        }
    }
}
