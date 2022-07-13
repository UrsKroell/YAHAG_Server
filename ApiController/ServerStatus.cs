namespace YAHGA_Server.ApiController
{
    public class ServerStatus
    {
        public bool IsServerRunning { get; set; }
        public int TcpConnections { get; set; }
        public int ConnectedClients { get; set; }
    }
}