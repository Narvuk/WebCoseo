using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebCoseo.Helpers.Crawlers;

namespace WebCoseo.Views.CronTasks
{
    /// <summary>
    /// Interaction logic for CronTasksMain.xaml
    /// </summary>
    public partial class CronTasksMain : UserControl
    {
        public static CronTasksMain? _instance;
        public CronTasksMain()
        {
            _instance = this;
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Get_CronTasks();
        }

        private void ClearCronTasks_List()
        {
            CronTasks_List.ItemsSource = null;
            CronTasks_List.Items.Clear();
        }

        public void Get_CronTasks()
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                ClearCronTasks_List();
                CronTasks_List.ItemsSource = context.CronTasks.OrderBy(x => x.Id).ToList();
            }
        }

        private async void StopService_Click(object sender, RoutedEventArgs e)
        {
            var getToastWindow = new ToastWindow();
            using AppServices.WebcoseoDbContext context = new();
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            var getCronTask = context.CronTasks.Single(x => x.Id == idconv);
            if (getCronTask.Status == "Stopped")
            {
                getToastWindow.Send_Message("Already Stopped", getCronTask.Name.ToString() + " is already stopped");
                getToastWindow.Show();
            }
            if (getCronTask.Status != "Stopped")
            {
                if (MessageBox.Show("Do you really want to stop " + getCronTask.Name.ToString() + "?",
                       "Confirm Stop Service",
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    if (getCronTask.CronKey == "crawler_crawlpool_service")
                    {
                        getCronTask.Status = "Stopped";
                        context.SaveChanges();

                        await Task.Run(() =>
                        {
                            var startService = new CrawlerPoolStart();
                            startService.Run();
                        });
                    }
                    else
                    {

                        var stopService = new Helpers.CronTasks.CronTaskStop();
                        stopService.Run(getCronTask.Id);
                    }
                    Get_CronTasks();
                }
            }

        }
        private async void StartService_Click(object sender, RoutedEventArgs e)
        {
            var getToastWindow = new ToastWindow();
            using AppServices.WebcoseoDbContext context = new();
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            var getCronTask = context.CronTasks.Single(x => x.Id == idconv);
            if (getCronTask.Status == "Running")
            {
                getToastWindow.Send_Message("Already Running", getCronTask.Name.ToString() + " is already running");
                getToastWindow.Show();
            }
            if (getCronTask.Status != "Running")
            {
                if (getCronTask.CronKey == "crawler_crawlpool_service")
                {
                    getCronTask.Status = "Running";
                    context.SaveChanges();

                    await Task.Run(() =>
                    {
                        var startService = new CrawlerPoolStart();
                        startService.Run();
                    });
                }
                else
                {
                    var startService = new Helpers.CronTasks.CronTaskStart();
                    startService.Run(getCronTask.Id);
                }
                Get_CronTasks();
               
            }

        }
    }
}
