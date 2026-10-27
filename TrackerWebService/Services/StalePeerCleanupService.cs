using Microsoft.Extensions.Hosting;
using TrackerWebService.Data;

namespace TrackerWebService.Services
{
    public class StalePeerCleanupService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Remove the peers that heartbeats are stale
                DBManager.RemoveStalePeers();

                // 10 secs delay before the next clenaup is done
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
           
        } }
    }


}
