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

namespace WebCoseo.Views.Crawlers.Actions
{
    /// <summary>
    /// Interaction logic for AddSkipCrawl.xaml
    /// </summary>
    public partial class AddSkipCrawl : UserControl
    {
        public static AddSkipCrawl? _instance;
        public AddSkipCrawl()
        {
            _instance = this;
            InitializeComponent();
        }

        public void Submit_SkipCrawl_Button(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new())
            {
                if (Add_SkipCrawl_Domain_Data.Text.StartsWith("https://"))
                {
                    MessageBox.Show("Please remove https:// this part of the system uses just domain.com for example", "Use Domain without https://");
                    return;
                }

                if (Add_SkipCrawl_Domain_Data.Text == "")
                {
                    MessageBox.Show("Please enter a domain - for example use somthing like example.com ", "Please enter a Domain");
                    return;
                }

                crawlersContext.SkipCrawl.Add(new Models.Crawlers.SkipCrawl
                {
                    Domain = Add_SkipCrawl_Domain_Data.Text,
                    Reason = Add_SkipCrawl_Reason_Data.Text,
                    Created = DateTime.Now,
                   
                });

              
                crawlersContext.SaveChanges();

                // After work reset the form and go back to website view
                Add_SkipCrawl_Domain_Data.Text = null;
                Add_SkipCrawl_Reason_Data.Text = null;
                ViewDetails._instance.GetView_SkipCrawl_List();
                Details.SkipCrawl._instance.Get_SkipCrawlUrls();
            }
        }

        public void Cancel_SkipCrawl_Button(object sender, RoutedEventArgs e)
        {
            Add_SkipCrawl_Domain_Data.Text = null;
            Add_SkipCrawl_Reason_Data.Text = null;
            ViewDetails._instance.GetView_SkipCrawl_List();
            Details.SkipCrawl._instance.Get_SkipCrawlUrls();
        }
    }
}
