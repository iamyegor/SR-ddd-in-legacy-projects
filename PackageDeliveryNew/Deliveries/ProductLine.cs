using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class ProductLine : ValueObject
{
    public Product Product { get; }
    public int Amount { get; }

    public ProductLine(Product product, int amount)
    {
        Product = Guard.Against.Null(product);
        Amount = Guard.Against.NegativeOrZero(amount);
    }

    protected override IEnumerable<object?> GetPropertiesForComparison()
    {
        yield return Product;
        yield return Amount;
    }
}
