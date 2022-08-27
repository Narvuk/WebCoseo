using Microsoft.EntityFrameworkCore;
using WebCoseo.Models.Reports.Backlinks.Dashboard;
using WebCoseo.Models.Reports.Backlinks.Websites;
using WebCoseo.Models.Reports.MySites.Dashboard;
using WebCoseo.Models.Reports.Websites.Dashboard;

namespace WebCoseo.AppServices
{
    public class ReportsDbContext : DbContext
    {
        public DbSet<BL_Websites_Monthly_Total>? BL_Websites_Monthly_Total { get; set; }
        public DbSet<BL_Websites_Monthly_New>? BL_Websites_Monthly_New { get; set; }
        public DbSet<BL_Websites_Monthly_Active>? BL_Websites_Monthly_Active { get; set; }
        public DbSet<BL_Websites_Monthly_Lost>? BL_Websites_Monthly_Lost { get; set; }
        public DbSet<BL_Dash_Monthly_Total>? BL_Dash_Monthly_Total { get; set; }
        public DbSet<BL_Dash_Monthly_Active_Total>? BL_Dash_Monthly_Active_Total { get; set; }
        public DbSet<BL_Dash_Monthly_New_Total>? BL_Dash_Monthly_New_Total { get; set; }
        public DbSet<BL_Dash_Monthly_Lost_Total>? BL_Dash_Monthly_Lost_Total { get; set; }
        public DbSet<MySites_Dash_Monthly_Total>? MySites_Dash_Monthly_Total { get; set; }
        public DbSet<MySitesPages_Dash_Monthly_Total>? MySitesPages_Dash_Monthly_Total { get; set; }
        public DbSet<Websites_Dash_Monthly_Total>? Websites_Dash_Monthly_Total { get; set; }
        public DbSet<Websites_Dash_Monthly_NoBacklinks_Total>? Websites_Dash_Monthly_NoBacklinks_Total { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=webcoseo-reports.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}