using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server
{
    public class Service : BackgroundService
    {
        private readonly ILogger<Service> logger;
        private readonly INetworkManager networkManager;

        public Service(ILogger<Service> logger, INetworkManager networkManager)
        {
            this.logger = logger;
            this.networkManager = networkManager;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting Service");
            logger.LogInformation("Starting NetworkManager");
            networkManager.Start();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping NetworkManager");
            networkManager.Stop();
            
            logger.LogInformation("Stopping Service");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}