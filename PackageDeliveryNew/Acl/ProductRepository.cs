using Dapper;
using Microsoft.Data.SqlClient;
using PackageDeliveryNew.Deliveries;
using PackageDeliveryNew.Utils;

namespace PackageDeliveryNew.Acl;

public class ProductRepository
{
    private const double PoundsInKilogram = 2.20462;

    public Product? GetById(int id)
    {
        ProductFromLegacy? productFromLegacy = GetProductFromLegacyById(id);
        if (productFromLegacy != null)
        {
            return MapProductFromLegacyToProduct(productFromLegacy);
        }

        return null;
    }

    private Product? MapProductFromLegacyToProduct(ProductFromLegacy productFromLegacy)
    {
        if (productFromLegacy.WT == null && productFromLegacy.WT_KG == null)
        {
            throw new Exception($"Invalid weight in product {productFromLegacy.NMB_CM}");
        }

        double weightInPounds =
            productFromLegacy.WT ?? productFromLegacy.WT_KG!.Value * PoundsInKilogram;

        return new Product(productFromLegacy.NMB_CM, weightInPounds);
    }

    private ProductFromLegacy? GetProductFromLegacyById(int id)
    {
        using (var connection = new SqlConnection(ConnectionString.Value))
        {
            string query =
                @"
                SELECT NMB_CM, WT, WT_KG
                FROM [dbo].[PRD_TBL]
                WHERE NMB_CM = @Id";

            return connection.Query<ProductFromLegacy>(query, new { Id = id }).SingleOrDefault();
        }
    }
}

public class ProductFromLegacy
{
    public int NMB_CM { get; set; }
    public double? WT { get; set; }
    public double? WT_KG { get; set; }
}
