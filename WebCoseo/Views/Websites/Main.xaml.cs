using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Websites
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public static Main? _instance;
        public Main()
        {
            _instance = this;
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(Listing);
        }

        public void SetActiveUserControl(UserControl control)
        {
            Listing.Visibility = Visibility.Collapsed;
            ViewDetails.Visibility = Visibility.Collapsed;
            AddNew.Visibility = Visibility.Collapsed;
            Edit.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }

        public void ViewWebsitesListing()
        {
            SetActiveUserControl(Listing);
        }

        public void ViewWebsitesDetails()
        {
            SetActiveUserControl(ViewDetails);
        }

        public void ViewAddNew()
        {
            SetActiveUserControl(AddNew);
        }

        public void ViewEdit()
        {
            SetActiveUserControl(Edit);
        }

    }
}
