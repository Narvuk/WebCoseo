using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.MySites
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : UserControl
    {
        public static Edit? _instance;
        public int getMySiteId { get; set; }
        public Edit()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadInitial(int MySiteId)
        {
            getMySiteId = MySiteId;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getMySite = context.MySites.Single(x => x.Id == getMySiteId);
                msAddNew_SiteName_data.Text = getMySite.SiteName;
                msAddNew_SiteDescription_data.Text = getMySite.Description;
            }
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

                var mySite = context.MySites.Single(x => x.Id == getMySiteId);


                mySite.SiteName = msAddNew_SiteName_data.Text;
                mySite.Description = msAddNew_SiteDescription_data.Text;
                mySite.Updated = DateTime.Now;
                context.SaveChanges();

                ClearSubmitBoxes();
                Listing._instance.GetMySites();
                Main._instance.ViewMySitesListing();
            }
        }

        public void ClearSubmitBoxes()
        {
            msAddNew_SiteName_data.Text = "";
            msAddNew_SiteDescription_data.Text = "";
        }

    }
}
