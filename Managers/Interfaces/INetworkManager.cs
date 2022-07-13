namespace YAHGA_Server.Managers.Interfaces
{
    public interface INetworkManager
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
        int GetConnectionCount();
    }
}