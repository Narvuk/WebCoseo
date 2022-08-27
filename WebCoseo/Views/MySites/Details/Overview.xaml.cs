using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WebCoseo.Views.MySites.Details
{
    /// <summary>
    /// Interaction logic for MySitesInfo.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        public static Overview _instance;
        public int GetMySiteId { set; get; }
        public Overview()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int MySiteId)
        {
            this.GetMySiteId = MySiteId;
            Update_Overview();
        }

        public void Update_Overview()
        {
            using AppServices.CoreDbContext coreContext = new();
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var getWebSite = context.MySites.Single(x => x.Id == GetMySiteId);
                var convertImg = getWebSite.Url.Replace("https://", "");
                var apiPagePeeker = coreContext.Settings.Single(x => x.SettingKey == "api_pagepeeker");
                if (apiPagePeeker.SettingValue == "Free")
                {
                    Uri thumb_load = new Uri($"http://free.pagepeeker.com/v2/thumbs.php?size=x&url={convertImg}", UriKind.RelativeOrAbsolute);
                    ImageSource imageSource = new BitmapImage(thumb_load);
                    Overview_Load_Thumnail.Source = imageSource;
                }
                if (apiPagePeeker.SettingValue != "" && apiPagePeeker.SettingValue != "Free")
                {
                    Uri thumb_load = new Uri($"http://api.pagepeeker.com/v2/thumbs.php?size=x&code={apiPagePeeker.SettingValue}&url={convertImg}", UriKind.RelativeOrAbsolute);
                    ImageSource imageSource = new BitmapImage(thumb_load);
                    Overview_Load_Thumnail.Source = imageSource;
                }
                MySite_Description.Text = getWebSite.Description;

                BacklinksTotal_Count.Text = context.Backlinks.Count(x => x.MySiteId == GetMySiteId).ToString();
                BacklinksActiveTotal_Count.Text = context.Backlinks.Count(x => x.MySiteId == GetMySiteId && x.Status == "Active").ToString();
                BacklinksLostTotal_Count.Text = context.Backlinks.Count(x => x.MySiteId == GetMySiteId && x.Status == "Lost").ToString();
                BacklinksNewTotal_Count.Text = context.Backlinks.Count(x => x.MySiteId == GetMySiteId && x.Status == "New").ToString();

                if (context.Backlinks.Count(x => x.MySiteId == GetMySiteId) > 0)
                {
                    string latestBacklink = context.Backlinks.Where(x => x.MySiteId == GetMySiteId).OrderByDescending(x => x.Created).First().BacklinkUrl.ToString();
                    LatestBacklink_Data.Text = latestBacklink;
                    ViewBacklink_Button.Visibility = Visibility.Visible;
                }
                else
                {
                    LatestBacklink_Data.Text = "No Latest Backlink - At this moment in time.";
                    ViewBacklink_Button.Visibility = Visibility.Collapsed;
                }

            }
        }

        public void ViewBacklink_Click(object sender, RoutedEventArgs e)
        {
            using AppServices.WebcoseoDbContext context = new();
            var latestBacklink = context.Backlinks.Where(x => x.MySiteId == GetMySiteId).OrderByDescending(x => x.Created).First();
            BacklinkView._instance.LoadDetails(latestBacklink.Id);
            ViewDetails._instance.GetView_ViewBacklink();
        }
    }
}
