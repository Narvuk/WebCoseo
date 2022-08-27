using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Settings
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
            SetActiveUserControl(ViewDetails);
        }

        public void SetActiveUserControl(UserControl control)
        {
            ViewDetails.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }

        public void View_SettingsDetails()
        {
            SetActiveUserControl(ViewDetails);
        }
    }
}
