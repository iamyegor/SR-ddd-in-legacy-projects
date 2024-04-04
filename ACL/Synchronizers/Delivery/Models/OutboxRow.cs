namespace ACL.Synchronizers.Delivery.Models;

public class OutboxRow
{
    public int Id { get; set; }
    public string Content {get; set; }
}