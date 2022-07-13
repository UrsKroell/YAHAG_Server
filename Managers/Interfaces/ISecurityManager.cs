using System.Threading.Tasks;

namespace YAHGA_Server.Managers.Interfaces
{
    public interface ISecurityManager
    {
        (string, string) CreateHashAndSalt(string password);
        Task<bool> CheckPassword(string username, string password);
    }
}