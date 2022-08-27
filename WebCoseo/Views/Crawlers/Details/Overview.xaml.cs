using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WebCoseo.Helpers.Crawlers;

namespace WebCoseo.Views.Crawlers.Details
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        public static Overview? _instance;
        public Overview()
        {
            _instance = this;
            InitializeComponent();
            Update_Crawler_Dashboard();
        }

        public void Update_Crawler_Dashboard()
        {
            using AppServices.CountersDbContext counterContext = new();
            using AppServices.CrawlersDbContext crawlersContext = new();
            using AppServices.WebcoseoDbContext webcoseoContext = new();

            var getService = webcoseoContext.CronTasks.Where(ct => ct.CronKey == "crawler_crawlpool_service").Single();
            if (getService.Status == "Stopped")
            {
                cralwers_start.Visibility = Visibility.Visible;
                cralwers_stop.Visibility = Visibility.Collapsed;
            }
            if (getService.Status == "Running")
            {
                cralwers_start.Visibility = Visibility.Collapsed;
                cralwers_stop.Visibility = Visibility.Visible;
            }

            try
            {
                //
                /* Create own window option to view live stats
                ActiveCrawlers_Count.Text = counterContext.CrawlerCounters.Single(c => c.CounterKey == "active_crawlers").CounterValue.ToString();
                WaitingCrawlers_Count.Text = counterContext.CrawlerCounters.Single(c => c.CounterKey == "waiting_crawlers").CounterValue.ToString();
                DiedCrawlers_Count.Text = counterContext.CrawlerCounters.Single(c => c.CounterKey == "died_crawlers").CounterValue.ToString();
                */
                CrawlerPool_Count.Text = counterContext.CrawlerCounters.Single(c => c.CounterKey == "crawlers_pool").CounterValue.ToString();
                CrawlerPoolPriority_Count.Text = counterContext.CrawlerCounters.Single(c => c.CounterKey == "priority_crawlpool").CounterValue.ToString();
                
            }
            catch (Exception)
            {
                CrawlerPool_Count.Text = "0";
                CrawlerPoolPriority_Count.Text = "0";
            }

            /*
            Crawlers_Working_List.ItemsSource = crawlersContext.CrawlPool.Where(x => x.Status == "Working").ToList();
            Crawlers_Waiting_List.ItemsSource = crawlersContext.CrawlPool.Where(x => x.Status == "OnHold").ToList();
            */
 
        }

        public async void Start_Crawler_Service(object sender, RoutedEventArgs e)
        {
            cralwers_start.Visibility = Visibility.Collapsed;
            cralwers_stop.Visibility = Visibility.Visible;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getCrawlerService = context.CronTasks.Single(ct => ct.CronKey == "crawler_crawlpool_service");
                getCrawlerService.Status = "Running";
                context.SaveChanges();

                await Task.Run(() =>
                {
                    var startService = new CrawlerPoolStart();
                    startService.Run();
                });

            }

        }

        public void Stop_Crawler_Service(object sender, RoutedEventArgs e)
        {
            cralwers_start.Visibility = Visibility.Visible;
            cralwers_stop.Visibility = Visibility.Collapsed;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getCrawlerService = context.CronTasks.Single(ct => ct.CronKey == "crawler_crawlpool_service");
                getCrawlerService.Status = "Stopped";
                context.SaveChanges();
            }

        }

        public void Live_Data_WindowView_Click(object sender, RoutedEventArgs e)
        {
            var liveDataWindow = new Windows.CrawlersLiveData();
            liveDataWindow.Show();
        }
    }
}
