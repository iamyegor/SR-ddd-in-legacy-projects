namespace ACL.Synchronizers.Delivery.FromLegacyToBubble;

internal class DeliveryInBubble
{
    // ReSharper disable once InconsistentNaming
    public int Id { get; set; }
    public string DestinationStreet { get; set; } = null!;
    public string DestinationCity { get; set; } = null!;
    public string DestinationState { get; set; } = null!;
    public string DestinationZipCode { get; set; } = null!;
}