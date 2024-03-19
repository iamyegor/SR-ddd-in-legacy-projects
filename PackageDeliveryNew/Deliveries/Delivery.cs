using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class Delivery : Entity
{
    private const double PricePerMilePerPound = 0.04;
    private const double NonConditionalCharge = 20;
    public Address Address { get; }

    public Delivery(int id, Address address)
        : base(id)
    {
        Address = Guard.Against.Null(address);
    }

    public decimal GetEstimate(double distanceInMiles, List<ProductLine> productLines)
    {
        Guard.Against.NegativeOrZero(distanceInMiles);
        Guard.Against.OutOfRange(productLines.Count, nameof(productLines), 0, 4);

        double totalWeightInPounds = productLines.Sum(x => x.Product.WeightInPounds * x.Amount);
        double estimate =
            totalWeightInPounds * distanceInMiles * PricePerMilePerPound + NonConditionalCharge;

        return decimal.Round((decimal)estimate, 2);
    }
}
