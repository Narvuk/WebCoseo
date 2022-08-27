using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebCoseo.AppServices.Crawlers;

namespace WebCoseo.Helpers.Crawlers
{
    public class CrawlerPoolStart
    {
        public static CrawlerPoolStart? _instance;
        public CrawlerPoolStart()
        {
            _instance = this;
        }

        public async void Run()
        {
            using AppServices.CoreDbContext coreContext = new();
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                //bool isRunning = true;
                int getMaxCrawlers = int.Parse(coreContext.Settings.Single(cs => cs.SettingKey == "crawlers_crawlpool_maxcrawlers").SettingValue);
                int counter = 0;
                int poolCount = CrawlPoolService.Get_CrawlPool_Count();
                if (poolCount > 0)
                {
                    while (counter <= getMaxCrawlers)
                    {
                        using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
                        {

                            var getCronTask = context.CronTasks.Single(ct => ct.CronKey == "crawler_crawlpool_service");
                            using (AppServices.CountersDbContext countersContext = new AppServices.CountersDbContext())
                            {
                                var getActiveCrawlers = countersContext.CrawlerCounters.Single(cs => cs.CounterKey == "active_crawlers");

                                await Task.Run(() =>
                                {
                                    var startCrawler = new CrawlPoolService();
                                    startCrawler.MainTask();
                                    getActiveCrawlers.CounterValue++;
                                    countersContext.SaveChanges();

                                    // Sleep Within the thread it will stop the GUI freezing
                                    Thread.Sleep(500);
                                });

                            }
                        }
                        counter++;
                    }
                    
                }
            }
        }
    }
}
