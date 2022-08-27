using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.Helpers.Counters
{
    public class CrawlersDbdata
    {
        public static CrawlersDbdata? _instance;
        public CrawlersDbdata()
        {
            _instance = this;
        }

        public async Task RunCheck()
        {
            await Task.Run(() =>
            {
                Crawler_Counters_Check();
            });
        }


        public void Crawler_Counters_Check()
        {
            using (AppServices.CountersDbContext counterContext = new AppServices.CountersDbContext())
            {
                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "crawlers_pool").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 1,
                        Name = "Crawlers Pool Count",
                        CounterKey = "crawlers_pool",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "active_crawlers").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 2,
                        Name = "Active Crawlers Count",
                        CounterKey = "active_crawlers",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "waiting_crawlers").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 3,
                        Name = "Waiting Crawlers Count",
                        CounterKey = "waiting_crawlers",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }
                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "died_crawlers").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 4,
                        Name = "Died Crawlers Count",
                        CounterKey = "died_crawlers",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }
                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "approved_sites").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 5,
                        Name = "Approved Sites Count",
                        CounterKey = "approved_sites",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "new_sites").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 6,
                        Name = "New Found Sites Count",
                        CounterKey = "new_sites",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "crawled_pages").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 7,
                        Name = "Crawled Pages Count",
                        CounterKey = "crawled_pages",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "active_scanners").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 8,
                        Name = "Active Scanners Count",
                        CounterKey = "active_scanners",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "waiting_scanners").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 9,
                        Name = "Waiting Scanners Count",
                        CounterKey = "waiting_scanners",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "died_scanners").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 10,
                        Name = "Died Scanners Count",
                        CounterKey = "died_scanners",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "priority_crawledpages").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 11,
                        Name = "Priority Crawled Pages",
                        CounterKey = "priority_crawledpages",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

                try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "priority_crawlpool").Single().Id.ToString(); }
                catch (Exception)
                {
                    counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                    {
                        Id = 12,
                        Name = "Priority Crawl Pool",
                        CounterKey = "priority_crawlpool",
                        CounterValue = 0
                    });
                    counterContext.SaveChanges();
                    Crawler_Counters_Check();
                }

            }

        }

    }
}
