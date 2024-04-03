using System.ComponentModel;
using ACL.Synchronizers.Delivery;

namespace ACL.Workers;

public class DeliverySynchronizerWorker : BackgroundService
{
    private readonly LegacyDeliverySynchronizer _legacyDeliverySynchronizer;
    private readonly BubbleDeliverySynchronizer _bubbleDeliverySynchronizer;

    public DeliverySynchronizerWorker(
        LegacyDeliverySynchronizer legacyDeliverySynchronizer,
        BubbleDeliverySynchronizer bubbleDeliverySynchronizer
    )
    {
        _legacyDeliverySynchronizer = legacyDeliverySynchronizer;
        _bubbleDeliverySynchronizer = bubbleDeliverySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Delivery worker started!");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _legacyDeliverySynchronizer.Sync();
            _bubbleDeliverySynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
