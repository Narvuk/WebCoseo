using System;
using System.Linq;
using System.Windows.Controls;

namespace WebCoseo.Views.Dashboard.Details
{
    /// <summary>
    /// Interaction logic for HomeMain.xaml
    /// </summary>
    public partial class HomeMain : UserControl
    {
        public static HomeMain? _instance;
        public HomeMain()
        {
            _instance = this;
            InitializeComponent();
            UpdateDashboard();
        }

        public void UpdateDashboard()
        {
            using (AppServices.ReportsDbContext reportsContext = new())
            {
                MySitePages_Total_List.ItemsSource = reportsContext.MySitesPages_Dash_Monthly_Total.OrderByDescending(x => x.Id).Take(4).ToList();
                Websites_Total_List.ItemsSource = reportsContext.Websites_Dash_Monthly_Total.OrderByDescending(x => x.Id).Take(4).ToList();
                Backlinks_Total_List.ItemsSource = reportsContext.BL_Dash_Monthly_Total.OrderByDescending(x => x.Id).Take(4).ToList();
                Active_Backlinks_Total_List.ItemsSource = reportsContext.BL_Dash_Monthly_Active_Total.OrderByDescending(x => x.Id).Take(4).ToList();
                Lost_Backlinks_Total_List.ItemsSource = reportsContext.BL_Dash_Monthly_Lost_Total.OrderByDescending(x => x.Id).Take(4).ToList();
                New_Backlinks_Total_List.ItemsSource = reportsContext.BL_Dash_Monthly_New_Total.OrderByDescending(x => x.Id).Take(4).ToList();

            }
        }


    }
}
