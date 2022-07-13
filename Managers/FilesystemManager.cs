using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtFs.NetStandard;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.Managers
{
    public class FilesystemManager : IFilesystemManager
    {
        private readonly ILogger<FilesystemManager> logger;
        private readonly IDatabaseManager databaseManager;

        public FilesystemManager(ILogger<FilesystemManager> logger, IDatabaseManager databaseManager)
        {
            this.logger = logger;
            this.databaseManager = databaseManager;
        }
        
        public async Task<Filesystem> GetFilesystem(string hostIp)
        {
            var serializer = new FilesystemSerializer();
            var json = await databaseManager.GetFilesystem(hostIp);
            var fs = serializer.Deserialize(json);
            return fs;
        }
        
        public async Task<Filesystem> GetFilesystem(Guid hostGuid)
        {
            var serializer = new FilesystemSerializer();
            var json = await databaseManager.GetFilesystem(hostGuid);
            var fs = serializer.Deserialize(json);
            return fs;
        }

        public async Task<bool> SaveFilesystem(string hostIp, Filesystem filesystem)
        {
            var serializer = new FilesystemSerializer();
            var json = serializer.Serialize(filesystem);
            return await databaseManager.SaveFilesystem(hostIp, json);
        }
        
        public async Task<bool> SaveFilesystem(Guid hostGuid, Filesystem filesystem)
        {
            var serializer = new FilesystemSerializer();
            var json = serializer.Serialize(filesystem);
            return await databaseManager.SaveFilesystem(hostGuid, json);
        }
    }
}