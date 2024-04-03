namespace PackageDeliveryNew.Infrastructure;

public class Synchronization
{
    public string Name { get; set; }
    public bool IsSyncRequired { get; set; }
    public int RowVersion { get; set; }
}