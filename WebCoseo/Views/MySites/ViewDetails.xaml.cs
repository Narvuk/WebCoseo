using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WebCoseo.Models;

namespace WebCoseo.Views.MySites
{
    /// <summary>
    /// Interaction logic for MySitesDetails.xaml
    /// </summary>
    public partial class ViewDetails : UserControl
    {
        public static ViewDetails? _instance;
        int GetMySiteId { get; set; }
        public ViewDetails()
        {
            _instance = this;
            InitializeComponent();
            SetActiveUserControl(Overview);
        }

        public void LoadId(int MySiteId)
        {
            GetMySiteId = MySiteId;
            Details.Overview._instance.LoadDetails(MySiteId);
            Details.SitePages._instance.LoadDetails(MySiteId);
            Details.SiteMaps._instance.LoadDetails(MySiteId);
            Details.Backlinks._instance.LoadDetails(MySiteId);
            Details.ViewWebsite._instance.LoadDetails(MySiteId);
            Actions.AddSitePage._instance.LoadDetails(MySiteId);
            Actions.AddSiteMap._instance.LoadDetails(MySiteId);
        }

        public void SetActiveUserControl(UserControl control)
        {
            Overview.Visibility = Visibility.Collapsed;
            ViewWebsite.Visibility = Visibility.Collapsed;
            SitePages.Visibility = Visibility.Collapsed;
            SiteMaps.Visibility = Visibility.Collapsed;
            Backlinks.Visibility = Visibility.Collapsed;
            ViewBacklink.Visibility = Visibility.Collapsed;
            // Actions
            EditSitePage.Visibility = Visibility.Collapsed;
            AddSiteMap.Visibility = Visibility.Collapsed;
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
                var getWebsite = context.MySites.Where(x => x.Id == GetMySiteId).Single();
                Details.ViewWebsite._instance.WebsitesBrowser_LoadPage(getWebsite.Url);

            }
            Details.ViewWebsite._instance.MySites_Browser_View.Visibility = Visibility.Visible;
            SetActiveUserControl(ViewWebsite);
        }

        public void SitePages_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(SitePages);
        }
        public void ViewSitePage_Load()
        {
            SetActiveUserControl(SitePages);
        }

        public void SiteMaps_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(SiteMaps);
        }
        public void ViewSiteMaps_Load()
        {
            SetActiveUserControl(SiteMaps);
        }

        public void GetView_SitePages()
        {
            SetActiveUserControl(SitePages);
        }

        public void GetView_EditSitePage()
        {
            SetActiveUserControl(EditSitePage);
        }

        public void GetView_AddSiteMap()
        {
            SetActiveUserControl(AddSiteMap);
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
            Details.ViewWebsite._instance.MySites_Browser_View.Visibility = Visibility.Visible;
            SetActiveUserControl(ViewWebsite);
        }

    }

}
