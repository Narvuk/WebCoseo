using System.Linq;

namespace WebCoseo.Helpers.CronTasks
{
    public class CronTaskStop
    {
        public static CronTaskStop? _instance;
        public CronTaskStop()
        {
            _instance = this;
        }

        public void Run(int cronid)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getTask = context.CronTasks.Where(ct => ct.Id == cronid).Single();

                if (getTask.CronKey == "app_update_service")
                {
                    getTask.Status = "Stopped";
                    context.SaveChanges();
                }

                if (getTask.CronKey == "backlinks_checker_service")
                {
                    getTask.Status = "Stopped";
                    context.SaveChanges();
                }

                if (getTask.CronKey == "reports_service")
                {
                    getTask.Status = "Stopped";
                    context.SaveChanges();
                }
                if (getTask.CronKey == "analysis_htmlchecker")
                {
                    getTask.Status = "Stopped";
                    context.SaveChanges();
                }
                if (getTask.CronKey == "sitemaps_checker")
                {
                    getTask.Status = "Stopped";
                    context.SaveChanges();
                }
                if (getTask.CronKey == "mysites_counts_checker")
                {
                    getTask.Status = "Stopped";
                    context.SaveChanges();
                }

            }

        }
    }
}
