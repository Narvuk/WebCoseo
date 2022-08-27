using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Analysis
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
            Sites.Visibility = Visibility.Collapsed;
            FoundSites.Visibility = Visibility.Collapsed;
            FoundSitesBrowser.Visibility = Visibility.Collapsed;
            CrawlersPages.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }


        public void Analysis_Overview_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(Overview);
        }

        public void Analysis_Sites_Menu_Click(object sender, RoutedEventArgs e)
        {
            Details.Sites._instance.LoadInitial();
            SetActiveUserControl(Sites);
        }

        public void Analysis_FoundSites_Menu_Click(object sender, RoutedEventArgs e)
        {
            Details.FoundSites._instance.LoadInitial();
            SetActiveUserControl(FoundSites);
        }

        public void Analysis_ViewWebSite_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(FoundSitesBrowser);
        }

        public void Analysis_CrawlersPages_Menu_Click(object sender, RoutedEventArgs e)
        {
            Details.CrawlersPages._instance.LoadInitial();
            SetActiveUserControl(CrawlersPages);
        }
       

        public void Analysis_Overview_MainWindow()
        {
            SetActiveUserControl(Overview);
        }

        public void GetView_ViewWebsite()
        {
            SetActiveUserControl(FoundSitesBrowser);
        }

        public void GetView_FoundSites()
        {
            Details.FoundSites._instance.LoadInitial();
            SetActiveUserControl(FoundSites);
        }

    }
}
