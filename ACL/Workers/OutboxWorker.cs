using ACL.Synchronizers.Delivery.OutboxSynchronizers;
using ACL.Synchronizers.Product;

namespace ACL.Workers;

public class OutboxWorker : BackgroundService
{
    private readonly BubbleOutboxDeliverySynchronizer _bubbleOutboxDeliverySynchronizer;
    private readonly LegacyOutboxProductSynchronizer _legacyOutboxProductSynchronizer;
    private readonly LegacyOutboxDeliverySynchronizer _legacyOutboxDeliverySynchronizer;

    public OutboxWorker(
        BubbleOutboxDeliverySynchronizer bubbleOutboxDeliverySynchronizer,
        LegacyOutboxProductSynchronizer legacyOutboxProductSynchronizer,
        LegacyOutboxDeliverySynchronizer legacyOutboxDeliverySynchronizer
    )
    {
        _bubbleOutboxDeliverySynchronizer = bubbleOutboxDeliverySynchronizer;
        _legacyOutboxProductSynchronizer = legacyOutboxProductSynchronizer;
        _legacyOutboxDeliverySynchronizer = legacyOutboxDeliverySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Outbox worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            _bubbleOutboxDeliverySynchronizer.Sync();
            _legacyOutboxProductSynchronizer.Sync();
            _legacyOutboxDeliverySynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
