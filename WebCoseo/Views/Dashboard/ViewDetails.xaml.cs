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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebCoseo.Views.About.Details;

namespace WebCoseo.Views.Dashboard
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
            SetActiveUserControl(HomeMain);
        }

        public void ViewDetails_ChangeTitle(string title)
        {
            ViewDetails_Title.Content = title;
        }

        public void SetActiveUserControl(UserControl control)
        {
            HomeMain.Visibility = Visibility.Collapsed;
            ReportsMySitesPages.Visibility = Visibility.Collapsed;
            ReportsWebsites.Visibility = Visibility.Collapsed;
            ReportsBacklinksTotal.Visibility = Visibility.Collapsed;
            ReportsBacklinksActiveTotal.Visibility = Visibility.Collapsed;
            ReportsBacklinksNewTotal.Visibility = Visibility.Collapsed;
            ReportsBacklinksLostTotal.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }


        public void HomeMain_Overview_MainWindow()
        {
            ViewDetails_ChangeTitle("Dashboard");
            SetActiveUserControl(HomeMain);
        }

        public void HomeMain_Overview_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails_ChangeTitle("Dashboard");
            Details.HomeMain._instance.UpdateDashboard();
            SetActiveUserControl(HomeMain);
        }

        public void Reports_MySitesPages_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails_ChangeTitle("Totals Stats");
            Details.ReportsMySitesPages._instance.LoadInitial();
            SetActiveUserControl(ReportsMySitesPages);
        }

        public void Reports_Websites_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails_ChangeTitle("Totals Stats");
            Details.ReportsWebsites._instance.LoadInitial();
            SetActiveUserControl(ReportsWebsites);
        }

        public void Reports_BacklinksTotal_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails_ChangeTitle("Totals Stats");
            Details.ReportsBacklinksTotal._instance.LoadInitial();
            SetActiveUserControl(ReportsBacklinksTotal);
        }

        public void Reports_BacklinksActiveTotal_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails_ChangeTitle("Totals Stats");
            Details.ReportsBacklinksActiveTotal._instance.LoadInitial();
            SetActiveUserControl(ReportsBacklinksActiveTotal);
        }

        public void Reports_BacklinksNewTotal_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails_ChangeTitle("Totals Stats");
            Details.ReportsBacklinksNewTotal._instance.LoadInitial();
            SetActiveUserControl(ReportsBacklinksNewTotal);
        }

        public void Reports_BacklinksLostTotal_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails_ChangeTitle("Totals Stats");
            Details.ReportsBacklinksLostTotal._instance.LoadInitial();
            SetActiveUserControl(ReportsBacklinksLostTotal);
        }

    }
}
