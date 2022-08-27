using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.MySites.Actions
{
    /// <summary>
    /// Interaction logic for EditSitePage.xaml
    /// </summary>
    public partial class EditSitePage : UserControl
    {
        public static EditSitePage? _instance;
        // Getters and setters varibles to use over all functions
        public int getMySiteId { get; set; }
        public int getMySitePageId { get; set; }
        public EditSitePage()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int MySitePageId)
        {
            getMySitePageId = MySitePageId;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getMySitePage = context.MySitesPages.Where(x => x.Id == MySitePageId).Single();
                var getMySite = context.MySites.Where(x => x.Id == getMySitePage.MySiteId).Single();

                getMySiteId = getMySite.Id;
                Edit_SitePage_Website_Name.Text = getMySite.SiteName.ToString();
                Edit_SitePage_PageName_Name.Text = getMySitePage.PageName.ToString();
                Edit_SitePage_Url_Data.Text = getMySitePage.Url.ToString();
                Edit_SitePage_Description_Data.Text = getMySitePage.Description.ToString();
            }
        }


        void OnLoad(object sender, RoutedEventArgs e)
        {

        }

        public void getMySitePageUrl(string url)
        {
            Edit_SitePage_Url_Data.Text = url;
        }

        public void Submit_SitePage_Button(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (Edit_SitePage_PageName_Name.Text == string.Empty)
                {
                    MessageBox.Show("My Site page name is required, Please give a name for the page you wish to save", "Need To Enter My Site Page");
                    return;
                }
                var sitePage = context.MySitesPages.Single(x => x.Id == getMySitePageId);


                sitePage.PageName = Edit_SitePage_PageName_Name.Text;
                sitePage.Description = Edit_SitePage_Description_Data.Text;
                sitePage.Updated = DateTime.Now;

                // Do Page Counts
                //findSite.PagesCount++;

                // Save to database the new backlink
                context.SaveChanges();

                // Update Listings and Cards
                Views.MySites.Listing._instance.GetMySites();
                Views.MySites.Details.SitePages._instance.GetSitePages();

                // After work reset the form and go back to website view
                ViewDetails._instance.GetView_SitePages();

                //MainWindow._instance.StatusBar_DoAction("Adding New Site", "Successfully added new site", 3000);
                //MainWindow._instance.SendToast("Added New My Site Page", "You have successfully added a new page to my sites");
            }
        }

        public void Cancel_SitePage_Button(object sender, RoutedEventArgs e)
        {
            Edit_SitePage_PageName_Name.Text = null;
            Edit_SitePage_Description_Data.Text = null;
            ViewDetails._instance.GetView_SitePages();
        }

    }
}
