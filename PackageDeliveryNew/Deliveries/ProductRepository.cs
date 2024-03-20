using Dapper;
using Microsoft.Data.SqlClient;
using PackageDeliveryNew.Utils;

namespace PackageDeliveryNew.Deliveries;

public class ProductRepository
{
    public Product? GetById(int id)
    {
        ProductData? productData = GetRawProductById(id);
        return productData == null ? null : MapRawProductToProduct(productData);
    }

    public IEnumerable<Product> GetAll()
    {
        IEnumerable<ProductData> allProducts = GetAllProductsRaw();

        return allProducts.Select(MapRawProductToProduct);
    }

    private IEnumerable<ProductData> GetAllProductsRaw()
    {
        string query =
            @"SELECT *
            FROM [dbo].[Product]";

        using (var connection = new SqlConnection(ConnectionString.Value))
        {
            return connection.Query<ProductData>(query);
        }
    }

    private Product MapRawProductToProduct(ProductData productData)
    {
        return new Product(productData.ProductId, productData.WeightInPounds, productData.Name);
    }

    private ProductData? GetRawProductById(int id)
    {
        string query =
            @"
            SELECT *
            FROM [dbo].[Product]
            WHERE ProductId = @productId";

        using (var connection = new SqlConnection(ConnectionString.Value))
        {
            return connection.Query<ProductData>(query, new { productId = id }).SingleOrDefault();
        }
    }
}

public class ProductData
{
    public int ProductId { get; set; }
    public double WeightInPounds { get; set; }
    public string Name { get; set; } = null!;
}
