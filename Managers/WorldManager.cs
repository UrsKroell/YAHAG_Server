using Microsoft.Extensions.Logging;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.Managers
{
    public class WorldManager
    {
        private readonly ILogger<WorldManager> logger;
        private readonly IVmManager vmManager;

        public WorldManager(ILogger<WorldManager> logger, IVmManager vmManager)
        {
            this.logger = logger;
            this.vmManager = vmManager;
        }
    }
}