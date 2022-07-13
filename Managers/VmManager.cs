using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YAHGA_Server.Database.Models;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.Managers
{
    public class VmManager : IVmManager
    {
        private readonly ILogger<VmManager> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IFilesystemManager filesystemManager;
        private readonly IDatabaseManager databaseManager;

        private readonly Dictionary<Guid, Vm.Vm> vms;

        public VmManager(ILogger<VmManager> logger, IServiceProvider serviceProvider,IFilesystemManager filesystemManager, IDatabaseManager databaseManager)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.filesystemManager = filesystemManager;
            this.databaseManager = databaseManager;
            
            vms = new Dictionary<Guid, Vm.Vm>();
        }

        public async Task<bool> StartVm(HostModel hostModel)
        {
            // Hosts owned by Players will not be handled by the server.
            if (hostModel.IsPlayerGateway) return false;
            
            var vmLogger = serviceProvider.GetService(typeof(ILogger<Vm.Vm>)) as ILogger<Vm.Vm>;
            var vm = new Vm.Vm(vmLogger, filesystemManager, databaseManager, hostModel);
            await vm.InitializeAsync();
            vms.TryAdd(hostModel.Guid, vm);
            
            return true;
        }
        
        public Vm.Vm GetVm(Guid guid)
        {
            return vms.ContainsKey(guid) 
                ? vms[guid] 
                : null;
        }

        public bool VmExists(Guid guid)
        {
            return vms.ContainsKey(guid);
        }

        public bool TryGetVm(Guid guid, out Vm.Vm vm)
        {
            vm = GetVm(guid);
            return vm != null;
        }
    }
}