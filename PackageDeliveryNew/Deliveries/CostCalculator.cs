using FluentResults;

namespace PackageDeliveryNew.Deliveries;

public class CostCalculator
{
    private readonly ProductRepository _productRepository = new ProductRepository();
    private readonly DeliveryRepository _deliveryRepository = new DeliveryRepository();
    private readonly AddressResolver _addressResolver = new AddressResolver();

    public Result<decimal> Calculate(
        int deliveryId,
        int? productId1,
        int amount1,
        int? productId2,
        int amount2,
        int? productId3,
        int amount3,
        int? productId4,
        int amount4
    )
    {
        // if (productId1 == null && productId2 == null && productId3 == null & productId4 == null)
        // {
        //     return Result.Fail("At least one product has to be provided");
        // }
        //
        // Delivery? delivery = _deliveryRepository.GetById(deliveryId);
        // if (delivery == null)
        // {
        //     throw new Exception($"Delivery with id {deliveryId} wasn't found.");
        // }
        //
        // double? distance = _addressResolver.GetDistanceTo(delivery.Destination);
        // if (distance == null)
        // {
        //     return Result.Fail("Address is invalid");
        // }
        //
        // var productIdAndAmountTuples = new List<(int? productId, int amount)>
        // {
        //     (productId1, amount1),
        //     (productId2, amount2),
        //     (productId3, amount3),
        //     (productId4, amount4)
        // }.ToList();
        //
        // List<ProductLine> productLines = [];
        // foreach (var productIdAndAmount in productIdAndAmountTuples)
        // {
        //     if (productIdAndAmount.productId == null)
        //     {
        //         continue;
        //     }
        //
        //     Product? product = _productRepository.GetById(productIdAndAmount.productId.Value);
        //     if (product == null)
        //     {
        //         throw new Exception($"There is no product with id {productIdAndAmount.productId}");
        //     }
        //
        //     productLines.Add(new ProductLine(product, productIdAndAmount.amount));
        // }

        // return delivery.GetEstimate(distance.Value, productLines);
        return 0;
    }
}
