using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WebCoseo.Views.Settings.Details
{
    /// <summary>
    /// Interaction logic for SystemSettings.xaml
    /// </summary>
    public partial class SystemSettings : UserControl
    {
        public static SystemSettings? _instance;
        public SystemSettings()
        {
            _instance = this;
            InitializeComponent();
            GetSettings();
        }

        public void GetSettings()
        {
            using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
            {
                var crawlerLimit = context.Settings.Single(x => x.SettingKey == "crawlers_crawlpool_maxcrawlers");
                var crawlersPerSite = context.Settings.Single(x => x.SettingKey == "crawlers_crawlpool_maxpersite");
                var getBacklinksNextCheck = context.Settings.Single(x => x.SettingKey == "backlinks_next_check");
                var getAnalysisMaxScanners = context.Settings.Single(x => x.SettingKey == "analysis_maxscanners");
                var getAnalysisMaxPerSite = context.Settings.Single(x => x.SettingKey == "analysis_maxscanners_persite");
                var getSitemapsDelay = context.Settings.Single(x => x.SettingKey == "sitemaps_new_delay");
                var getSitemapsNextCheck = context.Settings.Single(x => x.SettingKey == "sitemaps_next_check_days");
                var getCrawledPagesNextCheck = context.Settings.Single(x => x.SettingKey == "crawledpages_next_check_days");               

                Settings_CrawlerLimit_Description.Text = crawlerLimit.Description;
                Settings_CrawlerLimit_Value.Text = crawlerLimit.SettingValue;
                Settings_CrawlerMaxPerSite_Description.Text = crawlersPerSite.Description;
                Settings_CrawlerMaxPerSite_Value.Text = crawlersPerSite.SettingValue;
                Backlinks_Next_Check_Hours_Value.Text = getBacklinksNextCheck.SettingValue.ToString();
                Backlinks_Next_Check_Hours_Description.Text = getBacklinksNextCheck.Description.ToString();
                
                Max_Analysis_Scanners_Value.Text = getAnalysisMaxScanners.SettingValue.ToString();
                Max_Analysis_Scanners_Description.Text = getAnalysisMaxScanners.Description.ToString();
                Max_Analysis_PerSite_Value.Text = getAnalysisMaxPerSite.SettingValue.ToString();
                Max_Analysis_PerSite_Description.Text = getAnalysisMaxPerSite.Description.ToString();

                Sitemaps_Delay_Value.Text = getSitemapsDelay.SettingValue.ToString();
                Sitemaps_Delay_Description.Text = getSitemapsDelay.Description.ToString();
                Sitemaps_NextCheck_Value.Text = getSitemapsNextCheck.SettingValue.ToString();
                Sitemaps_NextCheck_Description.Text = getSitemapsNextCheck.Description.ToString();

                CrawledPages_NextCheck_Value.Text = getCrawledPagesNextCheck.SettingValue.ToString();
                CrawledPages_NextCheck_Description.Text = getCrawledPagesNextCheck.Description.ToString();

            }
        }

        public void Save_Settings_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
            {
                var getToastWindow = new ToastWindow();
                var crawlerLimit = context.Settings.Single(x => x.SettingKey == "crawlers_crawlpool_maxcrawlers");
                var crawlerMaxPerSite = context.Settings.Single(x => x.SettingKey == "crawlers_crawlpool_maxpersite");
                var getBacklinksNextCheck = context.Settings.Single(x => x.SettingKey == "backlinks_next_check");
                var getAnalysisMaxScanners = context.Settings.Single(x => x.SettingKey == "analysis_maxscanners");
                var getAnalysisMaxPerSite = context.Settings.Single(x => x.SettingKey == "analysis_maxscanners_persite");
                var getSitemapsDelay = context.Settings.Single(x => x.SettingKey == "sitemaps_new_delay");
                var getSitemapsNextCheck = context.Settings.Single(x => x.SettingKey == "sitemaps_next_check_days");
                var getCrawledPagesNextCheck = context.Settings.Single(x => x.SettingKey == "crawledpages_next_check_days");
                int numbervalue;
                bool crawlerIsNumber = int.TryParse(Settings_CrawlerLimit_Value.Text, out numbervalue);
                bool crawlerPerSiteIsNumber = int.TryParse(Settings_CrawlerMaxPerSite_Value.Text, out numbervalue);
                bool BLNextCheckIsNumber = int.TryParse(Backlinks_Next_Check_Hours_Value.Text, out numbervalue);
                bool AnalysisScannersIsNumber = int.TryParse(Max_Analysis_Scanners_Value.Text, out numbervalue);
                bool AnalysisPerSiteIsNumber = int.TryParse(Max_Analysis_PerSite_Value.Text, out numbervalue);
                bool SitemapsDelayIsNumber = int.TryParse(Sitemaps_Delay_Value.Text, out numbervalue);
                bool SitemapsNextCheckIsNumber = int.TryParse(Sitemaps_NextCheck_Value.Text, out numbervalue);
                bool CrawledPagesNextCheckIsNumber = int.TryParse(CrawledPages_NextCheck_Value.Text, out numbervalue);
                // if the value is not a number show error box to enter number

                if (!crawlerIsNumber)
                {
                    getToastWindow.Send_Message("Crawler Limit Needs Number", "Please enter a number instead of text for example 5");
                    getToastWindow.Show();
                    return;
                }
                if (!crawlerPerSiteIsNumber)
                {
                    getToastWindow.Send_Message("Max Per Site Limit Needs Number", "Please enter a number instead of text for example 2");
                    getToastWindow.Show();
                    return;
                }
                if (!BLNextCheckIsNumber)
                {
                    getToastWindow.Send_Message("Backlinks Next Check Needs Number", "Please enter a number instead of text for example 24");
                    getToastWindow.Show();
                   
                    return;
                }
                if (!AnalysisScannersIsNumber)
                {
                    getToastWindow.Send_Message("Max Analysis Scanners Needs Number", "Please enter a number instead of text for example 5");
                    getToastWindow.Show();
                    return;
                }
                if (!AnalysisPerSiteIsNumber)
                {
                    getToastWindow.Send_Message("Max Analysis Per Site Needs Number", "Please enter a number instead of text for example 2");
                    getToastWindow.Show();                   
                    return;
                }
                if (!SitemapsDelayIsNumber)
                {
                    getToastWindow.Send_Message("Sitemaps Delay Needs Number", "Please enter a number instead of text for example 10");
                    getToastWindow.Show();
                    return;
                }
                if (!SitemapsNextCheckIsNumber)
                {
                    
                    getToastWindow.Send_Message("Sitemaps Next Check Needs Number", "Please enter a number instead of text for example 7");
                    getToastWindow.Show();                    
                    return;
                }
                if (!CrawledPagesNextCheckIsNumber)
                {

                    getToastWindow.Send_Message("Crawled Pages Next Check Needs Number", "Please enter a number instead of text for example 14");
                    getToastWindow.Show();
                    return;
                }
                crawlerLimit.SettingValue = Settings_CrawlerLimit_Value.Text;
                crawlerMaxPerSite.SettingValue = Settings_CrawlerMaxPerSite_Value.Text;
                getBacklinksNextCheck.SettingValue = Backlinks_Next_Check_Hours_Value.Text;
                getAnalysisMaxScanners.SettingValue = Max_Analysis_Scanners_Value.Text;
                getAnalysisMaxPerSite.SettingValue = Max_Analysis_PerSite_Value.Text;
                getSitemapsDelay.SettingValue = Sitemaps_Delay_Value.Text;
                getSitemapsNextCheck.SettingValue = Sitemaps_NextCheck_Value.Text;
                getCrawledPagesNextCheck.SettingValue = CrawledPages_NextCheck_Value.Text;

                // save the settings
                context.SaveChanges();
                getToastWindow.Send_Message("Saved System Settings", "You have successfully saved the system settings");
                getToastWindow.Show();
                /*Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ViewDetails_Page_Title.Content = "Settings Saved";
                        ViewDetails_Page_Title_BG.Background = Brushes.DarkGreen;
                    });
                    Thread.Sleep(3000);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ViewDetails_Page_Title.Content = "System";
                        var colorConverter = new BrushConverter();
                        ViewDetails_Page_Title_BG.Background = (Brush)colorConverter.ConvertFromString("#121a2e");
                    });
                });*/
            }
        }

        public void Reset_Settings_Click(object sender, RoutedEventArgs e)
        {
            GetSettings();
        }
    }
}
