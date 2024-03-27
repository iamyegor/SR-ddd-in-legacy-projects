using ACL.Synchronizers.Outbox;

namespace ACL.Workers;

public class OutboxProcessingWorker : BackgroundService
{
    private readonly BubbleOutboxDeliverySynchronizer _bubbleOutboxDeliverySynchronizer;
    private readonly LegacyOutboxDeliverySynchronizer _legacyOutboxDeliverySynchronizer;
    private readonly LegacyOutboxProductSynchronizer _legacyOutboxProductSynchronizer;

    public OutboxProcessingWorker(
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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _bubbleOutboxDeliverySynchronizer.Sync();
                _legacyOutboxDeliverySynchronizer.Sync();
                _legacyOutboxProductSynchronizer.Sync();

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                throw;
            }
        }
    }
}
