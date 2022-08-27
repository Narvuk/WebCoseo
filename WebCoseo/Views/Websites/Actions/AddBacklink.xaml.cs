using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Websites.Actions
{
    /// <summary>
    /// Interaction logic for AddBacklink.xaml
    /// </summary>
    public partial class AddBacklink : UserControl
    {
        public static AddBacklink? _instance;
        // Getters and setters varibles to use over all functions
        public int GetWebsiteId { get; set; }
        public AddBacklink()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int WebsiteId)
        {
            this.GetWebsiteId = WebsiteId;
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getWebsite = context.Websites.Where(x => x.Id == WebsiteId).Single();
                AddNew_Backlink_Website_Name.Text = getWebsite.Name.ToString();

                AddNew_Backlink_MySite_ComboBox.ItemsSource = context.MySites.ToList().OrderBy(ms => ms.SiteName);
            }
        }


        void OnLoad(object sender, RoutedEventArgs e)
        {
            //InitializeAsync();
        }

        public void getBacklinkUrl(string url)
        {
            AddNew_Backlink_Url_Data.Text = url;
        }

        public void Submit_Backlink_Button(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (AddNew_Backlink_MySite_ComboBox.SelectedValue == null)
                {
                    MessageBox.Show("My Site is required, Please select one", "Need To Select My Site");
                    return;
                }
                if (AddNew_Backlink_MySitePage_ComboBox.SelectedValue == null)
                {
                    MessageBox.Show("My Site Page is required, Please select one", "Need To Select My Site Page");
                    return;
                }

                int findMySiteId = int.Parse(AddNew_Backlink_MySite_ComboBox.SelectedValue.ToString());
                int findSitePageId = int.Parse(AddNew_Backlink_MySitePage_ComboBox.SelectedValue.ToString());
                var MysiteRecord = context.MySites.Single(x => x.Id == findMySiteId);
                var MysitepageRecord = context.MySitesPages.Single(x => x.Id == findSitePageId);

                context.Backlinks.Add(new Models.Backlinks
                {
                    Status = "New",
                    BacklinkUrl = AddNew_Backlink_Url_Data.Text,
                    WebsiteId = GetWebsiteId,
                    MySiteId = findMySiteId,
                    MySitePageId = findSitePageId,
                    Description = AddNew_Backlink_Description_Data.Text,
                    LastChecked = DateTime.Now,
                    NextCheck = DateTime.Now.AddHours(24),
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });

                // Do Counts
                MysiteRecord.BacklinksCount++;
                MysitepageRecord.BacklinksCount++;

                // Save to database the new backlink
                context.SaveChanges();

                // After work reset the form and go back to website view
                AddNew_Backlink_MySite_ComboBox.SelectedIndex = -1;
                AddNew_Backlink_MySitePage_ComboBox.ItemsSource = null;
                Views.Websites.Details.ViewWebsite._instance.AddBacklinks_View.Visibility = Visibility.Collapsed;
                Views.Websites.Details.ViewWebsite._instance.Website_Browser_View.Visibility = Visibility.Visible;
                // Update listing and counts
                Views.Websites.Listing._instance.GetWebsites();
                Views.Websites.Details.Backlinks._instance.GetBacklinks();
                Views.Dashboard.Details.HomeMain._instance.UpdateDashboard();
                //MainWindow._instance.StatusBar_DoAction("Adding New Backlink", "Successfully added new backlink", 3000);
                //MainWindow._instance.SendToast("Added New Backlink", "You have successfully added a new backlink");
            }
        }

        public void Cancel_Backlink_Button(object sender, RoutedEventArgs e)
        {
            AddNew_Backlink_MySite_ComboBox.SelectedIndex = -1;
            AddNew_Backlink_MySitePage_ComboBox.ItemsSource = null;
            AddNew_Backlink_Description_Data.Text = null;
            Views.Websites.Details.ViewWebsite._instance.AddBacklinks_View.Visibility = Visibility.Collapsed;
            Views.Websites.Details.ViewWebsite._instance.Website_Browser_View.Visibility = Visibility.Visible;
        }



        private void MySitesPages_ComboBox(object sender, SelectionChangedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (AddNew_Backlink_MySite_ComboBox.SelectedValue != null)
                {
                    int selectId = int.Parse(AddNew_Backlink_MySite_ComboBox.SelectedValue.ToString());
                    AddNew_Backlink_MySitePage_ComboBox.ItemsSource = context.MySitesPages.Where(msp => msp.MySiteId == selectId).ToList().OrderBy(ms => ms.PageName);
                }
                else
                {
                    AddNew_Backlink_MySitePage_ComboBox.ItemsSource = null;
                }
            }
        }

    }
}
