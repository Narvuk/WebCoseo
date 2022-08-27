using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.MySites.Actions
{
    /// <summary>
    /// Interaction logic for AddSitePage.xaml
    /// </summary>
    public partial class AddSitePage : UserControl
    {
        public static AddSitePage? _instance;
        // Getters and setters varibles to use over all functions
        public int GetMySiteId { get; set; }
        public AddSitePage()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int MySiteId)
        {
            GetMySiteId = MySiteId;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getMySite = context.MySites.Where(x => x.Id == MySiteId).Single();
                AddNew_SitePage_Website_Name.Text = getMySite.SiteName.ToString();
            }
        }


        void OnLoad(object sender, RoutedEventArgs e)
        {

        }

        public void getMySitePageUrl(string url)
        {
            AddNew_SitePage_Url_Data.Text = url;
        }

        public void Submit_SitePage_Button(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (AddNew_SitePage_PageName_Name.Text == string.Empty)
                {
                    MessageBox.Show("My Site page name is required, Please give a name for the page you wish to save", "Need To Enter My Site Page");
                    return;
                }
                var findSite = context.MySites.Single(x => x.Id == GetMySiteId);

                context.MySitesPages.Add(new Models.MySitesPages
                {
                    MySiteId = GetMySiteId,
                    Url = AddNew_SitePage_Url_Data.Text,
                    PageName = AddNew_SitePage_PageName_Name.Text,
                    Description = AddNew_SitePage_Description_Data.Text,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });

                // Do Page Counts
                findSite.PagesCount++;

                // Save to database the new backlink
                context.SaveChanges();
                // After work reset the form and go back to website view
                Views.MySites.Details.ViewWebsite._instance.AddSitePage_View.Visibility = Visibility.Collapsed;
                Views.MySites.Details.ViewWebsite._instance.MySites_Browser_View.Visibility = Visibility.Visible;
                // Update Listings and Cards
                Views.MySites.Listing._instance.GetMySites();
                Views.MySites.Details.SitePages._instance.GetSitePages();
                //Views.HomeMain._instance.UpdateDashboard();
                //MainWindow._instance.StatusBar_DoAction("Adding New Site", "Successfully added new site", 3000);
                //MainWindow._instance.SendToast("Added New My Site Page", "You have successfully added a new page to my sites");
            }
        }

        public void Cancel_SitePage_Button(object sender, RoutedEventArgs e)
        {
            AddNew_SitePage_PageName_Name.Text = null;
            AddNew_SitePage_Description_Data.Text = null;
            Views.MySites.Details.ViewWebsite._instance.AddSitePage_View.Visibility = Visibility.Collapsed;
            Views.MySites.Details.ViewWebsite._instance.MySites_Browser_View.Visibility = Visibility.Visible;
        }

    }
}
