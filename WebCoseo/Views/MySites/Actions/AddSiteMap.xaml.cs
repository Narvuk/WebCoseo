using System;
using System.Linq;
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

namespace WebCoseo.Views.MySites.Actions
{
    /// <summary>
    /// Interaction logic for AddSiteMap.xaml
    /// </summary>
    public partial class AddSiteMap : UserControl
    {
        public static AddSiteMap? _instance;
        // Getters and setters varibles to use over all functions
        public int GetMySiteId { get; set; }
        public AddSiteMap()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int MySiteId)
        {
            GetMySiteId = MySiteId;
        }

        public void Submit_SiteMap_Button(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (Edit_SiteMap_Url.Text == string.Empty)
                {
                    MessageBox.Show("Sitemap Url is required, Please provide a sitemap url you wish to save", "Need To Enter Sitemap Url");
                    return;
                }
                //var findSite = context.MySites.Single(x => x.Id == GetMySiteId);

                context.MySitesSiteMaps.Add(new Models.MySitesSiteMaps
                {
                    MySiteId = GetMySiteId,
                    Status = "Pending",
                    SitemapUrl = Edit_SiteMap_Url.Text,
                    NextRead = DateTime.Now.AddMinutes(10),
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });


                // Save to database the new backlink
                context.SaveChanges();

                Edit_SiteMap_Url.Text = null;
                // Update Listings and Cards
                Views.MySites.Listing._instance.GetMySites();
                Views.MySites.Details.SiteMaps._instance.GetSiteMaps();

                // After work reset the form and go back to list view
                ViewDetails._instance.ViewSiteMaps_Load();

                //MainWindow._instance.StatusBar_DoAction("Adding New Site", "Successfully added new site", 3000);
                //MainWindow._instance.SendToast("Added New My Site Page", "You have successfully added a new page to my sites");
            }
        }

        public void Cancel_SiteMap_Button(object sender, RoutedEventArgs e)
        {
            Edit_SiteMap_Url.Text = null;
            ViewDetails._instance.ViewSiteMaps_Load();
        }
    }
}
