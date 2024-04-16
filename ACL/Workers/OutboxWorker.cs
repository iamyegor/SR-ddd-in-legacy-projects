using ACL.Synchronizers.Product;

namespace ACL.Workers;

public class OutboxWorker : BackgroundService
{
    private readonly LegacyOutboxProductSynchronizer _legacyOutboxProductSynchronizer;

    public OutboxWorker(LegacyOutboxProductSynchronizer legacyOutboxProductSynchronizer)
    {
        _legacyOutboxProductSynchronizer = legacyOutboxProductSynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Outbox worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            _legacyOutboxProductSynchronizer.Sync();

            await Task.Delay(1000, stoppingToken);
        }
    }
}
