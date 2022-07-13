using Network;
using Shared.Network.Requests;

namespace YAHGA_Server.PacketHandlers.Interfaces
{
    public interface IHandshakePacketHandler
    {
        void HandshakePacketReceived(HandshakeRequest request, Connection con);
    }
}