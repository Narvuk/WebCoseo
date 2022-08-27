using Microsoft.EntityFrameworkCore;

namespace WebCoseo.AppServices
{
    public class CrawlersDbContext : DbContext
    {
        public DbSet<Models.Crawlers.CrawlPool>? CrawlPool { get; set; }
        public DbSet<Models.Crawlers.CrawledPages>? CrawledPages { get; set; }
        public DbSet<Models.Crawlers.CrawlSites>? CrawlSites { get; set; }
        public DbSet<Models.Crawlers.NewSites>? NewSites { get; set; }
        public DbSet<Models.Crawlers.SkipCrawl>? SkipCrawl { get; set; }
        public DbSet<Models.Crawlers.SkipContains>? SkipContains { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=webcoseo-crawlers.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}