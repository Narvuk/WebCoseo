using Microsoft.Web.WebView2.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Websites.Details
{
    /// <summary>
    /// Interaction logic for ViewWebsite.xaml
    /// </summary>
    public partial class ViewWebsite : UserControl
    {
        public static ViewWebsite _instance;

        public int GetWebsiteId { set; get; }
        public string GetWebsiteUrl { set; get; }
        public ViewWebsite()
        {
            _instance = this;
            InitializeComponent();

        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            //InitializeAsync();
        }

        public void LoadDetails(int WebsiteId)
        {
            GetWebsiteId = WebsiteId;
            using AppServices.WebcoseoDbContext context = new();
            GetWebsiteUrl = context.Websites.Single(x => x.Id == WebsiteId).Url;
        }

        public async void InitializeAsync(string url)
        {
            await WebsitesBrowser.EnsureCoreWebView2Async(null);
            WebsitesBrowser.CoreWebView2.Navigate(url);
            WebsitesBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
            WebsitesBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            await WebsitesBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            WebsitesBrowser.NavigationCompleted += Websites_NavigationCompleted;
            // Fail Safe to ensure that the initial onload url goes into url bar
            SiteUrl_Box.Text = url;


        }

        private void Websites_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                WebsitesBrowser.WebMessageReceived += UpdateAddressBar;
            }
        }

        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            SiteUrl_Box.Text = uri;
            WebsitesBrowser.CoreWebView2.PostWebMessageAsString(uri);
        }


        public async void WebsitesBrowser_LoadPage(string url)
        {

            //ViewDetails._instance.Change_TitleText_TopBar("Websites - Loading Website....");
            if (WebsitesBrowser != null && WebsitesBrowser.CoreWebView2 != null)
            {
                WebsitesBrowser.CoreWebView2.Navigate(url);
                await WebsitesBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
                WebsitesBrowser.NavigationCompleted += Websites_NavigationCompleted;

            }
            else
            {
                InitializeAsync(url);
            }

        }

        public void Website_Add_Backlink_Click(object sender, RoutedEventArgs e)
        {
            var getToastWindow = new ToastWindow();
            // When data is large we cannot add manually need to add to crawl pool.
            int csId = 0;
            string blUrl = SiteUrl_Box.Text;
            using AppServices.CrawlersDbContext crawlersDbContext = new();
            var checkInSites = crawlersDbContext.CrawlSites.Count(x => x.Url == GetWebsiteUrl);
            if (checkInSites == 0)
            {
                crawlersDbContext.CrawlSites.Add(new Models.Crawlers.CrawlSites
                {
                    Url = GetWebsiteUrl,
                    Status = "New",
                    Created = DateTime.Now,
                });
                crawlersDbContext.SaveChanges();
                // Refresh Overview page to show active in crawlers
                Overview._instance.Update_Overview();
            }
            if (checkInSites > 0)
            {
                csId = crawlersDbContext.CrawlSites.Single(x => x.Url == GetWebsiteUrl).Id;
            }
            var checkInPool = crawlersDbContext.CrawlPool.Count(x => x.Url == blUrl);
            if (checkInPool == 0)
            {
                crawlersDbContext.Add(new Models.Crawlers.CrawlPool
                {
                    CrawlSitesId = csId,
                    Url = blUrl,
                    Status = "Waiting",
                    IsPriority = true,
                    Created = DateTime.Now,
                });
                crawlersDbContext.SaveChanges();
                getToastWindow.Send_Message("Added To Crawl Pool", "The url has been added to the crawl pool to check for backlinks and other data");
                getToastWindow.Show();
               
            }
            if (checkInPool > 0)
            {
                getToastWindow.Send_Message("Already In Crawl Pool", "The url is already in the crawl pool waiting to be crawled");
                getToastWindow.Show();
               
            }

            Overview._instance.Update_Crawlers_Counts();

            //Website_Browser_View.Visibility = Visibility.Collapsed;
            //AddBacklinks_View.Visibility = Visibility.Visible;

        }

        public void Website_Home_Click(object sender, RoutedEventArgs e)
        {
            WebsitesBrowser_LoadPage(GetWebsiteUrl);
        }

        public void Contact_Add_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You found a clue! We will be having the ability to add and use contact information via the software soon.", "Feature Coming Soon");
        }
    }
}
