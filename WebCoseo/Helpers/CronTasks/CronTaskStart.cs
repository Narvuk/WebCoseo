using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCoseo.AppServices.CronTasks;

namespace WebCoseo.Helpers.CronTasks
{
    public class CronTaskStart
    {
        public static CronTaskStart? _instance;
        public CronTaskStart()
        {
            _instance = this;
        }

        public async void Run(int cronid)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getTask = context.CronTasks.Where(ct => ct.Id == cronid).Single();

                if (getTask.CronKey == "app_update_service")
                {

                    getTask.Status = "Running";
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Started " + getTask.Name.ToString()

                    });
                    context.SaveChanges();
                    var startAppUpdateService = new AppUpdateCheck();
                    startAppUpdateService.MainTask(cronid);
                }

                if (getTask.CronKey == "backlinks_checker_service")
                {

                    getTask.Status = "Running";
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Started " + getTask.Name.ToString()

                    });
                    context.SaveChanges();
                    var startBackLinksCheckerService = new BacklinksChecker();
                    startBackLinksCheckerService.MainTask(cronid);
                }

                if (getTask.CronKey == "reports_service")
                {

                    getTask.Status = "Running";
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Started " + getTask.Name.ToString()

                    });
                    context.SaveChanges();
                    var startReportsService = new ReportsUpdater();
                    startReportsService.MainTask(cronid);
                }

                if (getTask.CronKey == "analysis_htmlchecker")
                {

                    getTask.Status = "Running";

                    context.SaveChanges();
                    using AppServices.CoreDbContext coreContext = new();
                    using (AppServices.CountersDbContext countersContext = new AppServices.CountersDbContext())
                    {
                        // Fix stale or missing finished scanners
                        var getActiveScanners = countersContext.CrawlerCounters.Single(cs => cs.CounterKey == "active_scanners");
                        if (getActiveScanners.CounterValue > 0)
                        {
                            getActiveScanners.CounterValue = 0;
                            countersContext.SaveChanges();
                        }
                        int getMaxScanners = int.Parse(coreContext.Settings.Single(cs => cs.SettingKey == "analysis_maxscanners").SettingValue);
                        int counter = 0;
                        while (counter <= getMaxScanners)
                        {
                            getActiveScanners = countersContext.CrawlerCounters.Single(cs => cs.CounterKey == "active_scanners");

                            await Task.Run(() =>
                            {

                                var startAnalysisHtmlCheckerService = new AnalysisHtmlCheck();
                                startAnalysisHtmlCheckerService.MainTask(cronid);
                                getActiveScanners.CounterValue++;
                                countersContext.SaveChanges();

                            // Sleep Within the thread it will stop the GUI freezing
                                Thread.Sleep(500);
                            });
                            counter++;
                        }
                    }
                }

                if (getTask.CronKey == "sitemaps_checker")
                {

                    getTask.Status = "Running";
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Started " + getTask.Name.ToString()

                    });
                    context.SaveChanges();
                    var startSitemapsCheckerService = new SiteMapsCheck();
                    startSitemapsCheckerService.MainTask(cronid);
                }

                if (getTask.CronKey == "crawler_crawlpages_check_service")
                {

                    getTask.Status = "Running";
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Started " + getTask.Name.ToString()

                    });
                    context.SaveChanges();
                    var startCrawledPagesCheckService = new CrawledPagesCheck();
                    startCrawledPagesCheckService.MainTask(cronid);
                }

                if (getTask.CronKey == "mysites_counts_checker")
                {

                    getTask.Status = "Running";
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Started " + getTask.Name.ToString()

                    });
                    context.SaveChanges();
                    var startMySitesCountsService = new MySitesCountsCheck();
                    startMySitesCountsService.MainTask(cronid);
                }


            }

        }
    }
}
