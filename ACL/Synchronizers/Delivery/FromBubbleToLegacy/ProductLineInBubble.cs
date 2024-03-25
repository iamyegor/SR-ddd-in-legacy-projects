namespace ACL.Synchronizers.Delivery.FromBubbleToLegacy;

public class ProductLineInBubble
{
    public int ProductId { get; set; }
    public int Amount { get; set; }
    public int DeliveryId { get; set; }
    public bool IsDeleted { get; set; }
}
