using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Analysis.Details
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        public static Overview _instance;
        public Overview()
        {
            _instance = this;
            InitializeComponent();

        }

        public void Update_Analysis_Overview()
        {
            using AppServices.CountersDbContext countersContext = new();
            using AppServices.CrawlersDbContext crawlersContext = new();
            using AppServices.WebcoseoDbContext webcoseoContext = new();

            var getService = webcoseoContext.CronTasks.Where(ct => ct.CronKey == "analysis_htmlchecker").Single();
            if (getService.Status == "Stopped")
            {
                analysis_htmlcheck_stop.Visibility = Visibility.Collapsed;
                analysis_htmlcheck_start.Visibility = Visibility.Visible;
            }
            if (getService.Status == "Running")
            {
                analysis_htmlcheck_start.Visibility = Visibility.Collapsed;
                analysis_htmlcheck_stop.Visibility = Visibility.Visible;
            }

            try
            {
                ApprovedSites_Count.Text = countersContext.CrawlerCounters.Single(c => c.CounterKey == "approved_sites").CounterValue.ToString();
                FoundSites_Count.Text = countersContext.CrawlerCounters.Single(c => c.CounterKey == "new_sites").CounterValue.ToString();
                WaitingPriority_Pages_Count.Text = countersContext.CrawlerCounters.Single(c => c.CounterKey == "priority_crawledpages").CounterValue.ToString();
            }
            catch (Exception)
            {
                ApprovedSites_Count.Text = "0";
                FoundSites_Count.Text = "0";
                WaitingPriority_Pages_Count.Text = "0";
            }

            
            Waiting_Scan_Count.Text = crawlersContext.CrawledPages.Count(x => x.Status == "Waiting").ToString();
            Scanned_Pages_Count.Text = crawlersContext.CrawledPages.Count(x => x.Status == "Completed").ToString();
            Excluded_Pages_Count.Text = crawlersContext.CrawledPages.Count(x => x.Status == "Excluded").ToString();

            

        }


        public void Start_Analysis_Html_Check(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getService = context.CronTasks.Where(ct => ct.CronKey == "analysis_htmlchecker").Single();
                if (getService.Status != "Running")
                {
                    var startService = new Helpers.CronTasks.CronTaskStart();
                    startService.Run(getService.Id);
                    analysis_htmlcheck_start.Visibility = Visibility.Collapsed;
                    analysis_htmlcheck_stop.Visibility = Visibility.Visible;
                }
            }
        }

        public void Stop_Analysis_Html_Check(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getService = context.CronTasks.Where(ct => ct.CronKey == "analysis_htmlchecker").Single();
                if (getService.Status != "Stopped")
                {
                    var stopService = new Helpers.CronTasks.CronTaskStop();
                    stopService.Run(getService.Id);
                    analysis_htmlcheck_stop.Visibility = Visibility.Collapsed;
                    analysis_htmlcheck_start.Visibility = Visibility.Visible;
                }
            }
        }

        public void Live_Data_WindowView_Click(object sender, RoutedEventArgs e)
        {
            var liveDataWindow = new Crawlers.Windows.CrawlersLiveData();
            liveDataWindow.Show();
        }

    }
}
