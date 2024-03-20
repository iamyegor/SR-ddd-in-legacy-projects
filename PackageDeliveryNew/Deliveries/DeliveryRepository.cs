using Dapper;
using Microsoft.Data.SqlClient;
using PackageDeliveryNew.Utils;

namespace PackageDeliveryNew.Deliveries;

public class DeliveryRepository
{
    public Delivery? GetById(int id)
    {
        (DeliveryData? deliveryData, List<ProductLineData> productLinesData) = GetRawDeliveryById(
            id
        );

        return deliveryData == null
            ? null
            : MapRawDeliveryToDelivery(deliveryData, productLinesData);
    }

    private Delivery MapRawDeliveryToDelivery(
        DeliveryData deliveryData,
        List<ProductLineData> productLinesData
    )
    {
        Address destination = new Address(
            deliveryData.DestinationStreet,
            deliveryData.DestinationCity,
            deliveryData.DestinationState,
            deliveryData.DestinationZipCode
        );

        IEnumerable<ProductLine> productLines = productLinesData.Select(line => new ProductLine(
            new Product(line.ProductId, line.ProductWeightInPounds, line.ProductName),
            line.Amount
        ));

        return new Delivery(
            deliveryData.DeliveryId,
            destination,
            deliveryData.CostEstimate,
            productLines
        );
    }

    private (DeliveryData?, List<ProductLineData>) GetRawDeliveryById(int id)
    {
        string query =
            @"
            SELECT *
            FROM [dbo].[Delivery] d
            WHERE d.DeliveryID = @deliveryId

            SELECT pline.*, prod.WeightInPounds ProductWeightInPounds, prod.Name ProductName
            FROM [dbo].[ProductLine] pline
            INNER JOIN [dbo].[Product] prod on pline.ProductID = prod.ProductID
            WHERE pline.DeliveryID = @deliveryId";

        using (var connection = new SqlConnection(ConnectionString.Value))
        {
            SqlMapper.GridReader reader = connection.QueryMultiple(query, new { deliveryId = id });
            DeliveryData? deliveryData = reader.Read<DeliveryData>().SingleOrDefault();
            List<ProductLineData> productLinesData = reader.Read<ProductLineData>().ToList();

            return (deliveryData, productLinesData);
        }
    }

    public void Save(Delivery delivery)
    {
        string query1 =
            @"
            UPDATE [dbo].[Delivery]
            SET CostEstimate = @CostEstimate
            WHERE DeliveryID = @Id
            
            DELETE FROM [dbo].[ProductLine]
            WHERE DeliveryId = @Id";

        string query2 =
            @"
            INSERT INTO [dbo].[ProductLine] (ProductID, Amount, DeliveryID)
            VALUES (@ProductId, @Amount, @DeliveryId)";

        using (var connection = new SqlConnection(ConnectionString.Value))
        {
            connection.Execute(query1, new { delivery.Id, delivery.CostEstimate });

            var productLinesToInsert = delivery.ProductLines.Select(pl => new
            {
                ProductId = pl.Product.Id,
                pl.Amount,
                DeliveryId = delivery.Id
            });

            connection.Execute(query2, productLinesToInsert);
        }
    }
}

public class DeliveryData
{
    public int DeliveryId { get; set; }
    public decimal? CostEstimate { get; set; }
    public string DestinationStreet { get; set; } = null!;
    public string DestinationCity { get; set; } = null!;
    public string DestinationState { get; set; } = null!;
    public string DestinationZipCode { get; set; } = null!;
}

public class ProductLineData
{
    public int ProductId { get; set; }
    public int Amount { get; set; }
    public double ProductWeightInPounds { get; set; }
    public string ProductName { get; set; }
}
