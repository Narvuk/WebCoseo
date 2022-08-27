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

namespace WebCoseo.Views.About.Details
{
    /// <summary>
    /// Interaction logic for AboutWebCoseo.xaml
    /// </summary>
    public partial class AboutWebCoseo : UserControl
    {
        public AboutWebCoseo()
        {
            InitializeComponent();
            Get_AboutInfo();
        }

        public void Get_AboutInfo()
        {
            using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
            {
                WebCoseo_Version_Number.Text = context.SystemConfig.Single(ck => ck.ConfigKey == "sys_app_version").ConfigValue;
                Release_Stability_Value.Text = context.SystemConfig.Single(cv => cv.ConfigKey == "sys_app_version_stability").ConfigValue;
            }
        }
    }
}
