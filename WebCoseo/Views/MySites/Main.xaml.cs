using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.MySites
{
    /// <summary>
    /// Interaction logic for MySitesMain.xaml
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

        public void ViewMySitesListing()
        {
            SetActiveUserControl(Listing);
        }

        public void ViewMySitesDetails()
        {
            SetActiveUserControl(ViewDetails);
        }

        public void ViewMySitesAddNew()
        {
            SetActiveUserControl(AddNew);
        }

        public void ViewMySitesEdit()
        {
            SetActiveUserControl(Edit);
        }
    }
}
