namespace ACL.Synchronizers.Delivery.Models;

public class ProductLineInBubble
{
    public int ProductId { get; set; }
    public double Amount { get; set; }
    public int DeliveryId { get; set; }
    public bool IsDeleted { get; set; }
}
