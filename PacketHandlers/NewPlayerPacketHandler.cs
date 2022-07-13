using Microsoft.Extensions.Logging;
using Network;
using Shared.Network.Messages;
using Shared.Network.Requests;
using Shared.Network.Responses;
using YAHGA_Server.Managers.Interfaces;
using YAHGA_Server.PacketHandlers.Interfaces;

namespace YAHGA_Server.PacketHandlers
{
    public class NewPlayerPacketHandler : INewPlayerPacketHandler
    {
        private readonly ILogger<NewPlayerPacketHandler> logger;
        private readonly IUserManager userManager;

        public NewPlayerPacketHandler(ILogger<NewPlayerPacketHandler> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }
        
        public async void NewPlayerPacketReceived(NewPlayerRequest packet, Connection con)
        {
            var result = await userManager.CreateNewPlayerEntity(packet.Surname, packet.GivenName,
                packet.DateOfBirth, packet.Country, packet.Username, packet.Password, con);

            con.Send(result
                ? new NewPlayerResponse(true, null, packet)
                : new NewPlayerResponse(false, new ErrorMessage("Unable to create new Entity."), packet));
        }
    }
}