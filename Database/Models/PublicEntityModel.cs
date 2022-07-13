using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YAHGA_Server.Database.Models
{
    public class PublicEntityModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool IsLawEnforcement { get; set; }
        public bool IsHealthCareProvider { get; set; }
        public bool IsGovernmentEntity { get; set; }
    }
}