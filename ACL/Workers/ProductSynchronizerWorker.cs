using ACL.Synchronizers.Product;

namespace ACL.Workers;

public class ProductSynchronizerWorker : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);
    private readonly FromLegacyToBubbleProductSynchronizer _fromLegacyToBubbleSynchronizer;

    public ProductSynchronizerWorker(
        FromLegacyToBubbleProductSynchronizer fromLegacyToBubbleSynchronizer
    )
    {
        _fromLegacyToBubbleSynchronizer = fromLegacyToBubbleSynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Start syncing products");
                
                _fromLegacyToBubbleSynchronizer.Sync();
                
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
