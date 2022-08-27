using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebCoseo.Views.Crawlers.Details
{
    /// <summary>
    /// Interaction logic for SkipCrawl.xaml
    /// </summary>
    public partial class SkipCrawl : UserControl
    {
        public static SkipCrawl? _instance;
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public SkipCrawl()
        {
            _instance = this;
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Get_SkipCrawlUrls();
        }

        private void ClearSkipCrawl_List()
        {
            SkipCrawl_List.ItemsSource = null;
            SkipCrawl_List.Items.Clear();
        }

        public void Get_SkipCrawlUrls()
        {
            ClearSkipCrawl_List();
            SkipCrawl_SearchBox.Text = "";
            Combobox_ItemsPerPage.Text = pageSize.ToString();
            CurrentPage_Label.Text = pageNumber.ToString();
            using AppServices.CrawlersDbContext crawlersContext = new();
            SkipCrawl_List.ItemsSource = crawlersContext.SkipCrawl.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        }

        private void SkipCrawl_Search_Click(object sender, RoutedEventArgs e)
        {
            ClearSkipCrawl_List();
            using AppServices.CrawlersDbContext crawlersContext = new();
            var skipCrawlFound = crawlersContext.SkipCrawl.Where(l => l.Domain.ToLower().Contains(SkipCrawl_SearchBox.Text.ToLower()))
                .Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            SkipCrawl_List.ItemsSource = skipCrawlFound.ToList();
        }

        private void SkipCrawl_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearSkipCrawl_List();
            Get_SkipCrawlUrls();
        }


        public void Get_Total_Pages()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                totalItems = crawlersContext.SkipCrawl.Count();
                if (totalItems % pageSize == 0)
                {
                    totalPages = totalItems / pageSize;
                    TotalPages_Label.Text = totalPages.ToString();
                }
                else
                {
                    totalPages = totalItems / pageSize + 1;
                    TotalPages_Label.Text = totalPages.ToString();
                }
            }
        }

        public void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = pageNumber + 1;
            Get_SkipCrawlUrls();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                Get_SkipCrawlUrls();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            Get_SkipCrawlUrls();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            Get_SkipCrawlUrls();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            Get_SkipCrawlUrls();
            Get_Total_Pages();
        }

        private void CurrentPage_Label_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                int numbervalue;
                bool selectionIsNumber = int.TryParse(CurrentPage_Label.Text, out numbervalue);
                if (selectionIsNumber)
                {
                    pageNumber = int.Parse(CurrentPage_Label.Text);
                    Get_SkipCrawlUrls();
                }
            }

        }

        public void AddNew_SkipCrawl_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails._instance.GetView_AddSkipCrawl();
        }

        public void Remove_SkipCrawlDomain_Click(object sender, RoutedEventArgs e)
        {
            
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using AppServices.CrawlersDbContext crawlersContext = new();
            // do check before remove
            var getSkipCrawl = crawlersContext.SkipCrawl.Single(x => x.Id == idconv);
           
            if (MessageBox.Show("Are you sure you wish to remove and allow this domain to be crawled again?",
                "Confirm Removal of Skipped Domain",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                crawlersContext.Remove(getSkipCrawl);
                crawlersContext.SaveChanges();
                Get_SkipCrawlUrls();
            }
            else
            {
                return;
            }


            


            
        }
    }
}
