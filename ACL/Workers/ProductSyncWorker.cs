using ACL.Synchronizers.Product;

namespace ACL.Workers;

public class ProductSyncWorker : BackgroundService
{
    private readonly LegacyProductSynchronizer _legacyProductSynchronizer;

    public ProductSyncWorker(LegacyProductSynchronizer legacyProductSynchronizer)
    {
        _legacyProductSynchronizer = legacyProductSynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Product worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            _legacyProductSynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
