using System;
using System.Windows;
using System.Windows.Threading;

namespace WebCoseo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App? _instance;
        public App()
        {
            _instance = this;
            InitializeComponent();
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {

            MessageBox.Show(args.Exception.ToString());

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }

    }
}
