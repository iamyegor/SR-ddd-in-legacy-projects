using ACL.ConnectionStrings;
using ACL.Synchronizers.Product.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace ACL.Synchronizers.Product;

public class FromLegacyToBubbleProductSynchronizer
{
    private const double PoundsInKilogram = 2.20462;

    public void Sync()
    {
        if (!IsSyncNeeded())
        {
            return;
        }

        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    List<ProductInLegacy> productsFromLegacy = GetUpdatedProductsFromLegacy(
                        connection,
                        transaction
                    );

                    List<ProductInBubble> mappedProducts = MapLegacyProducts(productsFromLegacy);

                    SaveProductsToOutbox(mappedProducts, connection, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }
    }

    private bool IsSyncNeeded()
    {
        using (var connection = new SqlConnection(LegacyConnectionString.Value))
        {
            string query =
                "SELECT IsSyncRequired FROM [dbo].[Synchronization] WHERE Name = 'Product'";
            return connection.Query<bool>(query).Single();
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
        int rowsAffected = connection.Execute(
            setIsSyncRequiredToFalse,
            new { syncVersion },
            transaction: transaction
        );

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Sync version conflict");
        }

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

    private void SaveProductsToOutbox(
        List<ProductInBubble> productsInBubble,
        SqlConnection connection,
        SqlTransaction transaction
    )
    {
        var productsToSave = productsInBubble.Select(p => new
        {
            Content = JsonConvert.SerializeObject(p)
        });

        string query =
            @"
            INSERT INTO outbox (content, type) 
            VALUES (@Content, 'Product')";

        connection.Execute(query, productsToSave, transaction: transaction);
    }
}
