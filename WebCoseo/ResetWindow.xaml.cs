using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WebCoseo.Helpers;

namespace WebCoseo
{
    /// <summary>
    /// Interaction logic for ResetWindow.xaml
    /// </summary>
    public partial class ResetWindow : Window
    {
        public static ResetWindow? _instance;
        public ResetWindow()
        {
            _instance = this;
            InitializeComponent();
            Task.Run(async () =>
            {
                int CountDownTimer = 20;

                this.Dispatcher.Invoke(() =>
                {
                    resetting_info.Text = "Stopping All Running Services, Please Wait.....";
                });

                using (AppServices.WebcoseoDbContext webcoseocontext = new AppServices.WebcoseoDbContext())
                {
                    webcoseocontext.CronTasks.Where(x => x.Status == "Running").ToList().ForEach(a => a.Status = "Stopped");
                    webcoseocontext.SaveChanges();
                    
                }
                // Stop the reporting
                AppServices.CronTasks.ReportsUpdater._instance.StopReportsUpdater();
                // Wait for the service to stop fully before reset
                while (CountDownTimer > 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        resetting_info.Text = "Stopping All Running Services, Please Wait... ( " + CountDownTimer + " )" ;
                    });
                    System.Threading.Thread.Sleep(1000);
                    CountDownTimer--;
                }

              

                DatabaseFacade mainfacade = new DatabaseFacade(new AppServices.WebcoseoDbContext());
                DatabaseFacade crawlersfacade = new DatabaseFacade(new AppServices.CrawlersDbContext());
                DatabaseFacade countersfacade = new DatabaseFacade(new AppServices.CountersDbContext());
                DatabaseFacade reportsfacade = new DatabaseFacade(new AppServices.ReportsDbContext());
                await mainfacade.EnsureDeletedAsync();
                await crawlersfacade.EnsureDeletedAsync();
                await countersfacade.EnsureDeletedAsync();
                await reportsfacade.EnsureDeletedAsync();


                this.Dispatcher.Invoke(() =>
                {
                    resetting_info.Text = " Application will restart shortly, Please Wait.....";
                });
                System.Threading.Thread.Sleep(5000);
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var startUpWindow = new SplashScreenWindow();
                    startUpWindow.Show();
                    this.Close();
                });
            });
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
