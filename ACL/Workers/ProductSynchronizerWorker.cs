using ACL.Synchronizers.Product;

namespace ACL.Workers;

public class ProductSynchronizerWorker : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);
    private readonly ProductSynchronizer _productSynchronizer;

    public ProductSynchronizerWorker(ProductSynchronizer productSynchronizer)
    {
        _productSynchronizer = productSynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Start syncing products");
                _productSynchronizer.Sync();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                throw;
            }
        }
    }
}