using System;
using System.Threading.Tasks;
using YAHGA_Server.Database.Models;

namespace YAHGA_Server.Managers.Interfaces
{
    public interface IDatabaseManager
    {
        Task<bool> CreateNewPlayer(ulong steamId);
        Task<bool> IsEntityOwnedByPlayer(Guid entityGuid);
        Task<bool> AddNewEntity(PrivateEntityModel entityModel, ulong steamId);
        Task<bool> CreateNewHost();
        Task<string> GetFilesystem(string hostIp);
        Task<string> GetFilesystem(Guid hostGuid);
        Task<bool> SaveFilesystem(string hostIp, string json);
        Task<bool> SaveFilesystem(Guid hostGuid, string json);
        Task<HostModel> GetHost(Guid hostGuid);
        Task<PlayerModel> GetPlayer(ulong steamId);
    }
}