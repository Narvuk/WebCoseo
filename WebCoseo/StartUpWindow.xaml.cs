using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WebCoseo.Helpers;

namespace WebCoseo
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        public static SplashScreenWindow? _instance;
        public SplashScreenWindow()
        {
            _instance = this;
            InitializeComponent();
            RunStartUp();
        }

        public async void RunStartUp()
        {
            await Task.Run(async () =>
            {
            this.Dispatcher.Invoke(() =>
            {
                loading_info.Text = "Checking Databases";
            });
                DatabaseFacade coreFacade = new DatabaseFacade(new AppServices.CoreDbContext());
                DatabaseFacade mainFacade = new DatabaseFacade(new AppServices.WebcoseoDbContext());
                DatabaseFacade crawlersFacade = new DatabaseFacade(new AppServices.CrawlersDbContext());
                DatabaseFacade countersFacade = new DatabaseFacade(new AppServices.CountersDbContext());
                DatabaseFacade reportsFacade = new DatabaseFacade(new AppServices.ReportsDbContext());
                await coreFacade.EnsureCreatedAsync();
                await mainFacade.EnsureCreatedAsync();
                await crawlersFacade.EnsureCreatedAsync();
                await countersFacade.EnsureCreatedAsync();
                await reportsFacade.EnsureCreatedAsync();
            });

            await Task.Run(async () =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    loading_info.Text = "System Configuration Checks";
                });
                // System 
                var SysConfCheck = new checkDbCoredata();
                await SysConfCheck.RunCheck();
                // Counters
                var CrawlersCountersCheck = new Helpers.Counters.CrawlersDbdata();
                await CrawlersCountersCheck.RunCheck();
            });

            await Task.Run(async () =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    loading_info.Text = "Checking Updates";
                });
                var doUpdates = new DoUpdates();
                await doUpdates.Run();
            });
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    loading_info.Text = "All Checks Done, Starting Up";
                });
                System.Threading.Thread.Sleep(3000);
            });

            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                });
            });
        }



        public double Progress
        {
            get { return start_progressBar.Value; }
            set { start_progressBar.Value = value; }
        }


        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public void Running_Updates(string update)
        {
            loading_info.Text = update;
        }
    }
}
