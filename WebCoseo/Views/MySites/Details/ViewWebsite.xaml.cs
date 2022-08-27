using Microsoft.Web.WebView2.Core;
using System;
using System.Linq;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.MySites.Details
{
    /// <summary>
    /// Interaction logic for ViewWebsite.xaml
    /// </summary>
    public partial class ViewWebsite : UserControl
    {
        public static ViewWebsite _instance;
        public int GetMySiteId { set; get; }
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

        public void LoadDetails(int MySiteId)
        {
            GetMySiteId = MySiteId;
            using AppServices.WebcoseoDbContext context = new();
            GetWebsiteUrl = context.MySites.Single(x => x.Id == GetMySiteId).Url;
        }

        public async void InitializeAsync(string url)
        {
            await MySitesBrowser.EnsureCoreWebView2Async(null);
            MySitesBrowser.CoreWebView2.Navigate(url);
            MySitesBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
            MySitesBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            await MySitesBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            MySitesBrowser.NavigationCompleted += Websites_NavigationCompleted;
            // Fail Safe to ensure that the initial onload url goes into url bar
            SiteUrl_Box.Text = url;


        }

        private void Websites_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                MySitesBrowser.WebMessageReceived += UpdateAddressBar;
            }
        }

        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            SiteUrl_Box.Text = uri;
            MySitesBrowser.CoreWebView2.PostWebMessageAsString(uri);
        }


        public async void WebsitesBrowser_LoadPage(string url)
        {

            //ViewDetails._instance.Change_TitleText_TopBar("Websites - Loading Website....");
            if (MySitesBrowser != null && MySitesBrowser.CoreWebView2 != null)
            {
                MySitesBrowser.CoreWebView2.Navigate(url);
                await MySitesBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
                MySitesBrowser.NavigationCompleted += Websites_NavigationCompleted;

            }
            else
            {
                InitializeAsync(url);
            }

        }

        public void Website_Add_SitePage_Click(object sender, RoutedEventArgs e)
        {
            using AppServices.WebcoseoDbContext context = new();
            if (context.MySitesPages.Any(w => w.Url == SiteUrl_Box.Text))
            {
                MessageBox.Show("Page already exists in your list of Site Pages");
            }
            else
            {
                MySites_Browser_View.Visibility = Visibility.Collapsed;
                AddSitePage_View.Visibility = Visibility.Visible;
                Views.MySites.Actions.AddSitePage._instance.getMySitePageUrl(SiteUrl_Box.Text);
            }
            
        }

        public void MySite_Home_Click(object sender, RoutedEventArgs e)
        {
            WebsitesBrowser_LoadPage(GetWebsiteUrl);
        }

    }
}
