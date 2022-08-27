using Microsoft.EntityFrameworkCore;

namespace WebCoseo.AppServices
{
    public class WebcoseoDbContext : DbContext
    {
        public DbSet<Models.MySites>? MySites { get; set; }
        public DbSet<Models.MySitesPages>? MySitesPages { get; set; }
        public DbSet<Models.MySitesSiteMaps>? MySitesSiteMaps { get; set; }
        public DbSet<Models.Websites>? Websites { get; set; }
        public DbSet<Models.Backlinks>? Backlinks { get; set; }
        public DbSet<Models.Tasks>? Tasks { get; set; }
        public DbSet<Models.Contacts>? Contacts { get; set; }
        public DbSet<Models.EmailAccounts>? EmailAccounts { get; set; }
        public DbSet<Models.ContactsPhoneNumbers>? ContactsPhoneNumbers { get; set; }
        public DbSet<Models.ContactsSocialMedia>? ContactsSocialMedia { get; set; }
        public DbSet<Models.ContactsWebsites>? ContactsWebsites { get; set; }
        public DbSet<Models.AppLogs>? AppLogs { get; set; }
        public DbSet<Models.CronTasks>? CronTasks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=webcoseo.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
