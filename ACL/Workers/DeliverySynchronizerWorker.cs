using ACL.Synchronizers.Delivery;

namespace ACL.Workers;

public class DeliverySynchronizerWorker : BackgroundService
{
    private readonly BubbleDeliverySynchronizer _bubbleDeliverySynchronizer;
    private readonly LegacyDeliverySynchronizer _legacyDeliverySynchronizer;

    public DeliverySynchronizerWorker(
        BubbleDeliverySynchronizer bubbleDeliverySynchronizer,
        LegacyDeliverySynchronizer legacyDeliverySynchronizer
    )
    {
        _bubbleDeliverySynchronizer = bubbleDeliverySynchronizer;
        _legacyDeliverySynchronizer = legacyDeliverySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Delivery worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            _bubbleDeliverySynchronizer.Sync();
            _legacyDeliverySynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
