using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shared.OperatingSystem;
using YAHGA_Server.Database.Models;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.Vm
{
    public class Vm
    {
        private readonly ILogger<Vm> logger;
        private readonly IFilesystemManager filesystemManager;
        private readonly IDatabaseManager databaseManager;
        
        private readonly Guid hostGuid;

        private Os operatingSystem;
        
        public Vm(ILogger<Vm> logger, IFilesystemManager filesystemManager, IDatabaseManager databaseManager, HostModel hostModel)
        {
            this.logger = logger;
            this.filesystemManager = filesystemManager;
            this.databaseManager = databaseManager;
            
            hostGuid = hostModel.Guid;
        }

        public async Task InitializeAsync()
        {
            var fs = await filesystemManager.GetFilesystem(hostGuid);
            operatingSystem = new Os(fs);
        }

        public Os GetOs()
        {
            return operatingSystem;
        }
    }
}