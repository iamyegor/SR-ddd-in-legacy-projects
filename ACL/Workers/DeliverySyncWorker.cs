using ACL.Synchronizers.BubbleDeliveries;
using ACL.Synchronizers.LegacyDeliveries;
using Serilog;

namespace ACL.Workers;

public class DeliverySyncWorker : BackgroundService
{
    private readonly BubbleDeliverySynchronizer _bubbleDeliverySynchronizer;
    private readonly LegacyDeliverySynchronizer _legacyDeliverySynchronizer;

    public DeliverySyncWorker(
        BubbleDeliverySynchronizer bubbleDeliverySynchronizer,
        LegacyDeliverySynchronizer legacyDeliverySynchronizer
    )
    {
        _bubbleDeliverySynchronizer = bubbleDeliverySynchronizer;
        _legacyDeliverySynchronizer = legacyDeliverySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Delivery worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _legacyDeliverySynchronizer.Sync();
                _bubbleDeliverySynchronizer.Sync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeliverySyncWorker caught the exception");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
