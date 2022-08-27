using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.Settings.Details
{
    /// <summary>
    /// Interaction logic for ResetSoftware.xaml
    /// </summary>
    public partial class ResetSoftware : UserControl
    {
        public ResetSoftware()
        {
            InitializeComponent();
        }

        public void Reset_Software_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to reset the software? after this point data will not be recoverable",
                    "Reset Application",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var resetWindow = new ResetWindow();
                resetWindow.Show();
                MainWindow._instance.Close_Window(IsReset: true);
            }

        }
    }

}
