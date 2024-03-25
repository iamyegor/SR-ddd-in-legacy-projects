using ACL.Utils;
using Dapper;
using Microsoft.Data.SqlClient;

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

        List<ProductInLegacy> productsFromLegacy = GetUpdatedProductsFromLegacy();
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

    private List<ProductInLegacy> GetUpdatedProductsFromLegacy()
    {
        string query =
            @"
                 SELECT *
                 FROM [dbo].[PRD_TBL] with (UPDLOCK)
                 WHERE IsSyncNeeded = 1
     
                 UPDATE [dbo].[PRD_TBL]
                 SET IsSyncNeeded = 0
                 WHERE IsSyncNeeded = 1
     
                 UPDATE Synchronization
                 SET IsSyncRequired = 0
                 WHERE Name = 'Product'";

        using (var connection = new SqlConnection(_legacyConnectionString))
        {
            return connection.Query<ProductInLegacy>(query).ToList();
        }
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
                UPDATE [dbo].[Products]
                SET Id = @ProductId, 
                WeightInPounds = @WeightInPounds, 
                Name = @Name
                WHERE Id = @ProductId;
    
                IF (@@ROWCOUNT = 0)
                BEGIN
                    INSERT [dbo].[Products] (Id, WeightInPounds, Name)
                    VALUES (@ProductId, @WeightInPounds, @Name)
                END;";

        using (var connection = new SqlConnection(_bubbleConnectionString))
        {
            connection.Execute(query, productsToSave);
        }
    }
}
