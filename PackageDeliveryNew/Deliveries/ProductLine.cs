using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class ProductLine : Entity<Guid>
{
    public Product Product { get; private set; }
    public int Amount { get; private set; }

    public ProductLine(Product product, int amount)
        : base(new Guid())
    {
        Product = Guard.Against.Null(product);
        Amount = Guard.Against.NegativeOrZero(amount);
    }

    private ProductLine()
        : base(new Guid()) { }
}
