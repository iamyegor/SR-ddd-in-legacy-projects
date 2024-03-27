using ACL.Synchronizers;
using ACL.Synchronizers.Delivery;

namespace ACL.Workers;

public class DeliverySynchronizerWorker : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
    private readonly FromLegacyToBubbleDeliverySynchronizer _fromLegacyToBubbleSynchronizer;
    private readonly FromBubbleToLegacyDeliverySynchronizer _fromBubbleToLegacySynchronizer;

    public DeliverySynchronizerWorker(
        FromLegacyToBubbleDeliverySynchronizer fromLegacyToBubbleSynchronizer,
        FromBubbleToLegacyDeliverySynchronizer fromBubbleToLegacySynchronizer
    )
    {
        _fromLegacyToBubbleSynchronizer = fromLegacyToBubbleSynchronizer;
        _fromBubbleToLegacySynchronizer = fromBubbleToLegacySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Start syncing deliveries");

                _fromLegacyToBubbleSynchronizer.Sync();
                _fromBubbleToLegacySynchronizer.Sync();

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
