using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using YAHGA_Server.Managers.Enums;

namespace YAHGA_Server.Managers.Interfaces
{
    public interface IUserManager
    {
        KeyValuePair<ulong, Connection> GetUserConnection(ulong steamid);
        KeyValuePair<ulong, Connection> GetUserConnection(Connection con);
        Task<EUserConnectionResponse> AddNewUserConnection(ulong steamid, Connection con);
        bool RemoveUserConnection(ulong steamid);
        bool RemoveUserConnection(Connection con);

        Task<bool> CreateNewPlayerEntity(string surname, string givenName, DateTime dateOfBirth,
            string country, string username, string password, Connection con);

        int GetConnectedUserCount();
    }
}