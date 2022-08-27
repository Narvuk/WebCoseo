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
using WebCoseo.Models;
using WebCoseo.Views.Crawlers.Actions;

namespace WebCoseo.Views.Crawlers.Actions
{
    /// <summary>
    /// Interaction logic for AddSkipContains.xaml
    /// </summary>
    public partial class AddSkipContains : UserControl
    {
        public static AddSkipContains? _instance;
        public AddSkipContains()
        {
            _instance = this;
            InitializeComponent();
        }

        public void Submit_SkipContains_Button(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new())
            {
                if (Add_SkipContains_Data.Text == "")
                {
                    MessageBox.Show("Please add what a url contains", "Url Contains cannot be blank");
                    return;
                }

                crawlersContext.SkipContains.Add(new Models.Crawlers.SkipContains
                {
                    Phrase = Add_SkipContains_Data.Text,
                    Reason = Add_SkipContains_Reason_Data.Text,
                    Created = DateTime.Now,

                });
                

                crawlersContext.SaveChanges();

                // After work reset the form and go back to website view
                Add_SkipContains_Data.Text = null;
                Add_SkipContains_Reason_Data.Text = null;
                ViewDetails._instance.GetView_SkipContains_List();
                Details.SkipContains._instance.Get_SkipContains();
            }
        }

        public void Cancel_SkipContains_Button(object sender, RoutedEventArgs e)
        {
            Add_SkipContains_Data.Text = null;
            Add_SkipContains_Reason_Data.Text = null;
            ViewDetails._instance.GetView_SkipContains_List();
            Details.SkipContains._instance.Get_SkipContains();
        }
    }
}
