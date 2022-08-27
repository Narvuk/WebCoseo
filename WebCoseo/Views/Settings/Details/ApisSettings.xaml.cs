using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WebCoseo.Views.Settings.Details
{
    /// <summary>
    /// Interaction logic for ApisSettings.xaml
    /// </summary>
    public partial class ApisSettings : UserControl
    {
        public static ApisSettings? _instance;
        public ApisSettings()
        {
            _instance = this;
            InitializeComponent();
            GetSettings();
        }

        public void GetSettings()
        {
            using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
            {
                var apiPagePeeker = context.Settings.Single(x => x.SettingKey == "api_pagepeeker");
                Settings_PagePeeker_Description.Text = apiPagePeeker.Description;
                Settings_PagePeeker_Value.Text = apiPagePeeker.SettingValue;
            }
        }

        public void Save_Settings_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
            {
                var getToastWindow = new ToastWindow();
                var apiPagePeeker = context.Settings.Single(x => x.SettingKey == "api_pagepeeker");

                // if the value is not a number show error box to enter number
                if (Settings_PagePeeker_Value.Text == "")
                {
                    getToastWindow.Send_Message("Enter Value for PagePeeker", "If you do not have an API key please use 'Free' in the box to save");
                    getToastWindow.Show();
                    return;
                }
               
                apiPagePeeker.SettingValue = Settings_PagePeeker_Value.Text;

                // save the settings
                context.SaveChanges();
                getToastWindow.Send_Message("API Settings Saved", "You have successfully saved the API settings");
                getToastWindow.Show();
                /*Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ViewDetails_Page_Title.Content = "Settings Saved";
                        ViewDetails_Page_Title_BG.Background = Brushes.DarkGreen;
                    });
                    Thread.Sleep(3000);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ViewDetails_Page_Title.Content = "APIs Settings";
                        var colorConverter = new BrushConverter();
                        ViewDetails_Page_Title_BG.Background = (Brush)colorConverter.ConvertFromString("#121a2e");
                    });
                });*/
            }
        }

        public void Reset_Settings_Click(object sender, RoutedEventArgs e)
        {
            GetSettings();
        }
    }
}
