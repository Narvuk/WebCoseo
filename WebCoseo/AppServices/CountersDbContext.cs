using Microsoft.EntityFrameworkCore;

namespace WebCoseo.AppServices
{
    public class CountersDbContext : DbContext
    {
        public DbSet<Models.Counters.CrawlerCounters>? CrawlerCounters { get; set; }
        public DbSet<Models.Counters.BacklinksCounters>? BacklinksCounters { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=webcoseo-counters.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}