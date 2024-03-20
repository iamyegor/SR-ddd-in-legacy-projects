using ACL.Utils;

namespace ACL.Synchronizers;

public class ProductSynchronizer
{
    private readonly string _legacyConnectionString;
    private readonly string _bubbleConnectionString;

    public ProductSynchronizer(ConnectionStrings connectionStrings)
    {
        _legacyConnectionString = connectionStrings.Legacy;
        _bubbleConnectionString = connectionStrings.Bubble;
    }
    
    public void Sync()
    {
        Console.WriteLine("Syncing products");
    }
}
