using Ardalis.GuardClauses;
using PackageDeliveryNew.Common;

namespace PackageDeliveryNew.Deliveries;

public class Delivery : Entity<int>
{
    private const double PricePerMilePerPound = 0.04;
    private const double NonConditionalCharge = 20;

    public Address Destination { get; }
    public decimal? CostEstimate { get; private set; }
    public IReadOnlyList<ProductLine> ProductLines => _productLines.ToList();
    private readonly List<ProductLine> _productLines;
    private readonly List<ProductLine> _removedProductLines = [];
    public bool IsSyncNeeded { get; set; }

    public Delivery(
        int id,
        Address destination,
        decimal? costEstimate,
        IEnumerable<ProductLine> productLines
    )
        : base(id)
    {
        CostEstimate = costEstimate == null ? null : Guard.Against.Negative(costEstimate.Value);
        _productLines = Guard.Against.Null(productLines.ToList());
        Destination = Guard.Against.Null(destination);
    }

    private Delivery()
        : base(0) { }

    public void RecalculateEstimatedCost(double distanceInMiles)
    {
        Guard.Against.NegativeOrZero(distanceInMiles);
        Guard.Against.Zero(_productLines.Count);

        double totalWeightInPounds = _productLines.Sum(x => x.Product.WeightInPounds * x.Amount);
        double estimate =
            totalWeightInPounds * distanceInMiles * PricePerMilePerPound + NonConditionalCharge;

        CostEstimate = decimal.Round((decimal)estimate, 2);
    }

    public void DeleteLine(ProductLine productLine)
    {
        _productLines.Remove(productLine);
        _removedProductLines.Add(productLine);
    }

    public IEnumerable<ProductLine> PopRemovedProductLines()
    {
        List<ProductLine> copy = _removedProductLines.ToList();

        _removedProductLines.Clear();

        return copy;
    }

    public void DeleteAllLines()
    {
        _productLines.Clear();
    }

    public void AddProductLine(Product product, int amount)
    {
        _productLines.Add(new ProductLine(product, amount));
    }
}
