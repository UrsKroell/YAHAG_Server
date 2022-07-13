using System.ComponentModel.DataAnnotations;

namespace YAHGA_Server.Database.Models
{
    public class PlayerModel
    {
        [Key]
        [Required]
        public ulong SteamId { get; set; }
        public PrivateEntityModel Entity { get; set; }
        
        public bool IsBanned { get; set; }
        public bool IsAdmin { get; set; }
    }
}