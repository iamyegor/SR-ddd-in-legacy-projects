using ACL.Synchronizers.Delivery;

namespace ACL.Workers;

public class DeliverySyncWorker : BackgroundService
{
    private readonly BubbleDeliverySynchronizer _bubbleDeliverySynchronizer;

    public DeliverySyncWorker(BubbleDeliverySynchronizer bubbleDeliverySynchronizer)
    {
        _bubbleDeliverySynchronizer = bubbleDeliverySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _bubbleDeliverySynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
