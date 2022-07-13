using Network;
using Shared.Network.Requests.FilesystemRequests.DirectoryRequests;
using Shared.Network.Requests.FilesystemRequests.FileRequests;

namespace YAHGA_Server.PacketHandlers.Interfaces
{
    public interface IFilesystemPacketHandler
    {
        void CreateFilePacketHandler(CreateFileRequest request, Connection con);
        void DeleteFilePacketHandler(DeleteFileRequest request, Connection con);
        void GetFilePacketHandler(GetFileRequest request, Connection con);
        void SaveFilePacketHandler(SaveFileRequest request, Connection con);
        void CreateDirectoryPacketHandler(CreateDirectoryRequest request, Connection con);
        void DeleteDirectoryPacketHandler(DeleteDirectoryRequest request, Connection con);
        void GetDirectoryPacketHandler(GetDirectoryRequest request, Connection con);
        void SaveDirectoryPacketHandler(SaveDirectoryRequest request, Connection con);
    }
}