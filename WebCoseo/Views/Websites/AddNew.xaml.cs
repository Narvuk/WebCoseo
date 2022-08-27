using System;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Websites
{
    /// <summary>
    /// Interaction logic for AddNew.xaml
    /// </summary>
    public partial class AddNew : UserControl
    {
        public static AddNew? _instance;
        public AddNew()
        {
            _instance = this;
            InitializeComponent();
        }

        private void CancelAddNew_Click(object sender, RoutedEventArgs e)
        {
            Main._instance.ViewWebsitesListing();
            ClearSubmitBoxes();
        }

        private void SubmitAddNew_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                // Check to see if field that are required are filled in
                if (string.IsNullOrEmpty(AddNew_SiteName_data.Text))
                {
                    // Show message?
                    MessageBox.Show("Please ensure this field is filled in before submitting", "Site Name is Required");

                    return;
                }

                if (string.IsNullOrEmpty(AddNew_SiteUrl_data.Text))
                {
                    // Show message?
                    MessageBox.Show("Please ensure this field is filled in before submitting", "Site Url is Required");

                    return; // Don't process
                }
                if (!string.IsNullOrEmpty(AddNew_SiteUrl_data.Text) && !AddNew_SiteUrl_data.Text.StartsWith("https://"))
                {
                    MessageBox.Show("The website url must start with https:// - for example https://google.com", "Missing https://");
                    return; // Dont process further
                }
                context.Websites.Add(new Models.Websites
                {
                    Name = AddNew_SiteName_data.Text,
                    Url = AddNew_SiteUrl_data.Text,
                    Description = AddNew_SiteDescription_data.Text,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });
                context.SaveChanges();
                ClearSubmitBoxes();
                Listing._instance.GetWebsites();
                Main._instance.ViewWebsitesListing();
            }
        }

        public void ClearSubmitBoxes()
        {
            AddNew_SiteName_data.Text = "";
            AddNew_SiteUrl_data.Text = "";
            AddNew_SiteDescription_data.Text = "";
        }
    }
}
