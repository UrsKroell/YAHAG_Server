using Network;
using Shared.Network.Requests;

namespace YAHGA_Server.PacketHandlers.Interfaces
{
    public interface INewPlayerPacketHandler
    {
        void NewPlayerPacketReceived(NewPlayerRequest packet, Connection con);
    }
}