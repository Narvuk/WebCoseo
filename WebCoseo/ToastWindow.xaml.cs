using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebCoseo
{
    /// <summary>
    /// Interaction logic for ToastWindow.xaml
    /// </summary>
    public partial class ToastWindow : Window
    {
        string GetTitle { get; set; }
        string GetMessage { get; set; }
        public ToastWindow()
        {
            InitializeComponent();
        }

        public void OnLoad(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 2;
            this.Top = desktopWorkingArea.Bottom - this.Height - 2;
            Toast_Title.Text = GetTitle;
            Toast_Message.Text = GetMessage;
            Task.Run(() =>
            {
                Thread.Sleep(8000);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            });
        }

        public void Send_Message(string title, string message)
        {
            GetTitle = title;
            GetMessage = message;
        }


    }
}
