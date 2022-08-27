using System.Windows.Controls;

namespace WebCoseo.Views
{
    /// <summary>
    /// Interaction logic for UpdatesMain.xaml
    /// </summary>
    public partial class UpdatesMain : UserControl
    {
        public static UpdatesMain _instance;
        public UpdatesMain()
        {
            _instance = this;
            InitializeComponent();
            Load_GetUpdates();
        }

        public async void Load_GetUpdates()
        {
            await WebsitesBrowser.EnsureCoreWebView2Async(null);
            WebsitesBrowser.CoreWebView2.Navigate("https://updates.narvuk.com/webcoseo/latest.html");
            WebsitesBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
            WebsitesBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            await WebsitesBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
        }
    }
}
