using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class ProductLine : ValueObject
{
    public int Id { get; private set; }
    public Product Product { get; private set; }
    public int Amount { get; private set; }

    public ProductLine(Product product, int amount)
    {
        Product = Guard.Against.Null(product);
        Amount = Guard.Against.NegativeOrZero(amount);
    }

    private ProductLine() { }

    protected override IEnumerable<object?> GetPropertiesForComparison()
    {
        yield return Product;
        yield return Amount;
    }
}
