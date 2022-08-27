using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Crawlers
{
    /// <summary>
    /// Interaction logic for ViewDetails.xaml
    /// </summary>
    public partial class ViewDetails : UserControl
    {
        public static ViewDetails? _instance;
        public ViewDetails()
        {
            _instance = this;
            InitializeComponent();
            SetActiveUserControl(Overview);
        }

        public void SetActiveUserControl(UserControl control)
        {
            Overview.Visibility = Visibility.Collapsed;
            CrawlPool.Visibility = Visibility.Collapsed;
            SkipCrawl.Visibility = Visibility.Collapsed;
            SkipContains.Visibility = Visibility.Collapsed;
            CrawlErrors.Visibility = Visibility.Collapsed;
            AddSkipCrawl.Visibility = Visibility.Collapsed;
            AddSkipContains.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }


        public void Crawlers_Overview_Menu_Click(object sender, RoutedEventArgs e)
        {
            Details.Overview._instance.Update_Crawler_Dashboard();
            SetActiveUserControl(Overview);
        }


        public void Crawlers_Overview_MainWindow()
        {
            SetActiveUserControl(Overview);
        }

        public void Crawlers_CrawlPool_Menu_Click(object sender, RoutedEventArgs e)
        {
            Details.CrawlPool._instance.Get_CrawlPool();
            SetActiveUserControl(CrawlPool);
        }

        public void Crawlers_SkipCrawl_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(SkipCrawl);
        }

        public void Crawlers_SkipContains_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(SkipContains);
        }

        public void Crawlers_CrawlErrors_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(CrawlErrors);
        }

        public void GetView_AddSkipCrawl()
        {
            SetActiveUserControl(AddSkipCrawl);
        }

        public void GetView_AddSkipContains()
        {
            SetActiveUserControl(AddSkipContains);
        }

        public void GetView_SkipCrawl_List()
        {
            SetActiveUserControl(SkipCrawl);
        }

        public void GetView_SkipContains_List()
        {
            SetActiveUserControl(SkipContains);
        }

    }
}
