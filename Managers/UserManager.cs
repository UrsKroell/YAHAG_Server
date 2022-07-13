using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Network;
using YAHGA_Server.Database.Models;
using YAHGA_Server.Managers.Enums;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ILogger<UserManager> logger;
        private readonly IDatabaseManager databaseManager;
        private readonly Dictionary<ulong, Connection> userConnectionDictionary;

        public UserManager(ILogger<UserManager> logger, IDatabaseManager databaseManager)
        {
            this.logger = logger;
            this.databaseManager = databaseManager;
            userConnectionDictionary = new Dictionary<ulong, Connection>();
        }

        public KeyValuePair<ulong, Connection> GetUserConnection(ulong steamid)
        {
            return userConnectionDictionary.FirstOrDefault(kvp => kvp.Key == steamid);
        }
        
        public KeyValuePair<ulong, Connection> GetUserConnection(Connection con)
        {
            return userConnectionDictionary.FirstOrDefault(kvp => kvp.Value == con);
        }

        public async Task<EUserConnectionResponse> AddNewUserConnection(ulong steamid, Connection con)
        {
            var pm = await databaseManager.GetPlayer(steamid);

            if (pm.IsBanned) return EUserConnectionResponse.UserBanned;
            if (!userConnectionDictionary.TryAdd(steamid, con)) return EUserConnectionResponse.Unable;

            return EUserConnectionResponse.Ok;
        }

        public bool RemoveUserConnection(ulong steamid)
        {
            var uc = userConnectionDictionary.FirstOrDefault(kvp => kvp.Key == steamid);
            if (uc.Key == 0 || uc.Value == null) return false;
            
            userConnectionDictionary.Remove(uc.Key);
            return true;
        }
        public bool RemoveUserConnection(Connection con)
        {
            var uc = userConnectionDictionary.FirstOrDefault(kvp => kvp.Value == con);
            if (uc.Key == 0 || uc.Value == null) return false;

            userConnectionDictionary.Remove(uc.Key);
            return true;
        }

        public async Task<bool> CreateNewPlayerEntity(string surname, string givenName, DateTime dateOfBirth,
            string country, string username, string password, Connection con)
        {
            var entityModel = new PrivateEntityModel
            {
                Surname = surname,
                GivenName = givenName,
                DateOfBirth = dateOfBirth,
                Country = country
            };

            var steamId
                = userConnectionDictionary.FirstOrDefault(kvp => kvp.Value == con).Key;

            return await databaseManager.AddNewEntity(entityModel, steamId);
        }

        public int GetConnectedUserCount()
        {
            return userConnectionDictionary.Count;
        }
    }
}