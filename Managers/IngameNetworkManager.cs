using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Shared.IngameNetwork.Connection;
using YAHGA_Server.Managers.Interfaces;

namespace YAHGA_Server.Managers
{
    public class IngameNetworkManager : IIngameNetworkManager
    {
        private readonly ILogger<IngameNetworkManager> logger;
        private readonly Dictionary<Guid, IngameConnection> ingameConnections;

        public IngameNetworkManager(ILogger<IngameNetworkManager> logger)
        {
            this.logger = logger;
            ingameConnections = new Dictionary<Guid, IngameConnection>();
        }
        
        public bool TryGetIngameConnection(Guid connectionGuid, out IngameConnection ingameConnection)
        {
            ingameConnection = null;
            if (!ingameConnections.ContainsKey(connectionGuid)) return false;

            ingameConnection = ingameConnections[connectionGuid];
            return true;
        }

        public bool UpdateIngameConnection(IngameConnection ingameConnection)
        {
            if (!ingameConnections.ContainsKey(ingameConnection.Guid)) return false;
            ingameConnections[ingameConnection.Guid] = ingameConnection;
            return true;
        }
        
        public bool TryAddIngameConnection(IngameConnection ingameConnection)
        {
            return ingameConnections.TryAdd(ingameConnection.Guid, ingameConnection);
        }

        public void RemoveIngameConnection(Guid connectionGuid)
        {
            ingameConnections.Remove(connectionGuid);
        }
    }
}