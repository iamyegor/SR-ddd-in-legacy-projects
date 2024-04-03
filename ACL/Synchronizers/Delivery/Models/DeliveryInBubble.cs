namespace ACL.Synchronizers.Delivery.Models;

public class DeliveryInBubble
{
    public int Id { get; set; }
    public string DestinationCity { get; set; } = null!;
    public string DestinationState { get; set; } = null!;
    public string DestinationStreet { get; set; } = null!;
    public string DestinationZipCode { get; set; } = null!;
    public decimal CostEstimate { get; set; }
    public List<ProductLineInBubble> ProductLines { get; set; } = null!;
}
