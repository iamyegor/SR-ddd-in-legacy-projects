using ACL.Synchronizers.Product;
using Serilog;

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
        Log.Information("Product worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _legacyProductSynchronizer.Sync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ProductSyncWorker caught the exception");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
