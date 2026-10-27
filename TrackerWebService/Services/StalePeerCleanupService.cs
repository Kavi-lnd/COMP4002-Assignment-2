using Microsoft.Extensions.Hosting;
using TrackerWebService.Data;

namespace TrackerWebService.Services
{
    public class StalePeerCleanupService : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DBManager.RemoveStalePeers();

                await Task.Delay(
                    TimeSpan.FromSeconds(10),
                    stoppingToken);
            }
        }
    }
}
