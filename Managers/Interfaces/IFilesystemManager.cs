using System;
using System.Threading.Tasks;
using VirtFs.NetStandard;

namespace YAHGA_Server.Managers.Interfaces
{
    public interface IFilesystemManager
    {
        Task<Filesystem> GetFilesystem(string hostIp);
        Task<Filesystem> GetFilesystem(Guid hostGuid);
        Task<bool> SaveFilesystem(string hostIp, Filesystem filesystem);
        Task<bool> SaveFilesystem(Guid hostGuid, Filesystem filesystem);
    }
}