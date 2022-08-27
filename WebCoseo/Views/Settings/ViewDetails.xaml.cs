using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Settings
{
    /// <summary>
    /// Interaction logic for ViewDetails.xaml
    /// </summary>
    public partial class ViewDetails : UserControl
    {
        public static ViewDetails? _instance;
        public ViewDetails()
        {
            _instance = this;
            InitializeComponent();
            SetActiveUserControl(SystemSettings);
        }

        public void SetActiveUserControl(UserControl control)
        {
            SystemSettings.Visibility = Visibility.Collapsed;
            ApisSettings.Visibility = Visibility.Collapsed;
            ResetSoftware.Visibility = Visibility.Collapsed;

            control.Visibility = Visibility.Visible;
        }


        public void Settings_System_Menu_Click(object sender, RoutedEventArgs e)
        {
            Details.SystemSettings._instance.GetSettings();
            SetActiveUserControl(SystemSettings);
        }


        public void ApisSettings_System_Menu_Click(object sender, RoutedEventArgs e)
        {
            Details.ApisSettings._instance.GetSettings();
            SetActiveUserControl(ApisSettings);
        }


        public void Settings_System_MainWindow()
        {
            SetActiveUserControl(SystemSettings);
        }

        public void Reset_Software_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(ResetSoftware);
        }
    }
}
