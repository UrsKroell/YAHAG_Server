using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YAHGA_Server.Database.Models
{
    public class FileSystemModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }
        
        [Column(TypeName = "json")]
        [Description("Lz4 compressed and base64 encoded json of a filesystem")]
        public string Json { get; set; }
    }
}