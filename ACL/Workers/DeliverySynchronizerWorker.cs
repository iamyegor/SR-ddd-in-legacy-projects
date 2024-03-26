using ACL.Synchronizers;
using ACL.Synchronizers.Delivery;

namespace ACL.Workers;

public class DeliverySynchronizerWorker : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
    private readonly DeliverySynchronizer _deliverySynchronizer;

    public DeliverySynchronizerWorker(DeliverySynchronizer deliverySynchronizer)
    {
        _deliverySynchronizer = deliverySynchronizer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Start syncing deliveries");
                // _deliverySynchronizer.Sync();

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
