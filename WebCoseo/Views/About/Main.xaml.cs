using System;
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

namespace WebCoseo.Views.About
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
