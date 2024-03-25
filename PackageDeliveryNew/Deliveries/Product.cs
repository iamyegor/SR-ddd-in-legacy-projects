using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class Product : Entity
{
    public string Name { get; private set; }
    public double WeightInPounds { get; private set; }

    public Product(int id, double weightInPounds, string name)
        : base(id)
    {
        Name = name;
        WeightInPounds = Guard.Against.Negative(weightInPounds);
    }

    private Product()
        : base(0) { }
}
