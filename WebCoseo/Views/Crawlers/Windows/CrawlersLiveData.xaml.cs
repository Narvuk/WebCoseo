using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace WebCoseo.Views.Crawlers.Windows
{
    /// <summary>
    /// Interaction logic for CrawlersLiveData.xaml
    /// </summary>
    public partial class CrawlersLiveData : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        bool IsDashRefresh;
        public CrawlersLiveData()
        {
            Loaded += ToolWindow_Loaded;
            InitializeComponent();
            Dash_Refresh();
        }

        void ToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        public async void Dash_Refresh()
        {
            IsDashRefresh = true;
            while (IsDashRefresh)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Update_Crawler_DashWindow();
                    });
                });
            }
        }

        public void Update_Crawler_DashWindow()
        {
            using (AppServices.CountersDbContext counterContext = new AppServices.CountersDbContext())
            {
                using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
                {
                    try
                    {
                        //
                        // Create own window option to view live stats
                        ActiveCrawlers_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "active_crawlers").CounterValue.ToString();
                        WaitingCrawlers_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "waiting_crawlers").CounterValue.ToString();
                        DiedCrawlers_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "died_crawlers").CounterValue.ToString();
                        CrawlerPool_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawlers_pool").CounterValue.ToString();
                        SitesFound_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "new_sites").CounterValue.ToString();
                        Scanners_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "active_scanners").CounterValue.ToString();
                        WaitingScanners_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "waiting_scanners").CounterValue.ToString();
                        DiedScanners_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "died_scanners").CounterValue.ToString();
                        Priority_WaitingScan_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "priority_crawledpages").CounterValue.ToString();
                        Priority_CrawlerPool_Count.Content = counterContext.CrawlerCounters.Single(c => c.CounterKey == "priority_crawlpool").CounterValue.ToString();
                        WaitingScan_Count.Content = crawlersContext.CrawledPages.Count(x => x.Status == "Waiting").ToString();
                        ScannedPages_Count.Content = crawlersContext.CrawledPages.Count(x => x.Status == "Completed").ToString();

                    }
                    catch (Exception)
                    {
                        CrawlerPool_Count.Content = "0";
                        ActiveCrawlers_Count.Content = "0";
                        WaitingCrawlers_Count.Content = "0";
                        DiedCrawlers_Count.Content = "0";
                        SitesFound_Count.Content = "0";
                        WaitingScan_Count.Content = "0";
                        ScannedPages_Count.Content = "0";
                        Scanners_Count.Content = "0";
                        WaitingScanners_Count.Content = "0";
                        DiedScanners_Count.Content = "0";
                        Priority_WaitingScan_Count.Content = "0";
                        Priority_CrawlerPool_Count.Content = "0";
                    }

                    /*
                    Crawlers_Working_List.ItemsSource = crawlersContext.CrawlPool.Where(x => x.Status == "Working").ToList();
                    Crawlers_Waiting_List.ItemsSource = crawlersContext.CrawlPool.Where(x => x.Status == "OnHold").ToList();
                    */

                }
            }
        }


        public void Stats_Click(object sender, RoutedEventArgs e)
        {
            Info_Panel.Visibility = Visibility.Collapsed;
            Stats_Panel.Visibility = Visibility.Visible;
        }
        public void Info_Click(object sender, RoutedEventArgs e)
        {
            Info_Panel.Visibility = Visibility.Visible;
            Stats_Panel.Visibility = Visibility.Collapsed;
        }

        public async void Close_Window_Click(object sender, RoutedEventArgs e)
        {
            Info_Panel.Visibility = Visibility.Collapsed;
            Stats_Panel.Visibility = Visibility.Collapsed;
            Closing_Panel.Visibility = Visibility.Visible;
            await Task.Run(() =>
            {
                IsDashRefresh = false;
                Thread.Sleep(5000);
            });
            this.Close();
        }

    }
}
