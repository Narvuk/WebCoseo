using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Websites
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : UserControl
    {
        public static Edit? _instance;
        public int GetWebsiteId { set; get; }
        public Edit()
        {
            _instance = this;
            InitializeComponent();
        }


        public void LoadInitial(int WebsiteId)
        {
            GetWebsiteId = WebsiteId;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getWebSite = context.Websites.Single(x => x.Id == GetWebsiteId);
                Edit_SiteName_data.Text = getWebSite.Name;
                Edit_SiteDescription_data.Text = getWebSite.Description;
            }
        }


        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            Main._instance.ViewWebsitesListing();
            ClearSubmitBoxes();
        }

        private void SubmitEdit_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                // Check to see if field that are required are filled in
                if (string.IsNullOrEmpty(Edit_SiteName_data.Text))
                {
                    // Show message?
                    MessageBox.Show("Please ensure this field is filled in before submitting", "Site Name is Required");

                    return;
                }

                var getWebSite = context.Websites.Single(x => x.Id == GetWebsiteId);


                getWebSite.Name = Edit_SiteName_data.Text;
                getWebSite.Description = Edit_SiteDescription_data.Text;
                getWebSite.Updated = DateTime.Now;

                context.SaveChanges();
                ClearSubmitBoxes();
                Listing._instance.GetWebsites();
                Main._instance.ViewWebsitesListing();
            }
        }

        public void ClearSubmitBoxes()
        {
            Edit_SiteName_data.Text = "";
            Edit_SiteDescription_data.Text = "";
        }
    }
}
