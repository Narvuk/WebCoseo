using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WebCoseo.Views.Websites.Details
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        public static Overview _instance;
        public int GetWebsiteId { set; get; }
        public Overview()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int WebsiteId)
        {
            this.GetWebsiteId = WebsiteId;
            Update_Overview();
        }

        public void Update_Overview()
        {
            using AppServices.CoreDbContext coreContext = new();
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getWebSite = context.Websites.Single(x => x.Id == GetWebsiteId);
                var convertImg = getWebSite.Url.Replace("https://", "");
                var apiPagePeeker = coreContext.Settings.Single(x => x.SettingKey == "api_pagepeeker");
                if(apiPagePeeker.SettingValue == "Free")
                {
                    Uri thumb_load = new Uri($"http://free.pagepeeker.com/v2/thumbs.php?size=x&url={convertImg}", UriKind.RelativeOrAbsolute);
                    ImageSource imageSource = new BitmapImage(thumb_load);
                    Overview_Load_Thumnail.Source = imageSource;
                }
                if (apiPagePeeker.SettingValue != "" && apiPagePeeker.SettingValue != "Free")
                {
                    Uri thumb_load = new Uri($"http://api.pagepeeker.com/v2/thumbs.php?size=x&code={apiPagePeeker.SettingValue}&url={convertImg}", UriKind.RelativeOrAbsolute);
                    ImageSource imageSource = new BitmapImage(thumb_load);
                    Overview_Load_Thumnail.Source = imageSource;
                }
                
                Website_Description.Text = getWebSite.Description;

                var DateTimeNow = DateTime.Now;

                BacklinksTotal_Count.Text = context.Backlinks.Count(x => x.WebsiteId == GetWebsiteId).ToString();
                BacklinksActiveTotal_Count.Text = context.Backlinks.Count(x => x.WebsiteId == GetWebsiteId && x.Status == "Active").ToString();
                BacklinksLostTotal_Count.Text = context.Backlinks.Count(x => x.WebsiteId == GetWebsiteId && x.Status == "Lost").ToString();
                BacklinksNewTotal_Count.Text = context.Backlinks.Count(x => x.WebsiteId == GetWebsiteId && x.Status == "New").ToString();

                if (context.Backlinks.Count(x => x.WebsiteId == GetWebsiteId) > 0)
                {
                    string latestBacklink = context.Backlinks.Where(x => x.WebsiteId == GetWebsiteId).OrderByDescending(x => x.Created).First().BacklinkUrl.ToString();
                    LatestBacklink_Data.Text = latestBacklink;
                    ViewBacklink_Button.Visibility = Visibility.Visible;
                }
                else
                {
                    LatestBacklink_Data.Text = "No Latest Backlink - At this moment in time.";
                    ViewBacklink_Button.Visibility = Visibility.Collapsed;
                }


                // Check Buttons
                using (AppServices.CrawlersDbContext CrawlersContext = new AppServices.CrawlersDbContext())
                {
                    if (CrawlersContext.CrawlSites.Where(surl => surl.Url == getWebSite.Url).Count() > 0)
                    {
                        cralwers_addto_sites.Visibility = Visibility.Collapsed;
                        crawlers_addedto_sites.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        cralwers_addto_sites.Visibility = Visibility.Visible;
                        crawlers_addedto_sites.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }


        public void ViewBacklink_Click(object sender, RoutedEventArgs e)
        {
            using AppServices.WebcoseoDbContext context = new();
            var latestBacklink = context.Backlinks.Where(x => x.WebsiteId == GetWebsiteId).OrderByDescending(x => x.Created).First();
            BacklinkView._instance.LoadDetails(latestBacklink.Id);
            ViewDetails._instance.GetView_ViewBacklink();
        }

        public void Add_Site_ToCrawlers_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getWebSite = context.Websites.Single(x => x.Id == GetWebsiteId);

                using (AppServices.CrawlersDbContext CrawlersContext = new AppServices.CrawlersDbContext())
                {
                    CrawlersContext.CrawlSites.Add(new Models.Crawlers.CrawlSites
                    {
                        Url = getWebSite.Url,
                        Status = "New",
                        Created = DateTime.Now
                    });
                    CrawlersContext.SaveChanges();

                    // Add to crawlpool list
                    var findCrawlSite = CrawlersContext.CrawlSites.Where(x => x.Url == getWebSite.Url).First();
                    CrawlersContext.CrawlPool.Add(new Models.Crawlers.CrawlPool
                    {
                        CrawlSitesId = findCrawlSite.Id,
                        Url = getWebSite.Url,
                        Status = "Waiting",
                        Created = DateTime.Now
                    });
                    CrawlersContext.SaveChanges();

                }
                getWebSite.Updated = DateTime.Now;
                context.SaveChanges();
            }
            Update_Crawlers_Counts();           
            Update_Overview();
        }

        public void Update_Crawlers_Counts()
        {
            using (AppServices.CrawlersDbContext crawlerContext = new())
            {
                int getPoolTotal = crawlerContext.CrawlPool.Count();
                int getPoolTotalPriority = crawlerContext.CrawlPool.Count(x => x.IsPriority == true);
                int getCrawlSitesTotal = crawlerContext.CrawlSites.Count();
                using (AppServices.CountersDbContext counterContext = new())
                {
                    var approvedSites = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "approved_sites");
                    var crawlersPool = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "crawlers_pool");
                    var crawlersPoolPriority = counterContext.CrawlerCounters.Single(ac => ac.CounterKey == "priority_crawlpool");
                    approvedSites.CounterValue = getCrawlSitesTotal;
                    crawlersPool.CounterValue = getPoolTotal;
                    crawlersPoolPriority.CounterValue = getPoolTotalPriority;
                    counterContext.SaveChanges();

                }
            }
        }
    }
}
