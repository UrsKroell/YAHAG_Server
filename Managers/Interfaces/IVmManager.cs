using System;
using System.Threading.Tasks;
using YAHGA_Server.Database.Models;

namespace YAHGA_Server.Managers.Interfaces
{
    public interface IVmManager
    {
        Task<bool> StartVm(HostModel hostModel);
        Vm.Vm GetVm(Guid guid);

        bool VmExists(Guid guid);
        bool TryGetVm(Guid guid, out Vm.Vm vm);
    }
}