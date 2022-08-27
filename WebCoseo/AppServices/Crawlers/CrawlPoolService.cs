using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Crawlers
{
    public class CrawlPoolService
    {
        public static CrawlPoolService? _instance;

        bool isRunning { get; set; }
        public CrawlPoolService()
        {
            _instance = this;
        }

        public async void MainTask()
        {
            isRunning = true;
            await Task.Run(async () =>
            {
                while (isRunning == true)
                {
                    int cpId = 0;
                    using CoreDbContext coreContext = new();
                    using (WebcoseoDbContext context = new WebcoseoDbContext())
                    {

                        var getTask = context.CronTasks.Single(ct => ct.CronKey == "crawler_crawlpool_service");
                        try
                        {
                            if (getTask.Status.ToString() == "Running" && Get_CrawlPool_Count() > 0)
                            {
                                try
                                {
                                    using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
                                    {
                                        //get the counters
                                        int poolToSkip = 0;

                                        var dtnow = DateTime.Now;
                                        var getPoolCountPriority = crawlerContext.CrawlPool.Where(gpa => gpa.Status == "Waiting" && gpa.IsPriority == true).Count();
                                        var getPoolCount = crawlerContext.CrawlPool.Where(gpa => gpa.Status == "Waiting").Count();
                                        Random rand = new Random();
                                        if (getPoolCountPriority > 0)
                                        {
                                            poolToSkip = rand.Next(0, getPoolCountPriority);
                                        }
                                        else
                                        {
                                            poolToSkip = rand.Next(0, getPoolCount);
                                        }
                                        if (getPoolCount > 0)
                                        {
                                            var getNextPage = crawlerContext.CrawlPool.Where(gnp => gnp.Status == "Waiting").OrderBy(gp => gp.Created).Skip(poolToSkip).First();
                                            if (getPoolCountPriority > 0)
                                            {
                                                getNextPage = crawlerContext.CrawlPool.Where(gnp => gnp.Status == "Waiting" && gnp.IsPriority == true).OrderBy(gp => gp.Created).Skip(poolToSkip).First();
                                            }
                                            cpId = getNextPage.Id;
                                            // We have now claimed the site made sure another crawler does not take it
                                            Change_Pool_Record_Status(cpId, "Working");

                                            var getWebsite = crawlerContext.CrawlSites.Single(gs => gs.Id == getNextPage.CrawlSitesId);
                                            var getPageUrl = getNextPage.Url;

                                            if (getWebsite.Status == "Paused")
                                            {
                                                Change_Pool_Record_Status(cpId, "Paused");
                                            }

                                            if (getWebsite.Status != "Paused")
                                            {

                                                // Format internal domain Sites
                                                var checkContains = getWebsite.Url.Replace("https://", "");
                                                // The external site check
                                                var checkContainsExternal = getPageUrl.Replace("https://", "");
                                                var getHostUrl = new Uri(getPageUrl).Host.ToString();
                                                var checkSkipDomains = crawlerContext.SkipCrawl.Where(d => d.Domain.Contains(getHostUrl)).Count();
                                                int checkSkipContainsCount = 0;
                                                var getSkipContainsList = crawlerContext.SkipContains.OrderBy(x => x.Id);

                                                if (getSkipContainsList != null)
                                                {
                                                    foreach (var sc in getSkipContainsList)
                                                    {
                                                        if (getPageUrl.Contains(sc.Phrase))
                                                        {
                                                            checkSkipContainsCount++;
                                                        }
                                                    }
                                                }

                                                // end external url check
                                                
                                                if (checkSkipDomains <= 0 && checkSkipContainsCount <= 0)
                                                {

                                                    int getClaimedCount = crawlerContext.CrawlPool
                                                    .Where(x => x.Status == "Working").Where(xc => xc.Url.Contains(checkContains)).Count();

                                                    int maxPerSite = int.Parse(coreContext.Settings.Single(x => x.SettingKey == "crawlers_crawlpool_maxpersite").SettingValue.ToString());

                                                    while (getClaimedCount > maxPerSite)
                                                    {

                                                        Change_Pool_Record_Status(cpId, "OnHold");
                                                        Thread.Sleep(3000);
                                                        Change_Pool_Record_Status(cpId, "Working");

                                                        getClaimedCount = crawlerContext.CrawlPool
                                                        .Where(x => x.Status == "Working").Where(xc => xc.Url.Contains(checkContains)).Count();
                                                    }

                                                    HtmlWeb web = new HtmlWeb();

                                                    var htmlDoc = await web.LoadFromWebAsync(getPageUrl);

                                                    if (web.StatusCode == HttpStatusCode.TooManyRequests)
                                                    {
                                                        Thread.Sleep(30000);
                                                    }
                                                    if (web.StatusCode == HttpStatusCode.NotFound)
                                                    {
                                                        Remove_Not_Found_Pages(getNextPage.Url);
                                                    }

                                                    // Do pre checks

                                                    if (web.StatusCode == HttpStatusCode.OK && !getNextPage.Url.EndsWith(".rss") && !getNextPage.Url.EndsWith(".txt"))
                                                    {
                                                        if (htmlDoc.DocumentNode.SelectSingleNode("//body") != null)
                                                        /*if (htmlDoc.DocumentNode.SelectNodes("//a[@href]") != null)*/
                                                        {

                                                            int checkCrawledPages = crawlerContext.CrawledPages.Where(gcp => gcp.Url == getNextPage.Url).Count();
                                                            // if we dont contain a scanned page we need append it
                                                            if (checkCrawledPages <= 0)
                                                            {
                                                                Add_CrawledPage(getNextPage.Url, getWebsite.Id, getNextPage.IsPriority);
                                                            }
                                                            if (checkCrawledPages > 0)
                                                            {
                                                                var thePage = crawlerContext.CrawledPages.Where(gcp => gcp.Url == getNextPage.Url).Single();
                                                                Update_LastCrawled_Page(thePage.Id, getNextPage.IsPriority);
                                                            }

                                                            

                                                        }

                                                    }
                                                    UpdateCrawlSite_LastCrawled(getWebsite.Id);

                                                } // End Check Skip Domains
                                                
                                                // Now check if done remove from the pool and do counts
                                                RemovePage_From_CrawlPool(cpId);
                                                // Update counts
                                                Update_CrawlerStats_Counts();

                                            } // End if site is not paused
                                        }

                                        // Now do the actions if nothing hold for a second to not over run system
                                        Thread.Sleep(1000);
                                    }
                                }
                                catch (Exception e)
                                {

                                    if (e.Message != "Sequence contains no elements")
                                    {
                                        isRunning = false;
                                        Add_New_AppLog("Error", "Crawlers", e.Message.ToString());
                                        // if error and fails set page back to waiting if it has not already been removed
                                        // if has been removed and theres a fault remove crawler from pool
                                        if (Get_CrawlPool_Count() > 0)
                                        {
                                            Check_OnError_Page(cpId);
                                        }
                                    }
                                    if (e.Message == "Sequence contains no elements")
                                    {
                                        // Do somthing in case need to check if it removes crawler work without check may need to validate
                                    }
                                    if (e.Message == "SQLite Error 5: 'database is locked'.")
                                    {
                                        Thread.Sleep(10000);
                                    }

                                    // After Checks refresh crawler screen

                                }
                            }
                            if (getTask.Status.ToString() == "Stopped")
                            {
                                isRunning = false;
                                Finish_FinalChecks_Inpool();

                            }
                        }
                        catch (Exception e)
                        {

                            if (e.Message != "Sequence contains no elements")
                            {
                                isRunning = false;
                                Add_New_AppLog("Error", "Crawlers", e.Message.ToString());
                                // if error and fails set page back to waiting if it has not already been removed
                                // if has been removed and theres a fault remove crawler from pool
                                
                            }
                            if (e.Message == "Sequence contains no elements")
                            {
                                // Do somthing in case need to check if it removes crawler work without check may need to validate
                            }

                            if (e.Message == "SQLite Error 5: 'database is locked'.")
                            {
                                isRunning = true;
                                Thread.Sleep(10000);
                            }

                            // After Checks refresh crawler screen

                        }

                    }
                }
            });
        }

        public void Finish_FinalChecks_Inpool()
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var poolcount = crawlerContext.CrawlPool.Count();
                var waitingCount = crawlerContext.CrawlPool.Where(x => x.Status == "Waiting").Count();
                using (CountersDbContext counterContext = new CountersDbContext())
                {
                    var activeCrawlers = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "active_crawlers");
                    var waitingCrawlers = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "waiting_crawlers");
                    var diedCrawlers = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "died_crawlers");
                    var crawlersPool = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "crawlers_pool");
                    var crawlersPoolPriority = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "priority_crawlpool");
                    if (poolcount == waitingCount)
                    {
                        activeCrawlers.CounterValue = 0;
                        waitingCrawlers.CounterValue = 0;
                        diedCrawlers.CounterValue = 0;
                        counterContext.SaveChanges();

                    }
                    if (poolcount == 0)
                    {
                        activeCrawlers.CounterValue = 0;
                        waitingCrawlers.CounterValue = 0;
                        diedCrawlers.CounterValue = 0;
                        crawlersPool.CounterValue = 0;
                        crawlersPoolPriority.CounterValue = 0;
                        counterContext.SaveChanges();
                    }
                }
            }
        }

        public static int Get_Crawlers_Total()
        {
            using (CountersDbContext counterContext = new CountersDbContext())
            {
                int ActiveCrawlers = counterContext.CrawlerCounters.Single(c => c.CounterKey == "active_crawlers").CounterValue;
                int WaitingCrawlers = counterContext.CrawlerCounters.Single(c => c.CounterKey == "waiting_crawlers").CounterValue;
                int TotalCrawlers = ActiveCrawlers + WaitingCrawlers;
                return TotalCrawlers;
            }
        }


        public void Add_New_AppLog(string logType, string logSection, string logMessage)
        {
            using (WebcoseoDbContext context = new WebcoseoDbContext())
            {
                context.AppLogs.Add(new Models.AppLogs
                {
                    Created = DateTime.Now,
                    Type = logType,
                    Section = logSection,
                    Message = logMessage,

                });
                context.SaveChanges();
            }
        }


        public void Check_OnError_Page(int cpId)
        {
            using (CrawlersDbContext crawlerContextError = new CrawlersDbContext())
            {
                var findErrPage = crawlerContextError.CrawlPool.Where(x => x.Id == cpId).Count();
                if (findErrPage > 0)
                {
                    Change_Pool_Record_Status(cpId, "Waiting");
                    isRunning = true;
                }
                else
                {
                    Change_Pool_Record_Status(cpId, "Error");
                }
            }
        }

        public void UpdateCrawlSite_LastCrawled(int websiteId)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var getSite = crawlerContext.CrawlSites.Single(w => w.Id == websiteId);
                if (getSite.Status != "Paused")
                {
                    getSite.Status = "Crawling";
                }
                getSite.LastCrawled = DateTime.Now;
                crawlerContext.SaveChanges();
            }
        }

        public void Change_Pool_Record_Status(int poolId, string CrawlStatus)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var poolRecord = crawlerContext.CrawlPool.Single(pr => pr.Id == poolId);
                poolRecord.Status = CrawlStatus;
                crawlerContext.SaveChanges();
            }
        }

        public static void Update_Crawlers_Counts()
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                int getActiveTotal = crawlerContext.CrawlPool.Count(x => x.Status == "Working");
                int getWaitingTotal = crawlerContext.CrawlPool.Count(x => x.Status == "OnHold");
                int getDiedTotal = crawlerContext.CrawlPool.Count(x => x.Status == "Error");
                int getPoolTotal = crawlerContext.CrawlPool.Count();
                int getPoolTotalPriority = crawlerContext.CrawlPool.Count(x => x.IsPriority == true);
                int getCrawledPagesPriority = crawlerContext.CrawledPages.Count(x => x.IsPriority == true);
                using (CountersDbContext counterContext = new CountersDbContext())
                {
                    var activeCrawlers = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "active_crawlers");
                    var waitingCrawlers = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "waiting_crawlers");
                    var diedCrawlers = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "died_crawlers");
                    var crawlersPool = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "crawlers_pool");
                    var crawlersPoolPriority = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "priority_crawlpool");
                    var crawledPagesPriority = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "priority_crawledpages");
                    activeCrawlers.CounterValue = getActiveTotal;
                    waitingCrawlers.CounterValue = getWaitingTotal;
                    diedCrawlers.CounterValue = getDiedTotal;
                    crawlersPool.CounterValue = getPoolTotal;
                    crawlersPoolPriority.CounterValue = getPoolTotalPriority;
                    crawledPagesPriority.CounterValue = getCrawledPagesPriority;
                    counterContext.SaveChanges();

                }
            }
        }


        public static int Get_CrawlPool_Count()
        {
            Update_Crawlers_Counts();
            using (CountersDbContext counterContext = new CountersDbContext())
            {
                int poolCount = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawlers_pool").CounterValue;
                return poolCount;
            }
        }


        public void Update_CrawlerStats_Counts()
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                using (CountersDbContext counterContext = new CountersDbContext())
                {
                    var CrawledPages = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawled_pages");
                    var NewSites = counterContext.CrawlerCounters.Single(c => c.CounterKey == "new_sites");
                    var CrawlPool = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawlers_pool");
                    var ApprovedSites = counterContext.CrawlerCounters.Single(c => c.CounterKey == "approved_sites");

                    CrawledPages.CounterValue = crawlerContext.CrawledPages.Count();
                    CrawlPool.CounterValue = crawlerContext.CrawlPool.Count();
                    NewSites.CounterValue = crawlerContext.NewSites.Count();
                    ApprovedSites.CounterValue = crawlerContext.CrawlSites.Count();
                    counterContext.SaveChanges();
                }
            }
        }

        public void Remove_Not_Found_Pages(string nextPageUrl)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var getNotFound = crawlerContext.CrawledPages.Where(gcp => gcp.Url == nextPageUrl);
                foreach (var nfPage in getNotFound)
                {
                    crawlerContext.CrawledPages.Remove(nfPage);
                }
                crawlerContext.SaveChanges();
            }
        }


        public void RemovePage_From_CrawlPool(int pageId)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var getCurrentPage = crawlerContext.CrawlPool.Single(x => x.Id == pageId);
                crawlerContext.CrawlPool.Remove(getCurrentPage);

                // Save Crawler info
                crawlerContext.SaveChanges();
            }
        }


        public void Add_CrawledPage(string hrefValue, int crawlSiteId, bool isPriority)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                crawlerContext.CrawledPages.Add(new Models.Crawlers.CrawledPages
                {
                    Url = hrefValue,
                    CrawlSitesId = crawlSiteId,
                    Status = "Waiting",
                    Created = DateTime.Now,
                    LastCrawled = DateTime.Now,
                    // save here in case adding html again in future
                    //HtmlLastChecked = DateTime.Now.AddYears(-100),
                    IsPriority = isPriority,
                    // save here in case adding html in future
                    //Html = getHtml
                });
                crawlerContext.SaveChanges();

            }
        }


        public void Update_LastCrawled_Page(int pageId, bool isPriority)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var getCrawledPage = crawlerContext.CrawledPages.Single(gcp => gcp.Id == pageId);
                getCrawledPage.LastCrawled = DateTime.Now;
                getCrawledPage.Status = "Waiting";
                // save here incase adding html again in future
                //getCrawledPage.Html = getHtml;
                getCrawledPage.IsPriority = isPriority;
                crawlerContext.SaveChanges();
            }
        }


    }
}
