using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Services.Store;

namespace WebCoseo.AppServices.CronTasks
{
    public class AnalysisHtmlCheck
    {
        public static AnalysisHtmlCheck? _instance;

        bool IsRunning { get; set; }
        int CrawlSitesId { get; set; }

        int CpId { get; set; }

        public AnalysisHtmlCheck()
        {
            _instance = this;
        }
        public async void MainTask(int cronid)
        {
            IsRunning = true;
            //MainWindow._instance.UpdateWindowStatusLabel("");
            //Views.CronTasks.CronTasksMain._instance.CT_Update_TelegramBot_Status("Running");
            await Task.Run(async() =>
            {
                while (IsRunning == true)
                {
                    CpId = 0;
                    using CoreDbContext coreContext = new();
                    using (WebcoseoDbContext context = new WebcoseoDbContext())
                    {

                        var getTask = context.CronTasks.Single(ct => ct.Id == cronid);
                        if (getTask.Status.ToString() == "Running" && Get_CrawledPages_Count() > 0)
                        {
                            try
                            {
                                using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
                                {
                                    int poolToSkip = 0;
                                    
                                    var dtnow = DateTime.Now;
                                    var getCrawledPagesCountPriority = crawlerContext.CrawledPages.Count(x => x.Status == "Waiting" && x.IsPriority == true);
                                    var getCrawledPagesCount = crawlerContext.CrawledPages.Count(x => x.Status == "Waiting");

                                    Random rand = new Random();
                                    if ( getCrawledPagesCountPriority > 0 )
                                    {
                                        poolToSkip = rand.Next(0, getCrawledPagesCountPriority);
                                    }
                                    else
                                    {
                                        poolToSkip = rand.Next(0, getCrawledPagesCount);
                                    }
                                    
                                    if (getCrawledPagesCount > 0)
                                    {
                                        var getNextPage = crawlerContext.CrawledPages.Where(gnp => gnp.Status == "Waiting").OrderBy(gp => gp.Created).Skip(poolToSkip).First();
                                        if (getCrawledPagesCountPriority > 0)
                                        {
                                            getNextPage = crawlerContext.CrawledPages.Where(gnp => gnp.Status == "Waiting" && gnp.IsPriority == true).OrderBy(gp => gp.Created).Skip(poolToSkip).First();
                                        }
                                        
                                        CpId = getNextPage.Id;
                                        // We have now claimed the page made sure another scanner does not take it
                                        Change_Page_Record_Status(CpId, "Working");

                                        var getWebsite = crawlerContext.CrawlSites.Single(gs => gs.Id == getNextPage.CrawlSitesId);
                                        CrawlSitesId = getWebsite.Id;                                     

                                        if (getWebsite.Status == "Paused")
                                        {
                                            Change_Page_Record_Status(CpId, "Paused");
                                        }

                                        if (getWebsite.Status != "Paused")
                                        {
                                            // Format internal domain Sites
                                            var checkContains = getWebsite.Url.Replace("https://", "");

                                            // if statement for max per site here
                                            int getClaimedCount = crawlerContext.CrawledPages
                                                    .Where(x => x.Status == "Working").Where(xc => xc.Url.Contains(checkContains)).Count();

                                            int maxPerSite = int.Parse(coreContext.Settings.Single(x => x.SettingKey == "analysis_maxscanners_persite").SettingValue.ToString());

                                            while (getClaimedCount > maxPerSite)
                                            {

                                                Change_Page_Record_Status(CpId, "OnHold");
                                                Thread.Sleep(3000);
                                                Change_Page_Record_Status(CpId, "Working");

                                                getClaimedCount = crawlerContext.CrawledPages
                                                .Where(x => x.Status == "Working").Where(xc => xc.Url.Contains(checkContains)).Count();
                                            }

                                            //var htmlDoc = new HtmlDocument();
                                            HtmlWeb web = new HtmlWeb();

                                            var htmlDoc = await web.LoadFromWebAsync(getNextPage.Url);

                                            if (web.StatusCode == HttpStatusCode.TooManyRequests)
                                            {
                                                Thread.Sleep(30000);
                                            }
                                            if (web.StatusCode == HttpStatusCode.NotFound)
                                            {
                                                Remove_Not_Found_Pages(getNextPage.Url);
                                            }

                                            //htmlDoc.LoadHtml(getNextPage.Html);

                                            // cpu saver loading doesnt take too long
                                            //Thread.Sleep(5000);
                                            if (web.StatusCode == HttpStatusCode.OK)
                                            {
                                                if (htmlDoc.DocumentNode.SelectNodes("//a[@href]") != null)
                                                {
                                                    foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                                                    {
                                                        if (link != null)
                                                        {
                                                            string hrefValue = link.GetAttributeValue("href", string.Empty);
                                                            var getNextPageDone = getNextPage.Url;
                                                            if (getNextPage.Url.EndsWith("/"))
                                                            {
                                                                getNextPageDone = getNextPageDone.Remove(getNextPageDone.Length - 1, 1);
                                                            }
                                                            if (hrefValue.StartsWith("/"))
                                                            {
                                                                // Need to get root domain
                                                                hrefValue = "https://" + new Uri(getNextPageDone).Host + hrefValue;
                                                            }
                                                            /*
                                                            if (hrefValue.StartsWith("#"))
                                                            {
                                                                // We dont need # values just main urls try to work somthing on next
                                                                hrefValue = getNextPageDone;
                                                            }
                                                            if (hrefValue.StartsWith("&"))
                                                            {
                                                                hrefValue = getNextPageDone + '/' + hrefValue;
                                                            }
                                                            if (hrefValue.StartsWith("?"))
                                                            {
                                                                hrefValue = getNextPageDone + '/' + hrefValue;
                                                            }
                                                            if (hrefValue.StartsWith("="))
                                                            {
                                                                hrefValue = getNextPageDone + '/' + hrefValue;
                                                            }*/

                                                            // Check For Backlinks against pages thats saved in my sites pages
                                                            int FindMySitesPages = context.MySitesPages.Where(fp => fp.Url == hrefValue).Count();
                                                            if (FindMySitesPages > 0)
                                                            {
                                                                CheckAdd_New_Backlink(hrefValue, getNextPage.Url, getWebsite.Url);
                                                            }

                                                            // Format for found links
                                                            var checkExternalContains = hrefValue.Replace("https://", "");

                                                            // not to self let crawler handle this check
                                                            /*
                                                            var getHostUrl = new Uri(getWebsite.Url).Host.ToString();
                                                            var checkSkipDomains = crawlerContext.SkipCrawl.Where(d => d.Domain.Contains(getHostUrl)).Count();
                                                            int checkSkipContainsCount = 0;
                                                            var getSkipContainsList = crawlerContext.SkipContains.OrderBy(x => x.Id);

                                                            if (getSkipContainsList != null)
                                                            {
                                                                foreach (var sc in getSkipContainsList)
                                                                {
                                                                    if (hrefValue.Contains(sc.Phrase))
                                                                    {
                                                                        checkSkipContainsCount++;
                                                                    }
                                                                }
                                                            }*/

                                                            // not to self let crawler handle this check end

                                                            int checkCrawledPages = crawlerContext.CrawledPages.Where(gcp => gcp.Url == hrefValue).Count();
                                                            // if we dont contain a scanned page we need append it
                                                            if (checkCrawledPages <= 0 /* && checkSkipDomains <= 0 && checkSkipContainsCount <= 0*/)
                                                            {
                                                                // remove the https for the check

                                                                if (hrefValue.Contains(checkContains) && hrefValue.StartsWith("https://"))
                                                                {
                                                                    Addto_CrawlPool(getWebsite.Id, hrefValue);
                                                                }
                                                                if (!hrefValue.Contains(checkContains) && hrefValue.StartsWith("https://"))
                                                                {
                                                                    Uri newUri = new(hrefValue);
                                                                    string fixedUri = newUri.Host;
                                                                    string newDomain = "https://" + fixedUri;

                                                                    // Check if in new sites already
                                                                    var checkNewSite = crawlerContext.NewSites.Where(gcp => gcp.Url == newDomain).Count();
                                                                    if (checkNewSite <= 0)
                                                                    {
                                                                        Addto_NewSites(newDomain);
                                                                    }
                                                                }
                                                            }
                                                            // page exists we do not need to save we just need to update last checked
                                                            if (checkCrawledPages == 1 /*&& checkSkipDomains <= 0 && checkSkipContainsCount <= 0*/)
                                                            {
                                                                var getCrawledPage = crawlerContext.CrawledPages.Single(gcp => gcp.Url == hrefValue);

                                                                if (hrefValue.Contains(checkContains))
                                                                {
                                                                    UpdateHtmlCheck_LastChecked(getCrawledPage.Id);
                                                                }
                                                                if (!hrefValue.Contains(checkContains) && hrefValue.StartsWith("https://"))
                                                                {
                                                                    Uri newUri = new(hrefValue);
                                                                    string fixedUri = newUri.Host;
                                                                    string newDomain = "https://" + fixedUri;

                                                                    var checkNewSite = crawlerContext.NewSites.Where(gcp => gcp.Url == newDomain).Count();
                                                                    if (checkNewSite <= 0)
                                                                    {
                                                                        Addto_NewSites(newDomain);
                                                                    }
                                                                }
                                                            }
                                                            if (checkCrawledPages > 1)
                                                            {
                                                                // hould never have to reach this point
                                                                CrawledPages_Duplicate_Check();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            
                                            // Garbage collector unsure as read it can be unstable at times
                                            // try another meathod unless last resort
                                            //GC.Collect();
                                            Change_Page_Record_Status(CpId, "Completed");
                                            Update_CrawlerStats_Counts();
                                        }
                                    } // End if pages count > 0


                                } // End DB crawler DB context




                                // Do action for all call and do check for latest call then search new

                                // 15 Minutes = 900000
                                // 10 Minutes = 600000
                                // 5 Minutes = 300000
                                Thread.Sleep(1000);
                            }
                            
                            catch (Exception e)
                            {

                                if (e.Message != "Sequence contains no elements")
                                {
                                    IsRunning = false;
                                    Add_New_AppLog("Error", "Analysis", e.Message.ToString());

                                    if (Get_CrawledPages_Count() > 0)
                                    {
                                        Check_OnError_Page(CpId);
                                    }

                                }
                                if (e.Message == "Sequence contains no elements")
                                {
                                    // Do somthing why it contains more than 1 element in the sequence
                                }

                                if (e.Message == "SQLite Error 5: 'database is locked'.")
                                {
                                    IsRunning = true;
                                    Thread.Sleep(10000);
                                }
                                
                            }
                        }
                        if (getTask.Status.ToString() == "Stopped")
                        {
                            IsRunning = false;
                            Finish_FinalChecks();
                            /*using CountersDbContext countersContext = new();
                            var getActiveScanners = countersContext.CrawlerCounters.Single(cs => cs.CounterKey == "active_scanners");
                            if (getActiveScanners.CounterValue <= 0)
                            {
                                getActiveScanners.CounterValue = 0;
                                countersContext.SaveChanges();
                            }
                            else
                            {
                                getActiveScanners.CounterValue--;
                                countersContext.SaveChanges();
                            }*/
                            
                        }
                    }
                }
            });

        }


        public void Finish_FinalChecks()
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var pagesCount = crawlerContext.CrawledPages.Count();
                var waitingCount = crawlerContext.CrawledPages.Where(x => x.Status == "Waiting").Count();
                var completedCount = crawlerContext.CrawledPages.Where(x => x.Status == "Completed").Count();
                using (CountersDbContext counterContext = new CountersDbContext())
                {
                    var activeScanners = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "active_scanners");
                    var waitingScanners = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "waiting_scanners");
                    var diedScanners = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "died_scanners");
                    var crawledPages = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "crawled_pages");
                    var crawledPagesPriority = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "priority_crawledpages");
                    if (pagesCount == waitingCount + completedCount)
                    {
                        activeScanners.CounterValue = 0;
                        waitingScanners.CounterValue = 0;
                        diedScanners.CounterValue = 0;
                        counterContext.SaveChanges();

                    }
                    if (pagesCount == 0)
                    {
                        activeScanners.CounterValue = 0;
                        waitingScanners.CounterValue = 0;
                        diedScanners.CounterValue = 0;
                        crawledPages.CounterValue = 0;
                        crawledPagesPriority.CounterValue = 0;
                        counterContext.SaveChanges();
                    }
                }
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
                var findErrPage = crawlerContextError.CrawledPages.Where(x => x.Id == cpId).Count();
                if (findErrPage > 0)
                {
                    Change_Page_Record_Status(cpId, "Waiting");
                    IsRunning = true;
                }
                else
                {
                    // Double check as this may not be reached
                    Change_Page_Record_Status(cpId, "Error");
                }
            }
        }

        public static void Update_Scanner_Counts()
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                int getActiveTotal = crawlerContext.CrawledPages.Count(x => x.Status == "Working");
                int getWaitingTotal = crawlerContext.CrawledPages.Count(x => x.Status == "OnHold");
                int getDiedTotal = crawlerContext.CrawledPages.Count(x => x.Status == "Error");
                int getPoolTotal = crawlerContext.CrawlPool.Count();
                int getPagesTotal = crawlerContext.CrawledPages.Count();
                int getPoolTotalPriority = crawlerContext.CrawlPool.Count(x => x.IsPriority == true);
                int getCrawledPagesPriority = crawlerContext.CrawledPages.Count(x => x.IsPriority == true);
                using (CountersDbContext counterContext = new CountersDbContext())
                {
                    var activeScanners = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "active_scanners");
                    var waitingScanners = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "waiting_scanners");
                    var diedScanners = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "died_scanners");
                    var crawlersPool = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "crawlers_pool");
                    var crawledPages = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "crawled_pages");
                    var crawlersPoolPriority = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "priority_crawlpool");
                    var crawledPagesPriority = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "priority_crawledpages");
                    activeScanners.CounterValue = getActiveTotal;
                    waitingScanners.CounterValue = getWaitingTotal;
                    diedScanners.CounterValue = getDiedTotal;
                    crawlersPool.CounterValue = getPoolTotal;
                    crawledPages.CounterValue = getPagesTotal;
                    crawlersPoolPriority.CounterValue = getPoolTotalPriority;
                    crawledPagesPriority.CounterValue = getCrawledPagesPriority;
                    counterContext.SaveChanges();

                }
            }
        }

        public static int Get_CrawledPages_Count()
        {
            Update_Scanner_Counts();
            using (CountersDbContext counterContext = new CountersDbContext())
            {
                int pagesCount = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawled_pages").CounterValue;
                return pagesCount;
            }
        }

        public void UpdateHtmlCheck_LastChecked(int getcpId)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var getpage = crawlerContext.CrawledPages.Single(w => w.Id == getcpId);
                getpage.HtmlLastChecked = DateTime.Now;
                crawlerContext.SaveChanges();
            }
        }

        public void Change_Page_Record_Status(int getcpId, string CrawlStatus)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var cpRecord = crawlerContext.CrawledPages.Single(pr => pr.Id == getcpId);
                cpRecord.Status = CrawlStatus;
                if (CrawlStatus == "Completed")
                {
                    cpRecord.IsPriority = false;
                }
                crawlerContext.SaveChanges();
            }
        }

        public void CrawledPages_Duplicate_Check()
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var CrawledPagesDupCheck = crawlerContext.CrawledPages.AsEnumerable().GroupBy(x => x.Url).SelectMany(sk => sk.Skip(1)).ToList();
                if (CrawledPagesDupCheck.Count > 0)
                {
                    crawlerContext.CrawledPages.RemoveRange(CrawledPagesDupCheck);

                }
                crawlerContext.SaveChanges();
            }
        }

        public void CheckAdd_New_Backlink(string hrefValue, string getNextPageUrl, string getWebsiteUrl)
        {
            using CoreDbContext coreContext = new();
            using (WebcoseoDbContext context = new WebcoseoDbContext())
            {
                var getMySitePage = context.MySitesPages.Single(gp => gp.Url == hrefValue);
                var useWebsite = context.Websites.Single(gw => gw.Url == getWebsiteUrl);
                int nextCheckTimer = int.Parse(coreContext.Settings.Single(x => x.SettingKey == "backlinks_next_check").SettingValue);

                var checkBacklinks = context.Backlinks.Where(cb => cb.BacklinkUrl == getNextPageUrl).Count();
                if (checkBacklinks <= 0)
                {
                    context.Backlinks.Add(new Models.Backlinks
                    {
                        Status = "New",
                        BacklinkUrl = getNextPageUrl,
                        WebsiteId = useWebsite.Id,
                        MySiteId = int.Parse(getMySitePage.MySiteId.ToString()),
                        MySitePageId = getMySitePage.Id,
                        LastChecked = DateTime.Now,
                        NextCheck = DateTime.Now.AddHours(nextCheckTimer),
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                    });
                    context.SaveChanges();
                }
            }
        }


        public void Addto_CrawlPool(int websiteId, string hrefValue)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var findInPool = crawlerContext.CrawlPool.Count(x => x.Url == hrefValue);
                if (findInPool <= 0)
                {
                    crawlerContext.CrawlPool.Add(new Models.Crawlers.CrawlPool
                    {
                        CrawlSitesId = websiteId,
                        Url = hrefValue,
                        Status = "Waiting",
                        Created = DateTime.Now
                    });
                    crawlerContext.SaveChanges();
                }

            }
        }

        public void Addto_NewSites(string hrefValue)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                crawlerContext.NewSites.Add(new Models.Crawlers.NewSites
                {
                    Url = hrefValue,
                    Status = "New",
                    Created = DateTime.Now,
                });
                crawlerContext.SaveChanges();
            }
        }

        public void Update_LastCrawled_Page(int pageId, string getHtml)
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                var getCrawledPage = crawlerContext.CrawledPages.Single(gcp => gcp.Id == pageId);
                getCrawledPage.LastCrawled = DateTime.Now;
                // save here in case adding html again
                //getCrawledPage.Html = getHtml;
                crawlerContext.SaveChanges();
            }
        }

        public int Get_ApprovedSites_Count()
        {
            using (CountersDbContext counterContext = new CountersDbContext())
            {
                int sitesCount = counterContext.CrawlerCounters.Single(c => c.CounterKey == "approved_sites").CounterValue;
                return sitesCount;
            }
        }

        public int Get_NewSites_Count()
        {
            using (CountersDbContext counterContext = new CountersDbContext())
            {
                int newsitesCount = counterContext.CrawlerCounters.Single(c => c.CounterKey == "new_sites").CounterValue;
                return newsitesCount;
            }
        }

        public void Update_CrawlerStats_Counts()
        {
            using (CrawlersDbContext crawlerContext = new CrawlersDbContext())
            {
                using (CountersDbContext counterContext = new CountersDbContext())
                {
                    var CrawledPages = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawled_pages");
                    var CrawledPagesPriority = counterContext.CrawlerCounters.Single(c => c.CounterKey == "priority_crawledpages");
                    var NewSites = counterContext.CrawlerCounters.Single(c => c.CounterKey == "new_sites");
                    var CrawlPool = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawlers_pool");
                    var ApprovedSites = counterContext.CrawlerCounters.Single(c => c.CounterKey == "approved_sites");

                    CrawledPages.CounterValue = crawlerContext.CrawledPages.Count();
                    CrawledPagesPriority.CounterValue = crawlerContext.CrawledPages.Count(c => c.IsPriority == true && c.Status == "Waiting");
                    CrawlPool.CounterValue = crawlerContext.CrawlPool.Count();
                    NewSites.CounterValue = crawlerContext.NewSites.Count();
                    ApprovedSites.CounterValue = crawlerContext.CrawlSites.Count();
                    counterContext.SaveChanges();
                    //Update_Scanner_Counts();
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
    }
}
