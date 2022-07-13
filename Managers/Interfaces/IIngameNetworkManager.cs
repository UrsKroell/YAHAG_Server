using System;
using Shared.IngameNetwork.Connection;

namespace YAHGA_Server.Managers.Interfaces
{
    public interface IIngameNetworkManager
    {
        bool TryGetIngameConnection(Guid connectionGuid, out IngameConnection ingameConnection);
        bool UpdateIngameConnection(IngameConnection ingameConnection);
        bool TryAddIngameConnection(IngameConnection ingameConnection);
        void RemoveIngameConnection(Guid connectionGuid);
    }
}