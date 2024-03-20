namespace ACL.Synchronizers.Delivery.FromBubbleToLegacy;

public class DeliveryInBubble
{
    public int DeliveryId { get; set; }
    public decimal? CostEstimate { get; set; }
    public List<ProductLineInBubble> ProductLines { get; set; }
}