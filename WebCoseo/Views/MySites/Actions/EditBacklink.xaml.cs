using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.MySites.Actions
{
    /// <summary>
    /// Interaction logic for EditBacklink.xaml
    /// </summary>
    public partial class EditBacklink : UserControl
    {
        public static EditBacklink? _instance;
        // Getters and setters varibles to use over all functions
        public int GetBacklinkId { get; set; }
        public int GetWebsiteId { get; set; }
        public int GetMySiteId { get; set; }
        public int GetMySitePageId { get; set; }

        public EditBacklink()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int backlinkId)
        {
            GetBacklinkId = backlinkId;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getBacklink = context.Backlinks.Single(bl => bl.Id == GetBacklinkId);
                var getWebsite = context.Websites.Single(x => x.Id == getBacklink.WebsiteId);
                var getMySite = context.MySites.Single(x => x.Id == getBacklink.MySiteId);
                var getMySitePage = context.MySitesPages.Single(x => x.Id == getBacklink.MySitePageId);
                GetWebsiteId = getWebsite.Id;
                GetMySiteId = getMySite.Id;
                GetMySitePageId = getMySitePage.Id;
                Edit_Backlink_Website_Name.Text = getWebsite.Name.ToString();
                Edit_Backlink_Url_Data.Text = getBacklink.BacklinkUrl.ToString();
                Edit_Backlink_MySiteUrl_Name.Text = getMySite.Url.ToString();
                Edit_Backlink_MySitePageUrl_Name.Text = getMySitePage.Url.ToString();

            }
        }


        void OnLoad(object sender, RoutedEventArgs e)
        {
            //InitializeAsync();
        }

        public void Submit_Backlink_Button(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {

                var getBacklink = context.Backlinks.Single(x => x.Id == GetBacklinkId);

                getBacklink.Description = Edit_Backlink_Description_Data.Text;
                getBacklink.Updated = DateTime.Now;

                // Save to database the new backlink
                context.SaveChanges();

                // After work reset the form and go back to website view

                // Update listing and counts
                Listing._instance.GetMySites();
                Details.Backlinks._instance.GetBacklinks();
                Dashboard.Details.HomeMain._instance.UpdateDashboard();

                // After work reset the form and go back to website view
                ViewDetails._instance.GetView_BacklinksList();

                //MainWindow._instance.StatusBar_DoAction("Adding New Backlink", "Successfully added new backlink", 3000);
                //MainWindow._instance.SendToast("Added New Backlink", "You have successfully added a new backlink");
            }
        }

        public void Cancel_Backlink_Button(object sender, RoutedEventArgs e)
        {

            Edit_Backlink_Description_Data.Text = null;

            // After work reset the form and go back to website view
            ViewDetails._instance.GetView_BacklinksList();
        }





    }
}
