using System;
using Microsoft.Extensions.Logging;
using Network;
using Shared.IngameNetwork.Connection;
using Shared.Network.Messages;
using Shared.Network.Requests.FilesystemRequests.DirectoryRequests;
using Shared.Network.Requests.FilesystemRequests.FileRequests;
using Shared.Network.Responses.FilesystemResponses.DirectoryResponses;
using Shared.Network.Responses.FilesystemResponses.FileResponses;
using YAHGA_Server.Managers.Interfaces;
using YAHGA_Server.PacketHandlers.Interfaces;

namespace YAHGA_Server.PacketHandlers
{
    public class FilesystemPacketHandler : IFilesystemPacketHandler
    {
        private readonly IDatabaseManager databaseManager;
        private readonly IFilesystemManager filesystemManager;
        private readonly IIngameNetworkManager ingameNetworkManager;
        private readonly ILogger<FilesystemPacketHandler> logger;

        public FilesystemPacketHandler(
            ILogger<FilesystemPacketHandler> logger,
            IDatabaseManager databaseManager,
            IFilesystemManager filesystemManager,
            IIngameNetworkManager ingameNetworkManager)
        {
            this.logger = logger;
            this.databaseManager = databaseManager;
            this.filesystemManager = filesystemManager;
            this.ingameNetworkManager = ingameNetworkManager;
        }

        #region File

        public async void CreateFilePacketHandler(CreateFileRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new CreateFileResponse(false, new ErrorMessage("Connection Error."), null, request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new CreateFileResponse(false, new ErrorMessage("Unable to access Host"), null, request));
                return;
            }

            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (fs.FileExists(request.FilePath))
            {
                con.Send(new CreateFileResponse(false, new ErrorMessage("File already exists."), null, request));
                return;
            }

            if (!fs.CreateFile(request.FilePath, request.Content))
            {
                con.Send(new CreateFileResponse(false, new ErrorMessage("Unable to create file."), null, request));
                return;
            }

            if (!fs.TryGetFile(request.FilePath, out var file))
            {
                con.Send(new CreateFileResponse(false, new ErrorMessage("Error on file creation."), null, request));
                return;
            }

            con.Send(new CreateFileResponse(true, null, file, request));
        }

        public async void DeleteFilePacketHandler(DeleteFileRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new DeleteFileResponse(false, new ErrorMessage("Connection Error."), request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new DeleteFileResponse(false, new ErrorMessage("Unable to access Host."), request));
                return;
            }

            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (!fs.TryGetFile(request.FilePath, out var file))
            {
                con.Send(new DeleteFileResponse(false, new ErrorMessage("File does not exist."), request));
                return;
            }

            if (!file.GetParent().RemoveChild(file))
            {
                con.Send(new DeleteFileResponse(false, new ErrorMessage("Unable to delete file."), request));
                return;
            }

