using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;

namespace WebCoseo.Views.Analysis.Details
{
    /// <summary>
    /// Interaction logic for FoundSitesBrowser.xaml
    /// </summary>
    public partial class FoundSitesBrowser : UserControl
    {
        public static FoundSitesBrowser _instance;

        public int foundSiteId; 
        public FoundSitesBrowser()
        {
            _instance = this;
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            //InitializeAsync();
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

        public void Remove_FromCrawler_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                
                if (MessageBox.Show("Confirm Removal of found site this may appear again when doing web crawling and analysis to prevent appearing use Not Used Button?",
                   "Confirm Removal of Site",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getFoundSite = crawlersContext.NewSites.Single(x => x.Id == foundSiteId);
                    crawlersContext.NewSites.Remove(getFoundSite);
                    crawlersContext.SaveChanges();
                    ViewDetails._instance.GetView_FoundSites();
                }
                else
                {
                    return;
                }
            }

        }

        public void NotUsed_FromCrawler_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                
                if (MessageBox.Show("Confirm that you do not wish to use this website in your websites, crawler and analysis?",
                   "Confirm not using Site",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getFoundSite = crawlersContext.NewSites.Single(x => x.Id == foundSiteId);

                    getFoundSite.Status = "NotUsed";
                    crawlersContext.SaveChanges();
                    ViewDetails._instance.GetView_FoundSites();
                }
                else
                {
                    return;
                }
            }

        }

        public void Import_FromCrawler_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
               
                if (MessageBox.Show("Confirm that you wish to import this website to your websites, future crawling and analysis?",
                   "Confirm importing of site",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    using (AppServices.WebcoseoDbContext context = new())
                    {

                        var getFoundSite = crawlersContext.NewSites.Single(x => x.Id == foundSiteId);
                        var Domain = getFoundSite.Url.Replace("https://", "");

                        context.Websites.Add(new Models.Websites
                        {
                            Name = Domain,
                            Url = getFoundSite.Url,
                            Description = Domain + " has been imported from the analysis found sites",
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        });
                        context.SaveChanges();

                        getFoundSite.Status = "Imported";
                        crawlersContext.SaveChanges();
                        ViewDetails._instance.GetView_FoundSites();
                    }
                }
                else
                {
                    return;
                }
            }

        }

    }
}
