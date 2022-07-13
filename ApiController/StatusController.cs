using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.ApiController
{
    [ApiController]
    [Route("/api/status")]
    public class StatusController : ControllerBase
    {
        private ILogger<StatusController> logger;
        private INetworkManager networkManager;
        private IUserManager userManager;

        public StatusController(ILogger<StatusController> logger, INetworkManager networkManager, IUserManager userManager)
        {
            this.logger = logger;
            this.networkManager = networkManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public OkObjectResult Get()
        {
            var isServerRunning = networkManager.IsRunning;
            var tcpConnections = networkManager.GetConnectionCount();
            var connectedClients = userManager.GetConnectedUserCount();

            return Ok(new ServerStatus
            {
                IsServerRunning = isServerRunning,
                ConnectedClients = connectedClients,
                TcpConnections = tcpConnections
            });
        }
    }
}