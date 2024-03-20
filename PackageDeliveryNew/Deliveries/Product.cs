using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class Product : Entity
{
    public string Name { get; }
    public double WeightInPounds { get; }

    public Product(int id, double weightInPounds, string name)
        : base(id)
    {
        Name = name;
        WeightInPounds = Guard.Against.Negative(weightInPounds);
    }
}
