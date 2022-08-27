using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebCoseo.Helpers.CronTasks;
using WebCoseo.Views;

namespace WebCoseo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? _instance;
        public MainWindow()
        {
            _instance = this;
            InitializeComponent();
            SetActiveUserControl(DashboardMain);
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                context.AppLogs.Add(new Models.AppLogs
                {
                    Created = DateTime.Now,
                    Type = "Information",
                    Section = "Application",
                    Message = "Application Started",

                });
                context.SaveChanges();
                // Start Services
                var getAppUpdate = context.CronTasks.Where(ct => ct.CronKey == "app_update_service").Single();
                var getBacklinksChecker = context.CronTasks.Where(ct => ct.CronKey == "backlinks_checker_service").Single();
                var getReportsService = context.CronTasks.Where(ct => ct.CronKey == "reports_service").Single();
                var getSitemapsService = context.CronTasks.Where(ct => ct.CronKey == "sitemaps_checker").Single();
                var getCrawledPagesCheckService = context.CronTasks.Where(ct => ct.CronKey == "crawler_crawlpages_check_service").Single();
                var getmySitesCountsService = context.CronTasks.Where(ct => ct.CronKey == "mysites_counts_checker").Single();
                var AppUpdateId = int.Parse(getAppUpdate.Id.ToString());
                var BacklinksCheckerId = int.Parse(getBacklinksChecker.Id.ToString());
                var ReportsServiceId = int.Parse(getReportsService.Id.ToString());
                var SitemapsServiceId = int.Parse(getSitemapsService.Id.ToString());
                var CrawledPagesCheckId = int.Parse(getCrawledPagesCheckService.Id.ToString());
                var MySitesCountsId = int.Parse(getmySitesCountsService.Id.ToString());
                var startAppUpdate = new CronTaskStart();
                var startBacklinksChecker = new CronTaskStart();
                var startReportsService = new CronTaskStart();
                var startSitemapsService = new CronTaskStart();
                var startCrawledPagesCheck = new CronTaskStart();
                var startMySitesCountsService = new CronTaskStart();
                startAppUpdate.Run(AppUpdateId);
                startBacklinksChecker.Run(BacklinksCheckerId);
                startReportsService.Run(ReportsServiceId);
                startSitemapsService.Run(SitemapsServiceId);
                startCrawledPagesCheck.Run(CrawledPagesCheckId);
                startMySitesCountsService.Run(MySitesCountsId);

               
            }

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ApplicationCloseButton_Click(object sender, RoutedEventArgs e)
        {

            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (MessageBox.Show("Do you really want to exit and close the application and running cron tasks?",
                    "Close Application",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    context.CronTasks.Where(x => x.Status == "Running").ToList().ForEach(a => a.Status = "Stopped");
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Application closed by user and all cron tasks stopped",

                    });
                    context.SaveChanges();

                    using (AppServices.CountersDbContext countersContext = new AppServices.CountersDbContext())
                    {
                        var activeCrawlers = countersContext.CrawlerCounters.Single(x => x.CounterKey == "active_crawlers");
                        var waitingCrawlers = countersContext.CrawlerCounters.Single(x => x.CounterKey == "waiting_crawlers");
                        var diedCrawlers = countersContext.CrawlerCounters.Single(x => x.CounterKey == "died_crawlers");
                        var activeScanners = countersContext.CrawlerCounters.Single(x => x.CounterKey == "active_scanners");
                        var waitingScanners = countersContext.CrawlerCounters.Single(x => x.CounterKey == "waiting_scanners");
                        var diedScanners = countersContext.CrawlerCounters.Single(x => x.CounterKey == "died_scanners");
                        activeCrawlers.CounterValue = 0;
                        waitingCrawlers.CounterValue = 0;
                        diedCrawlers.CounterValue = 0;
                        activeScanners.CounterValue = 0;
                        waitingScanners.CounterValue = 0;
                        diedScanners.CounterValue = 0;
                        countersContext.SaveChanges();
                    }

                    using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
                    {
                        crawlersContext.CrawlPool.Where(x => x.Status == "Working").ToList().ForEach(a => a.Status = "Waiting");
                        crawlersContext.CrawledPages.Where(x => x.Status == "Working").ToList().ForEach(a => a.Status = "Waiting");
                        crawlersContext.CrawlPool.Where(x => x.Status == "OnHold").ToList().ForEach(a => a.Status = "Waiting");
                        crawlersContext.CrawledPages.Where(x => x.Status == "OnHold").ToList().ForEach(a => a.Status = "Waiting");
                        crawlersContext.SaveChanges();
                    }

                    Close();
                }

            }

        }

        private void ApplicationMaximiseButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - 10;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 10;
        }
        private void ApplicationMinimiseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        public void SetActiveUserControl(UserControl control)
        {
            DashboardMain.Visibility = Visibility.Collapsed;
            MySitesMain.Visibility = Visibility.Collapsed;
            WebsitesMain.Visibility = Visibility.Collapsed;
            AnalysisMain.Visibility = Visibility.Collapsed;
            CrawlersMain.Visibility = Visibility.Collapsed;
            AppLogsMain.Visibility = Visibility.Collapsed;
            CronTasksMain.Visibility = Visibility.Collapsed;
            SettingsMain.Visibility = Visibility.Collapsed;
            AboutMain.Visibility = Visibility.Collapsed;
            UpdatesMain.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }

        private void SideMenu_Home_Click(object sender, RoutedEventArgs e)
        {
            Views.Dashboard.ViewDetails._instance.ViewDetails_ChangeTitle("Dashboard");
            Views.Dashboard.ViewDetails._instance.HomeMain_Overview_MainWindow();
            Views.Dashboard.Details.HomeMain._instance.UpdateDashboard();
            SetActiveUserControl(DashboardMain);
        }

        private void SideMenu_MySites_Click(object sender, RoutedEventArgs e)
        {
            Views.MySites.Main._instance.ViewMySitesListing();
            Views.MySites.Listing._instance.GetMySites();
            SetActiveUserControl(MySitesMain);
        }

        private void SideMenu_Websites_Click(object sender, RoutedEventArgs e)
        {
            Views.Websites.Main._instance.ViewWebsitesListing();
            Views.Websites.Listing._instance.GetWebsites();
            SetActiveUserControl(WebsitesMain);
        }

        private void SideMenu_Analysis_Click(object sender, RoutedEventArgs e)
        {
            Views.Analysis.Details.Overview._instance.Update_Analysis_Overview();
            Views.Analysis.ViewDetails._instance.Analysis_Overview_MainWindow();
            SetActiveUserControl(AnalysisMain);
        }

        private void SideMenu_Crawlers_Click(object sender, RoutedEventArgs e)
        {
            Views.Crawlers.Details.Overview._instance.Update_Crawler_Dashboard();
            Views.Crawlers.ViewDetails._instance.Crawlers_Overview_MainWindow();
            SetActiveUserControl(CrawlersMain);
        }

        private void SideMenu_AppLogs_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(AppLogsMain);
        }

        private void SideMenu_CronTasks_Click(object sender, RoutedEventArgs e)
        {
            Views.CronTasks.CronTasksMain._instance.Get_CronTasks();
            SetActiveUserControl(CronTasksMain);
        }

        private void SideMenu_Settings_Click(object sender, RoutedEventArgs e)
        {
            Views.Settings.ViewDetails._instance.Settings_System_MainWindow();
            SetActiveUserControl(SettingsMain);
        }

        private void SideMenu_AboutWebCoseo_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(AboutMain);
        }

        private void TopMenu_UpdateAvailable_Click(object sender, RoutedEventArgs e)
        {
            UpdatesMain._instance.Load_GetUpdates();
            SetActiveUserControl(UpdatesMain);
        }


        private async void TopMenu_CheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using AppServices.CoreDbContext coreContext = new();
                using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
                {
                    using (var httpClient = new HttpClient())
                    {
                        var json = await httpClient.GetStringAsync("https://updates.narvuk.com/webcoseo.json");
                        //var data = await json.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(json);
                        int cv = data.currentversion;
                        int systemversion = int.Parse(coreContext.SystemConfig.Where(cv => cv.ConfigKey == "sys_app_version_check").Single().ConfigValue);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (cv > systemversion)
                            {
                                IsUpdateAvailable("new");
                                var getToastWindow = new ToastWindow();
                                getToastWindow.Send_Message("Updates Check", "New Version Available!");
                                getToastWindow.Show();
                                //SendToast("Updates Check", "New Version Available!");

                            }
                            if (cv <= systemversion)
                            {
                                IsUpdateAvailable("current");
                                var getToastWindow = new ToastWindow();
                                getToastWindow.Send_Message("Updates Check", "No new version is available, you are on the latest version.");
                                getToastWindow.Show();
                                //SendToast("Updates Check", "No new version is available, you are on the latest version.");
                            }

                        });
                    }
                }
            }
            catch (Exception)
            {
                // If Error
            }

        }

        public void IsUpdateAvailable(string display)
        {
            if (display == "new")
            {
                App_Update_Button.Visibility = Visibility.Visible;
                App_CheckUpdate_Button.Visibility = Visibility.Collapsed;

            }
            if (display == "current")
            {
                App_Update_Button.Visibility = Visibility.Collapsed;
                App_CheckUpdate_Button.Visibility = Visibility.Visible;
            }
        }

        public void SendToast(string Title, string Message)
        {
            new ToastContentBuilder()
                .AddText(Title)
                .AddText(Message)
                //.AddButton(new ToastButton()
                // .SetContent("Read"))
                .Show();
        }

        /* public void UpdateWindowStatusLabel(string text)
         {
             Window_Status_Label.Text = text;
         }

         public void UpdateProgressBar(int value)
         {
             Window_Progress_Bar.Value = value;
         }*/

        public void Close_Window(bool IsReset)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (IsReset == false)
                {
                    context.CronTasks.Where(x => x.Status == "Running").ToList().ForEach(a => a.Status = "Stopped");
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Application closed by user and all cron tasks stopped",

                    });
                    context.SaveChanges();

                    using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
                    {
                        crawlersContext.CrawlPool.Where(x => x.Status == "Working").ToList().ForEach(a => a.Status = "Waiting");
                        crawlersContext.CrawlPool.Where(x => x.Status == "OnHold").ToList().ForEach(a => a.Status = "Waiting");
                        crawlersContext.SaveChanges();
                    }
                }

                Close();

            }
        }

        public async void StatusBar_DoAction(string beforeText, string AfterText, int Timer)
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    StatusBar_Progress.IsIndeterminate = true;
                    StatusBar_Text.Content = beforeText;
                });

                System.Threading.Thread.Sleep(Timer);

                this.Dispatcher.Invoke(() =>
                {
                    StatusBar_Progress.IsIndeterminate = false;
                    StatusBar_Text.Content = AfterText;
                });

                System.Threading.Thread.Sleep(3000);

                this.Dispatcher.Invoke(() =>
                {
                    StatusBar_Progress.IsIndeterminate = false;
                    StatusBar_Text.Content = "";
                });
            });
        }

    }
}

