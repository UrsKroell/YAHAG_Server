using Microsoft.Extensions.Logging;
using Network;
using Network.Enums;
using Shared.Network.Messages;
using Shared.Network.Requests;
using Shared.Network.Responses;
using YAHGA_Server.Managers.Enums;
using YAHGA_Server.Managers.Interfaces;
using YAHGA_Server.PacketHandlers.Interfaces;

namespace YAHGA_Server.PacketHandlers
{
    public class HandshakePacketHandler : IHandshakePacketHandler
    {
        private readonly ILogger<HandshakePacketHandler> logger;
        private readonly IUserManager userManager;

        public HandshakePacketHandler(ILogger<HandshakePacketHandler> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }
        
        public async void HandshakePacketReceived(HandshakeRequest request, Connection con)
        {
            if (request.SteamId == 0)
            {
                con.Send(new HandshakeResponse(false, new ErrorMessage("Invalid SteamId."), request));
                con.Close(CloseReason.ServerClosed);
                return;
            }
            
            var userCreationResponse = await userManager.AddNewUserConnection(request.SteamId, con);
            switch (userCreationResponse)
            {
                case EUserConnectionResponse.Ok:
                    con.Send(new HandshakeResponse(true, null, request));
                    return;
                case EUserConnectionResponse.Unable:
                    con.Send(new HandshakeResponse(false, new ErrorMessage("Unable to connect."), request));
                    con.Close(CloseReason.ServerClosed);
                    return;
                case EUserConnectionResponse.UserBanned:
                    con.Send(new HandshakeResponse(false, new ErrorMessage("Unable to connect. (#99817-a0f7)"), request));
                    con.Close(CloseReason.ServerClosed);
                    return;
            }
        }
    }
}