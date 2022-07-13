using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YAHGA_Server.Database.Models
{
    public class HostModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }
        
        public string Ip { get; set; }
        
        public FileSystemModel FileSystem { get; set; }
        
        public bool IsActive { get; set; }
        
        public PrivateEntityModel OwnerPrivateEntity { get; set; }
        
        public PublicEntityModel OwnerPublicEntity { get; set; }
        
        public bool IsPlayerGateway { get; set; }
    }
}