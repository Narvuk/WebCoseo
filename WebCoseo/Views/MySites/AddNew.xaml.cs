using System;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.MySites
{
    /// <summary>
    /// Interaction logic for MySitesAddNew.xaml
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
            Main._instance.ViewMySitesListing();
            ClearSubmitBoxes();
        }

        private void SubmitAddNew_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                // Check to see if field that are required are filled in
                if (string.IsNullOrEmpty(msAddNew_SiteName_data.Text))
                {
                    // Show message?
                    MessageBox.Show("Please ensure this field is filled in before submitting", "Site Name is Required");

                    return;
                }

                if (string.IsNullOrEmpty(msAddNew_SiteUrl_data.Text))
                {
                    // Show message?
                    MessageBox.Show("Please ensure this field is filled in before submitting", "Site Url is Required");

                    return; // Don't process
                }
                if (!string.IsNullOrEmpty(msAddNew_SiteUrl_data.Text) && !msAddNew_SiteUrl_data.Text.StartsWith("https://"))
                {
                    MessageBox.Show("The website url must start with https:// - for example https://google.com", "Missing https://");
                    return; // Dont process further
                }
                context.MySites.Add(new Models.MySites
                {
                    SiteName = msAddNew_SiteName_data.Text,
                    Url = msAddNew_SiteUrl_data.Text,
                    Description = msAddNew_SiteDescription_data.Text,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });
                context.SaveChanges();
                ClearSubmitBoxes();
                Listing._instance.GetMySites();
                Main._instance.ViewMySitesListing();
            }
        }

        public void ClearSubmitBoxes()
        {
            msAddNew_SiteName_data.Text = "";
            msAddNew_SiteUrl_data.Text = "";
            msAddNew_SiteDescription_data.Text = "";
        }

    }
}
