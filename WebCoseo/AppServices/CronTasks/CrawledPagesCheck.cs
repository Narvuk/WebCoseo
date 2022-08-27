using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Xml;

namespace WebCoseo.AppServices.CronTasks
{
    public class CrawledPagesCheck
    {
        public static CrawledPagesCheck? _instance;
        bool IsRunning { get; set; }
        public CrawledPagesCheck()
        {
            _instance = this;
        }

        public async void MainTask(int cronid)
        {
            IsRunning = true;
            //MainWindow._instance.UpdateWindowStatusLabel("");
            //Views.CronTasks.CronTasksMain._instance.CT_Update_TelegramBot_Status("Running");
            await Task.Run(async () =>
            {
                while (IsRunning == true)
                {
                    using CoreDbContext coreContext = new();
                    using CrawlersDbContext crawlerContext = new();
                    using (WebcoseoDbContext context = new WebcoseoDbContext())
                    {

                        var getTask = context.CronTasks.Single(ct => ct.Id == cronid);
                        if (getTask.Status.ToString() == "Running")
                        {
                            try
                            {
                                int getCrawledPagesDelay = int.Parse(coreContext.Settings.Single(x => x.SettingKey == "crawledpages_next_check_days").SettingValue);
                                DateTime dateTimeNow = DateTime.Now;
                                var countPages = crawlerContext.CrawledPages.Count(x => x.LastCrawled < dateTimeNow.AddDays(-getCrawledPagesDelay) && x.Status == "Completed");
                                if (countPages > 0)
                                {
                                    var getCrawlerPages = crawlerContext.CrawledPages.Where(x => x.LastCrawled < dateTimeNow.AddDays(-getCrawledPagesDelay) && x.Status == "Completed");

                                    foreach (var getCrawledPage in getCrawlerPages)
                                    {
                                        crawlerContext.Add(new Models.Crawlers.CrawlPool
                                        {
                                            CrawlSitesId = getCrawledPage.CrawlSitesId,
                                            Url = getCrawledPage.Url,
                                            Status = "Waiting",
                                            IsPriority = true,
                                            Created = DateTime.Now,
                                        });
                                        getCrawledPage.Status = "ReCrawling";
                                        crawlerContext.SaveChanges();

                                        // Check Html service Cron if not running do a run of counts,
                                        // if crawler or html checker is running it should auto check counts
                                        var checkCronTask = context.CronTasks.Single(x => x.CronKey == "analysis_htmlchecker");
                                        if (checkCronTask.Status.ToString() != "Running")
                                        {
                                            AnalysisHtmlCheck.Get_CrawledPages_Count();
                                        }
                                    }
                                }
                                // Do action for all call and do check for latest call then search new

                                // 15 Minutes = 900000
                                // 10 Minutes = 600000
                                // 5 Minutes = 300000
                                Thread.Sleep(10000);
                            }
                            catch (Exception e)
                            {

                                if (e.Message == "SQLite Error 5: 'database is locked'.")
                                {
                                    IsRunning = true;
                                    Thread.Sleep(10000);
                                }
                                else
                                {
                                    IsRunning = false;
                                    context.AppLogs.Add(new Models.AppLogs
                                    {
                                        Created = DateTime.Now,
                                        Type = "Error",
                                        Section = "Application",
                                        Message = e.Message.ToString(),

                                    });
                                    context.SaveChanges();
                                }

                            }
                        }
                        if (getTask.Status.ToString() == "Stopped")
                        {
                            IsRunning = false;
                            context.AppLogs.Add(new Models.AppLogs
                            {
                                Created = DateTime.Now,
                                Type = "Warning",
                                Section = "Application",
                                Message = "Sucessfully Stopped " + getTask.Name.ToString() + " Cron Task Service",

                            });
                            context.SaveChanges();

                        }
                    }
                }
            });
            //MainWindow._instance.UpdateWindowStatusLabel("");
            //Views.CronTasks.CronTasksMain._instance.CT_Update_TelegramBot_Status("Stopped");
        }
    }
}