            con.Send(new DeleteFileResponse(true, null, request));
        }

        public async void GetFilePacketHandler(GetFileRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new GetFileResponse(false, new ErrorMessage("Connection Error."), null, request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new GetFileResponse(false, new ErrorMessage("Unable to access Host."), null, request));
                return;
            }

            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (!fs.TryGetFile(request.FilePath, out var file))
            {
                con.Send(new GetFileResponse(false, new ErrorMessage("File does not exist."), null, request));
                return;
            }

            con.Send(new GetFileResponse(true, null, file, request));
        }

        public async void SaveFilePacketHandler(SaveFileRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new SaveFileResponse(false, new ErrorMessage("Connection Error."), null, request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new SaveFileResponse(false, new ErrorMessage("Unable to access Host."), null, request));
                return;
            }

            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (!fs.SaveFile(request.File, request.CreateIfNotExists))
            {
                con.Send(new SaveFileResponse(false, new ErrorMessage("Unable to save or create file."), null,
                    request));
                return;
            }

            if (!fs.TryGetFile(request.File.FullPath, out var file))
                con.Send(new SaveFileResponse(false, new ErrorMessage("Error while saving file."), null, request));

            con.Send(new SaveFileResponse(true, null, file, request));
        }

        #endregion

        #region Directory

        public async void CreateDirectoryPacketHandler(CreateDirectoryRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new CreateDirectoryResponse(false, new ErrorMessage("Connection Error."), null, request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new CreateDirectoryResponse(false, new ErrorMessage("Unable to access Host"), null, request));
                return;
            }


            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (fs.DirectoryExists(request.Path))
            {
                con.Send(new CreateDirectoryResponse(false, new ErrorMessage("Directory already exists."), null,
                    request));
                return;
            }

            if (!fs.CreateDirectory(request.Path))
            {
                con.Send(new CreateDirectoryResponse(false, new ErrorMessage("Directory could not be created."), null,
                    request));
                return;
            }

            if (!fs.TryGetDirectory(request.Path, out var dir))
            {
                con.Send(new CreateDirectoryResponse(false, new ErrorMessage("Error on directory creation."), null,
                    request));
                return;
            }

            con.Send(new CreateDirectoryResponse(true, null, dir, request));
        }

        public async void DeleteDirectoryPacketHandler(DeleteDirectoryRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new DeleteDirectoryResponse(false, new ErrorMessage("Connection Error."), request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new DeleteDirectoryResponse(false, new ErrorMessage("Unable to access Host"), request));
                return;
            }

            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (!fs.TryGetDirectory(request.Path, out var dir))
            {
                con.Send(new DeleteDirectoryResponse(false, new ErrorMessage("Directory does not exist."), request));
                return;
            }

            if (!fs.DeleteDirectory(dir.FullPath, request.Recursive))
            {
                con.Send(new DeleteDirectoryResponse(false, new ErrorMessage("Unable to delete directory."), request));
                return;
            }

            con.Send(new DeleteDirectoryResponse(true, null, request));
        }

        public async void GetDirectoryPacketHandler(GetDirectoryRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new GetDirectoryResponse(false, new ErrorMessage("Connection Error."), null, request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new GetDirectoryResponse(false, new ErrorMessage("Unable to access Host"), null, request));
                return;
            }

            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (!fs.TryGetDirectory(request.Path, out var dir))
            {
                con.Send(new GetDirectoryResponse(false, new ErrorMessage("Directory does not exist."), null, request));
                return;
            }
            
            con.Send(new GetDirectoryResponse(true, null, dir, request));
        }

        public async void SaveDirectoryPacketHandler(SaveDirectoryRequest request, Connection con)
        {
            Guid targetGuid;
            if (!ingameNetworkManager.TryGetIngameConnection(request.IngameConnection.Guid,
                out var ingameConnection))
            {
                targetGuid = request.IngameConnection.ClientGuid;
            }
            else
            {
                if (ingameConnection.ConnectionStatus != EConnectionStatus.Closed)
                {
                    targetGuid = ingameConnection.HostGuid;
                }
                else
                {
                    con.Send(new SaveDirectoryResponse(false, new ErrorMessage("Connection Error."), null, request));
                    return;
                }
            }

            var host = await databaseManager.GetHost(targetGuid);

            if (host.IsPlayerGateway)
            {
                con.Send(new SaveDirectoryResponse(false, new ErrorMessage("Unable to access Host"), null, request));
                return;
            }

            var fs = await filesystemManager.GetFilesystem(host.Guid);
            if (!fs.SaveDirectory(request.Directory, request.CreateIfNotExists))
            {
                con.Send(new SaveDirectoryResponse(false, new ErrorMessage("Unable to save or create directory."), null,
                        request));
                return;
            }

            if (!fs.TryGetDirectory(request.Directory.FullPath, out var dir))
            {
                con.Send(new SaveDirectoryResponse(false, new ErrorMessage("Error while saving directory."), null,
                    request));
                return;
            }

            con.Send(new SaveDirectoryResponse(true, null, dir, request));
        }

        #endregion
    }
}