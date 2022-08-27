using Microsoft.EntityFrameworkCore;

namespace WebCoseo.AppServices
{
    public class CoreDbContext : DbContext
    {
        public DbSet<Models.SystemConfig>? SystemConfig { get; set; }
        public DbSet<Models.Settings>? Settings { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=WebCoseoCore.db; Password = dbpassword");
            optionsBuilder.UseSqlite("Data Source=WebCoseoCore.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
