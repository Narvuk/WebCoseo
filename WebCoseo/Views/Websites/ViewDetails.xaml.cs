using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WebCoseo.Models;

namespace WebCoseo.Views.Websites
{
    /// <summary>
    /// Interaction logic for ViewDetails.xaml
    /// </summary>
    public partial class ViewDetails : UserControl
    {
        public static ViewDetails? _instance;
        int GetWebsiteId { get; set; }
        public ViewDetails()
        {
            _instance = this;
            InitializeComponent();
            SetActiveUserControl(Overview);
        }

        public void LoadId(int WebsiteId)
        {
            GetWebsiteId = WebsiteId;
            Details.Overview._instance.LoadDetails(WebsiteId);
            Details.Backlinks._instance.LoadDetails(WebsiteId);
            Details.ViewWebsite._instance.LoadDetails(WebsiteId);
            Actions.AddBacklink._instance.LoadDetails(WebsiteId);
            
        }

        public void SetActiveUserControl(UserControl control)
        {
            Overview.Visibility = Visibility.Collapsed;
            ViewWebsite.Visibility = Visibility.Collapsed;
            Backlinks.Visibility = Visibility.Collapsed;
            ViewBacklink.Visibility = Visibility.Collapsed;
            // Actions
            EditBacklink.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }


        public void Overview_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(Overview);
        }

        public void Overview_Default_LoadPage()
        {
            SetActiveUserControl(Overview);
        }
        public void ViewWebSite_Menu_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getWebsite = context.Websites.Where(x => x.Id == GetWebsiteId).Single();
                Details.ViewWebsite._instance.WebsitesBrowser_LoadPage(getWebsite.Url);

            }
            Details.ViewWebsite._instance.Website_Browser_View.Visibility = Visibility.Visible;
            SetActiveUserControl(ViewWebsite);
        }

        public void Backlinks_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(Backlinks);
        }

        public void GetView_EditBacklink()
        {
            SetActiveUserControl(EditBacklink);
        }

        public void GetView_BacklinksList()
        {
            SetActiveUserControl(Backlinks);
        }

        public void GetView_ViewBacklink()
        {
            SetActiveUserControl(ViewBacklink);
        }

        public void GetView_ViewWebsite()
        {
            Details.ViewWebsite._instance.Website_Browser_View.Visibility = Visibility.Visible;
            SetActiveUserControl(ViewWebsite);
        }

    }
}
