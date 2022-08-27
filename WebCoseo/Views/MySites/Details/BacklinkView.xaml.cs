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

namespace WebCoseo.Views.MySites.Details
{
    /// <summary>
    /// Interaction logic for BacklinkView.xaml
    /// </summary>
    public partial class BacklinkView : UserControl
    {
        public static BacklinkView? _instance;
        public int GetBacklinkId { get; set; }
        public BacklinkView()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int backlinkId)
        {
            GetBacklinkId = backlinkId;
            using (AppServices.WebcoseoDbContext context = new())
            {
                var getBacklink = context.Backlinks.Single(x => x.Id == GetBacklinkId);
                var getMySitePage = context.MySitesPages.Single(x => x.Id == getBacklink.MySitePageId);
                Backlink_Url.Text = getBacklink.BacklinkUrl;
                MySite_Page_Url.Text = getMySitePage.Url;
                Next_Checked.Text = getBacklink.NextCheck.ToString();
                Last_Checked.Text = getBacklink.LastChecked.ToString();
                Date_Updated.Text = getBacklink.Updated.ToString();
                Date_Created.Text = getBacklink.Created.ToString();
                Backlink_Status.Text = getBacklink.Status;
                Backlink_Description.Text = getBacklink.Description;
                if (getBacklink.Status == "Active")
                {
                    Backlink_Status_Card.Background = Brushes.DarkGreen;
                }
                if (getBacklink.Status == "Lost")
                {
                    Backlink_Status_Card.Background = Brushes.DarkRed;
                }
            }
        }

        public void View_Backlink_Url_Click(object sender, RoutedEventArgs e)
        {
            
            ViewWebsite._instance.WebsitesBrowser_LoadPage(Backlink_Url.Text);
            ViewDetails._instance.GetView_ViewWebsite();
            
        }

    }
}
