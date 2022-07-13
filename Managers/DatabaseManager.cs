using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YAHGA_Server.Database;
using YAHGA_Server.Database.Models;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.Managers
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly ILogger<DatabaseManager> logger;
        private readonly DatabaseContext context;

        public DatabaseManager(ILogger<DatabaseManager> logger, DatabaseContext context)
        {
            this.logger = logger;
            this.context = context;

            this.context.Database.Migrate();
        }
        
        public async Task<bool> CreateNewPlayer(ulong steamId)
        {
            await using var ctx = context;
            if (await ctx.Players.AnyAsync(p => p.SteamId == steamId))
                return false;

            await ctx.Players.AddAsync(new PlayerModel { SteamId = steamId });

            var result = await ctx.SaveChangesAsync();
            
            return result > 0;
        }

        public async Task<bool> IsEntityOwnedByPlayer(Guid entityGuid)
        {
            await using var ctx = context;
            return await ctx.Players
                .Include(p => p.Entity)
                .AsNoTracking()
                .AnyAsync(p => p.Entity.Guid == entityGuid);
        }
        
        public async Task<bool> AddNewEntity(PrivateEntityModel entityModel, ulong steamId)
        {
            await using var ctx = context;
            
            var e = await ctx.PrivateEntities.AddAsync(entityModel);
            entityModel = e.Entity;

            if (await ctx.SaveChangesAsync() <= 0)
            {
                return false;
            }

            if (!await ctx.Players.AnyAsync(p => p.SteamId == steamId))
            {
                if (!await CreateNewPlayer(steamId))
                {
                    return false;
                }
            }
            
            var player = await ctx.Players.SingleAsync(p => p.SteamId == steamId);
            player.Entity = entityModel;

            return await ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateNewHost()
        {
            return false;
        }

        public async Task<string> GetFilesystem(string hostIp)
        {
            await using var ctx = context;
            var host = await ctx.Hosts
                .Include(h => h.FileSystem)
                .AsNoTracking()
                .SingleAsync(h => h.Ip == hostIp);

            var jsonBytes = CompressionManager.DecompressAndBase64Decode(host.FileSystem.Json);
            return Encoding.UTF8.GetString(jsonBytes);
        }
        
        public async Task<string> GetFilesystem(Guid hostGuid)
        {
            await using var ctx = context;
            var host = await ctx.Hosts
                .Include(h => h.FileSystem)
                .AsNoTracking()
                .SingleAsync(h => h.Guid == hostGuid);

            var jsonBytes = CompressionManager.DecompressAndBase64Decode(host.FileSystem.Json);
            return Encoding.UTF8.GetString(jsonBytes);
        }

        public async Task<bool> SaveFilesystem(string hostIp, string json)
        {
            await using var ctx = context;
            var host = await ctx.Hosts.SingleAsync(h => h.Ip == hostIp);
            
            return await SaveFilesystem(host.Guid, json);
        }
        
        public async Task<bool> SaveFilesystem(Guid hostGuid, string json)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var base64 = CompressionManager.CompressAndBase64Encode(jsonBytes);
            
            await using var ctx = context;
            var host = await ctx.Hosts
                .Include(h => h.FileSystem)
                .SingleAsync(h => h.Guid == hostGuid);

            host.FileSystem.Json = base64;
            return await ctx.SaveChangesAsync() > 0;
        }

        public async Task<HostModel> GetHost(Guid hostGuid)
        {
            await using var ctx = context;
            var host = await ctx.Hosts
                .AsNoTracking()
                .SingleAsync(h => h.Guid == hostGuid);

            return host;
        }

        public async Task<PlayerModel> GetPlayer(ulong steamId)
        {
            await using var ctx = context;
            var pm = await ctx.Players
                .AsNoTracking()
                .SingleAsync(p => p.SteamId == steamId);
            return pm;
        }
    }
}