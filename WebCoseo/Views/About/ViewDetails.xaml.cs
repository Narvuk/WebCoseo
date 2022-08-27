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
    /// Interaction logic for ViewDetails.xaml
    /// </summary>
    public partial class ViewDetails : UserControl
    {
        public static ViewDetails? _instance;
        public ViewDetails()
        {
            _instance = this;
            InitializeComponent();
            SetActiveUserControl(AboutWebCoseo);
        }

        public void SetActiveUserControl(UserControl control)
        {
            AboutWebCoseo.Visibility = Visibility.Collapsed;
            
                        
            control.Visibility = Visibility.Visible;
        }


        public void AboutWebCoseo_Overview_MainWindow()
        {
            SetActiveUserControl(AboutWebCoseo);
        }

        public void AboutWebCoseo_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetActiveUserControl(AboutWebCoseo);
        }

       

    }
}
