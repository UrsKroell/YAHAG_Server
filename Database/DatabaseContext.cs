using Microsoft.EntityFrameworkCore;
using YAHGA_Server.Database.Models;

namespace YAHGA_Server.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PlayerModel> Players { get; set; }
        public DbSet<PrivateEntityModel> PrivateEntities { get; set; }
        public DbSet<PublicEntityModel> PublicEntities { get; set; }
        public DbSet<HostModel> Hosts { get; set; }
        public DbSet<FileSystemModel> FileSystems { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {}
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}