using ACL.Synchronizers.Product;

namespace ACL.Workers;

public class ProductSynchronizerWorker : BackgroundService
{
    private const int HourInMilliseconds = 60 * 60 * 1000;
    private readonly LegacyProductSynchronizer _productSynchronizer;

    public ProductSynchronizerWorker(LegacyProductSynchronizer productSynchronizer)
    {
        _productSynchronizer = productSynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Product worker started!");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _productSynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
