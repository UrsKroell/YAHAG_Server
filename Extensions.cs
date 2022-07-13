using Shared.IngameNetwork;
using YAHGA_Server.Database.Models;

namespace YAHGA_Server
{
    public static class Extensions
    {
        public static HostModel ToHostModel(this Host host)
        {
            return  new HostModel
            {
                Guid = host.Guid,
                Ip = host.Ip
            };
        }

        public static Host ToHost(this HostModel hostModel)
        {
            return new Host
            {
                Guid = hostModel.Guid,
                Ip = hostModel.Ip
            };
        }
    }
}