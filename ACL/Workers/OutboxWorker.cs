using ACL.Synchronizers.BubbleDeliveries;
using ACL.Synchronizers.LegacyDeliveries;
using ACL.Synchronizers.Product;
using Serilog;

namespace ACL.Workers;

public class OutboxWorker : BackgroundService
{
    private readonly LegacyOutboxProductSynchronizer _legacyOutboxProductSynchronizer;
    private readonly LegacyOutboxDeliverySynchronizer _legacyOutboxDeliverySynchronizer;
    private readonly BubbleOutboxDeliverySynchronizer _bubbleOutboxDeliverySynchronizer;

    public OutboxWorker(
        LegacyOutboxProductSynchronizer legacyOutboxProductSynchronizer,
        BubbleOutboxDeliverySynchronizer bubbleOutboxDeliverySynchronizer,
        LegacyOutboxDeliverySynchronizer legacyOutboxDeliverySynchronizer
    )
    {
        _legacyOutboxProductSynchronizer = legacyOutboxProductSynchronizer;
        _bubbleOutboxDeliverySynchronizer = bubbleOutboxDeliverySynchronizer;
        _legacyOutboxDeliverySynchronizer = legacyOutboxDeliverySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Outbox worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _legacyOutboxProductSynchronizer.Sync();
                _legacyOutboxDeliverySynchronizer.Sync();
                _bubbleOutboxDeliverySynchronizer.Sync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "OutboxWorker caught the exception");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
