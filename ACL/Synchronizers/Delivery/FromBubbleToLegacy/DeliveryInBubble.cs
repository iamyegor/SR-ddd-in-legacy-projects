namespace ACL.Synchronizers.Delivery.FromBubbleToLegacy;

public class DeliveryInBubble
{
    public int Id { get; set; }
    public decimal? CostEstimate { get; set; }
    public List<ProductLineInBubble> ProductLines { get; set; } = null!;
}
