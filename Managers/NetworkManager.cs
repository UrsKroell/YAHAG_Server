using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Network;
using Network.Enums;
using Shared.Network.Requests;
using Shared.Network.Requests.FilesystemRequests.DirectoryRequests;
using Shared.Network.Requests.FilesystemRequests.FileRequests;
using YAHGA_Server.Managers.Interfaces;
using YAHGA_Server.PacketHandlers.Interfaces;

namespace YAHGA_Server.Managers
{
    public class NetworkManager : INetworkManager
    {
        private readonly ILogger<NetworkManager> logger;
        private readonly IConfiguration configuration;
        private readonly IUserManager userManager;
        private readonly IHandshakePacketHandler handshakePacketHandler;
        private readonly IFilesystemPacketHandler filesystemPacketHandler;
        private readonly INewPlayerPacketHandler newPlayerPacketHandler;

        private ServerConnectionContainer connectionContainer;
        private readonly int port;
        public bool IsRunning { get; private set; }

        public NetworkManager(
            ILogger<NetworkManager> logger,
            IConfiguration configuration,
            IUserManager userManager,
            IHandshakePacketHandler handshakePacketHandler,
            IFilesystemPacketHandler filesystemPacketHandler,
            INewPlayerPacketHandler newPlayerPacketHandler)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.userManager = userManager;
            this.handshakePacketHandler = handshakePacketHandler;
            this.filesystemPacketHandler = filesystemPacketHandler;
            this.newPlayerPacketHandler = newPlayerPacketHandler;

            port = configuration.GetValue<int>("ServerConfiguration:Port");
            Build();
        }

        private void Build()
        {
            connectionContainer = ConnectionFactory.CreateSecureServerConnectionContainer(port, start: false, keySize: 2048);
            //connectionContainer = ConnectionFactory.CreateServerConnectionContainer(port, start: false);
            #region Settings

            connectionContainer.AddKownType(typeof(HandshakeRequest).Assembly);
            
            connectionContainer.AllowUDPConnections = false;
            connectionContainer.ConnectionEstablished += OnConnectionEstablished;
            connectionContainer.ConnectionLost += OnConnectionLost;

            #endregion
        }

        public void Start()
        {
            IsRunning = true;
            connectionContainer.Start();
            logger.LogInformation("NetworkManager.Server running");
            logger.LogInformation($"tcp://{connectionContainer.IPAddress}:{connectionContainer.Port}");
        }

        public void Stop()
        {
            IsRunning = false;
            CloseConnections();
            connectionContainer.Stop();
        }

        public int GetConnectionCount()
        {
            return connectionContainer.TCP_Connections.Count;
        }

        private void CloseConnections()
        {
            logger.LogInformation("Closing Connections");
            logger.LogInformation($"Connections to close: {connectionContainer.TCP_Connections.Count}");
            
            foreach (var connection in connectionContainer.TCP_Connections)
            {
                userManager.RemoveUserConnection(connection);
                connection.Close(CloseReason.ServerClosed, false);
            }
            
            logger.LogInformation("Connections closed");
        }

        private void OnConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            if (closeReason != CloseReason.ClientClosed)
            {
                logger.LogWarning(
                    $"Lost connection to {connection.IPRemoteEndPoint.Address} | Reason {Enum.GetName(typeof(CloseReason), closeReason)}");
            }
            userManager.RemoveUserConnection(connection);
        }

        private void OnConnectionEstablished(Connection connection, ConnectionType type)
        {
            connection.KeepAlive = true;
            connection.EnableLogging = true;
            
            logger.LogInformation($"New connection {connection.IPRemoteEndPoint.Address}");
            logger.LogInformation($"ConnectionType {Enum.GetName(typeof(ConnectionType), type)}");
            
            connection.RegisterStaticPacketHandler<HandshakeRequest>(handshakePacketHandler.HandshakePacketReceived);
            connection.RegisterStaticPacketHandler<NewPlayerRequest>(newPlayerPacketHandler.NewPlayerPacketReceived);
            
            RegisterFilesystemPacketHandlers(ref connection);
        }

        private void RegisterFilesystemPacketHandlers(ref Connection connection)
        {
            connection.RegisterStaticPacketHandler<CreateFileRequest>(filesystemPacketHandler.CreateFilePacketHandler);
            connection.RegisterStaticPacketHandler<DeleteFileRequest>(filesystemPacketHandler.DeleteFilePacketHandler);
            connection.RegisterStaticPacketHandler<GetFileRequest>(filesystemPacketHandler.GetFilePacketHandler);
            connection.RegisterStaticPacketHandler<SaveFileRequest>(filesystemPacketHandler.SaveFilePacketHandler);
            
            connection.RegisterStaticPacketHandler<CreateDirectoryRequest>(filesystemPacketHandler.CreateDirectoryPacketHandler);
            connection.RegisterStaticPacketHandler<DeleteDirectoryRequest>(filesystemPacketHandler.DeleteDirectoryPacketHandler);
            connection.RegisterStaticPacketHandler<GetDirectoryRequest>(filesystemPacketHandler.GetDirectoryPacketHandler);
            connection.RegisterStaticPacketHandler<SaveDirectoryRequest>(filesystemPacketHandler.SaveDirectoryPacketHandler);
        }
    }
}