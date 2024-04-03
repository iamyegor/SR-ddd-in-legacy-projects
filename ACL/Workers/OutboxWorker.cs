using ACL.Synchronizers.Delivery.OutboxSynchronizers;
using ACL.Synchronizers.Product;

namespace ACL.Workers;

public class OutboxWorker : BackgroundService
{
    private readonly BubbleOutboxDeliverySynchronizer _bubbleOutboxDeliverySynchronizer;
    private readonly LegacyOutboxDeliverySynchronizer _legacyOutboxDeliverySynchronizer;
    private readonly LegacyOutboxProductSynchronizer _legacyOutboxProductSynchronizer;

    public OutboxWorker(
        BubbleOutboxDeliverySynchronizer bubbleOutboxDeliverySynchronizer,
        LegacyOutboxDeliverySynchronizer legacyOutboxDeliverySynchronizer,
        LegacyOutboxProductSynchronizer legacyOutboxProductSynchronizer
    )
    {
        _bubbleOutboxDeliverySynchronizer = bubbleOutboxDeliverySynchronizer;
        _legacyOutboxDeliverySynchronizer = legacyOutboxDeliverySynchronizer;
        _legacyOutboxProductSynchronizer = legacyOutboxProductSynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Outbox worker started!");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _bubbleOutboxDeliverySynchronizer.Sync();
            _legacyOutboxDeliverySynchronizer.Sync();
            _legacyOutboxProductSynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
